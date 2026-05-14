using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Cysharp.Threading.Tasks;
using Firebase.Auth;


namespace SurvivorsLike
{
    public class FirebaseManager : SingletonMonoBehaviour<FirebaseManager>
    {
        private FirebaseAuth _auth;

        protected override void ChildAwake()
        {
        }

        //Firebase 초기화 함수
        private async UniTask InitFirebaseAsync()
        {
            //CheckAndFixDependenciesAsyncg함수는 Firebase를 사용하기 전에 반드시 필요한 네이티브 라이브러리가
            //기기에 준비되어 있는지 확인하고, 없으면 자동으로 수정을 시도한다.
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
            if(dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase 초기화 성공!");
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패 : {dependencyStatus}");
            }
        }

        private async UniTask<string> PlayAsGuest()
        {
            //FirebaseAuth.DefaultInstance은 앱 전체에서 공유하는 Auth 인스턴스임~
            _auth = FirebaseAuth.DefaultInstance;

            //이미 로그인 되어 있는지 검사
            if(_auth.CurrentUser != null)
            {
                Debug.Log($"이미 로그인 되어 있음! : {_auth.CurrentUser.UserId}");
                return _auth.CurrentUser.UserId;
            }

            // 익명 로그인: 이메일/비밀번호 없이 Firebase가 임시계정을 자동 생성해준다.
            // 같은 기기에서 재실행해도 동일한 UserId가 유지된다
            var result = await _auth.SignInAnonymouslyAsync().AsUniTask();
            Debug.Log($"익명 로그인 성공 - UserID : {result.User.UserId}");

            //이 유저 아이디를 저장해서 사용할 것~
            return result.User.UserId;
        }
    }
}
