using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TriInspector;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Pool;

namespace SurvivorsLike.UI
{
    //로딩(대기) UI, 오류 팝업창, 경고 팝업창 담당 클래스~
    //SystemUIManager의 모든 UI은 기본적으로 백그라운드에 투명한 이미지를 배치하여 레이캐스팅을 막는다.
    //한번에 하나의 UI만 출력된다.
    public class SystemUIManager : SingletonMonoBehaviour<SystemUIManager>
    {
        ////////////////////////////////////////////////////////
        #region Dialog
        [SerializeField] private Transform _dialogLayer;
        [SerializeField] private SystemDialog _dialogPrefab;
        [SerializeField] private SystemUIConfigSO _systemUIConfigSO;

        [Title("Object Pool Settings")]
        [SerializeField, Range(1, 5)] private int _dialogPoolDefaultSize = 2;
        [SerializeField, Range(1, 10)] private int _dialogPoolMaxSize = 5;

        private readonly Stack<SystemDialog> _dialogStack = new();
        private ObjectPool<SystemDialog> _dialogPool;

        private void InitPDialogPool()
        {
            _dialogPool = new ObjectPool<SystemDialog>(
                //풀에 오브젝트가 부족할 때 호출되어 새로 오브젝트를 생성하는 함수 정의
                createFunc: () =>
                {
                    //다이얼로그의 인스턴스 생성 후 dialogLayer에 자식으로 추가하고
                    //오브젝트는 비 활성화 시킨다.
                    var d = Instantiate(_dialogPrefab, _dialogLayer);
                    SystemDialog systemDialog = d.GetComponent<SystemDialog>();
                    systemDialog.Init(_systemUIConfigSO);
                    d.gameObject.SetActive(false);

                    return d;
                },

                //오브젝트을 풀에서 꺼낼 때 활성화 되게 설정
                actionOnGet: d => d.gameObject.SetActive(true),
                //오브젝트을 풀에 넣을 때 비 활성화 설정
                actionOnRelease: d => d.gameObject.SetActive(false),
                //풀의 maxSize을 초과하면 넘치는 오브젝트를 완전히 삭제 할 때 호출하는 로직 등록
                actionOnDestroy: d => Destroy(d.gameObject),
                //collectionCheck을 true로 설정하면 이미 반납된 오브젝트을 다시 반납시 예외 처리를 해준다.
                //하지만 성능상 false를 권장한다.
                collectionCheck: false,
                //풀의 인스턴스 생성 후 생성하여 채워야 할 오브젝트 갯수
                defaultCapacity: _dialogPoolDefaultSize,
                //풀이 보유할 수 있는 최대 오브젝트의 수 (이 사이즈를 초과하는 오브젝트는 자동으로 Destroy가 된다.)
                maxSize: _dialogPoolMaxSize
             );
        }

        private void ShowDialog(DialogConfig config)
        {
            //현재 스택의 최 상단에 있는 다이얼로그를 숨긴다.
            if (_dialogStack.TryPeek(out var currentDialog))
                currentDialog.Hide();

            var dialog = _dialogPool.Get();
            dialog.transform.SetParent(_dialogLayer, false);
            //SetAsLastSibling()함수는 해당 오브젝트를 부모 객체에서 부모 자식 목록 중
            //제일 마지막에 위치 시켜 준다. 즉 최 상단에 위치 시켜서 보이게 해준다.
            dialog.transform.SetAsLastSibling();
            dialog.Show(config, onClose: CloseTopDialog);

            //스택의 최 상단에 추가
            _dialogStack.Push(dialog);
        }

        //확인 버튼 만 존재하는 단순 알림 다이얼로그 창
        public void ShowAlert(
            string title,
            string message,
            DialogType type,
            string confirmText = "확인",
            Action onConfirm = null)
        {
            ShowDialog(new DialogConfig
            {
                Title = title,
                Message = message,
                Type = type,
                ConfirmText = confirmText,
                OnConfirm = onConfirm
            });
        }

        public async UniTask ShowAlertAsync(
            string title,
            string message,
            DialogType type,
            string confirmText = "확인",
            CancellationToken ct = default)
        {
            var tcs = new UniTaskCompletionSource();

            ShowDialog(new DialogConfig
            {
                Title = title,
                Message = message,
                Type = type,
                ConfirmText = confirmText,
                OnConfirm = () => tcs.TrySetResult(),
            });

            using var reg = ct.Register(() => tcs.TrySetResult());

            await tcs.Task;
        }

