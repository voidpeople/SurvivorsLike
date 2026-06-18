using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace SurvivorsLike
{
    //로딩(대기) UI, 오류 팝업창, 경고 팝업창 담당 클래스~
    //SystemUIManager의 모든 UI은 기본적으로 백그라운드에 투명한 이미지를 배치하여 레이캐스팅을 막는다.
    //한번에 하나의 UI만 출력된다.
    public class SystemUIManager : SingletonMonoBehaviour<SystemUIManager>
    {
        ////////////////////////////////////////////////////////
        #region Dialog
        [Header("Dialog")]
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

        // 시스템 이벤트 전용 메서드 - 호출부는 콜백만 전달
        public void ShowNetworkErrorDialog(Action onRetry, Action onQuit)
            => ShowDialog(DialogConfig.NetworkError(onRetry, onQuit));

        public void ShowAuthErrorDialog(Action onRetry, Action onQuit)
            => ShowDialog(DialogConfig.AuthError(onRetry, onQuit));

        public void ShowMaintenanceDialog(string message)
            => ShowDialog(DialogConfig.Maintenance(message));

        public void ShowSessionExpiredDialog(Action onConfirm)
            => ShowDialog(DialogConfig.SessionExpired(onConfirm));

        public void ShowForceUpdateDialog(Action onUpdate)
            => ShowDialog(DialogConfig.ForceUpdate(onUpdate));

        public void ShowNoticeDialog(string title, string message, Action onConfirm = null)
            => ShowDialog(DialogConfig.Notice(title, message, onConfirm));

        public void ShowCriticalErrorDialog(string message = null)
            => ShowDialog(DialogConfig.CriticalError(message ?? "예기치 못한 오류가 발생했습니다.\n앱을 재시작해 주세요."));

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
            Debug.Assert(_dialogLayer     != null, $"{nameof(SystemUIManager)}::ChildAwake=> _dialogLayer is null");
            Debug.Assert(_dialogPrefab    != null, $"{nameof(SystemUIManager)}::ChildAwake=> _dialogPrefab is null");
            Debug.Assert(_systemUIConfigSO != null, $"{nameof(SystemUIManager)}::ChildAwake=> _systemUIConfigSO is null");
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