        //확인와 취소 버튼이 있는 다이얼로그 선택 창
        public void ShowConfirm(
            string title,
            string message,
            DialogType type,
            string confirmText = "확인",
            string cancelText = "취소",
            Action onConfirm = null,
            Action onCancel = null)
        {
            ShowDialog(new DialogConfig
            {
                Title = title,
                Message = message,
                Type = type,
                ConfirmText = confirmText,
                CancelText = cancelText,
                OnConfirm = onConfirm,
                OnCancel = onCancel
            });
        }

        //확인 취소 버튼이 있는 비동기 다이얼로그 창
        /*
         * CancellationToken ct : 외부에서 "이 작업을 취소해라" 신호를 보낼 수 있는 토큰.
         *  - default = 아무도 취소 신호를 보내지 않는 기본 토큰
         *  - 씬 전환, 앱 종료 시 등 외부에서 강제 종료할 때 사용한다.* 
         */
        public async UniTask<bool> ShowConfirmAsync(
            string title,
            string message,
            DialogType type,
            string confirmText = "확인",
            string cancelText = "취소",
            CancellationToken ct = default)
        {
            /*
             * UniTaskCompletionSource<T> : 비동기 상황에서도 내가 원하는 타이밍에 직접 결과값을 반환할 수 있게 해 주는 객체
             * 1.tcs 를 생성하면 내부에 아직 완료되지 않은 UniTask<bool> 가 만들어진다.
             * 2.누군가 tcs.TrySetResult(값) 을 호출하는 순간 Task 가 완료 상태가 된다.
             * 3.아래의 await tcs.Task 는 그 순간까지 여기서 대기한다.
             */
            var tcs = new UniTaskCompletionSource<bool>();

            ShowDialog(new DialogConfig
            {
                Title = title,
                Message = message,
                Type = type,
                ConfirmText = confirmText,
                CancelText = cancelText,
                // [확인] 버튼을 누르면 → tcs 에 true 를 설정 → 아래 await 가 true 를 받고 실행 재개
                OnConfirm = () => tcs.TrySetResult(true),
                // [취소] 버튼을 누르면 → tcs 에 false 를 설정 → 아래 await 가 false 를 받고 실행 재개
                OnCancel = () => tcs.TrySetResult(false)
            });

            //CancellationToken 객체인 ct에 취소 콜백 함수를 등록한다.
            //즉 외부에서 비동기 작업에 대한 취소 신호가 오면 Register() 함수로 등록된
            //tcs.TrySetResult(false) 로직이 실행되어 Task가 완료 상태가 된다.
            //그러면 아래의 await에서 대기중 이던 로직이 재개되어 tcs.Task 값을 반환한다.
            using var reg = ct.Register(() => tcs.TrySetResult(false));

            //tcs.Task은 아직 완료되지 않은 UniTask<bool> 태스크 객체이다.
            //await 명령은 아직 완료되지 않은 tcs.Task 태스크가 완료될 때 까지 실행을 멈추고 기다린다.
            //그러다가 위의 tcs.TrySetResult() 로직 중 한개가 실행되면 태스크가 완료된 거므로
            //대기 상태를 중지하고 로직 실행을 재개하면서 tcs.TrySetResult을 통해 설정된 bool 값을 반환한다.
            //(그러니까 await 명령이 tcs.TrySetResult()함수가 실행되어 태스크가 완료될 때 까지 로직의 실행을 중지하고
            //대기를 타고 있는 거임)
            return await tcs.Task;
        }

        //현재 여러개의 다이얼로그 창들이 활성화 되어 있다면
        //최 상단의 다이얼로그를 닫기를 해준다.
        public void CloseTopDialog()
        {
            if (_dialogStack.TryPop(out var topDialog) == false)
                return;

            //탑 다이얼로그를 풀에 반환한다.
            _dialogPool.Release(topDialog);
        }
        #endregion Dialog

        ////////////////////////////////////////////////////////
        #region Indicator
        #endregion Indicator


        ////////////////////////////////////////////////////////
        #region Common
        protected override void ChildAwake()
        {
            _systemUIConfigSO.Init();
            InitPDialogPool();
        }

        protected override void OnDestroy()
        {
            _dialogPool?.Dispose();

            base.OnDestroy();
        }
        #endregion Common

    }
}
