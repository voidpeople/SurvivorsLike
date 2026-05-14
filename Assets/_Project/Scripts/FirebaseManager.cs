using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Google.MiniJSON;
using UnityEngine;


namespace SurvivorsLike
{
    public class FirebaseManager : SingletonMonoBehaviour<FirebaseManager>
    {
        private FirebaseAuth      _auth; //인증 담당
        private FirebaseFirestore _db;   //서버 DB 연동

        protected override void ChildAwake()
        {
        }

        //Firebase 초기화 함수
        private async UniTask InitFirebaseAsync()
        {
            //CheckAndFixDependenciesAsyncg함수는 Firebase를 사용하기 전에 반드시 필요한 네이티브 라이브러리가
            //기기에 준비되어 있는지 확인하고, 없으면 자동으로 수정을 시도한다.
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
            if(dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase 초기화 성공!");
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패 : {dependencyStatus}");
            }
        }

        //서버DB와 연동되는 DB 인스턴스를 가져온다.
        private void InitFirestore()
        {
            _db = FirebaseFirestore.DefaultInstance;
        }

        //익명으로 로그인~
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
            AuthResult result = await _auth.SignInAnonymouslyAsync().AsUniTask();
            Debug.Log($"익명 로그인 성공 - UserID : {result.User.UserId}");

            //이 유저 아이디를 저장해서 사용할 것~
            return result.User.UserId;
        }

        public async UniTask<UserData> LoadUserDataAsync(string userID)
        {
            //Document는 Firestore 데이터베이스의 기본 저장 단위입니다.
            //JSON과 유사한 구조로 데이터를 저장하며, 고유한 ID를 가집니다.
            //그리고 DocumentReference은 그런 문서(Document)을 참조하는 객체의 타입입니다.
            DocumentReference docRef = _db.Collection("users").Document(userID)
                            .Collection("profile").Document("data");

            //DocumentReference가 가리키는 문서 데이터로 부터 특정 순간의 데이터를 가져온다.
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync().AsUniTask();

            if(snapshot.Exists == false)
            {
                Debug.Log("저장 데이터가 없음~ 신규 유저 생성 할 것~");

                var newUserData = new UserData()
                {
                    userID = userID,
                    nickname = "", //자동생성 함수 추가
                    level = 1,
                    gold = 0,
                    gem = 0,
                };

                //await SaveUserDataAsync(newUserData);
                return newUserData;
            }

            //기존 유저는 서버로 부터 받은 데이터 그대로 반환
            return snapshot.ConvertTo<UserData>();
        }

        public async UniTask SaveUserDataAsync(UserData userData)
        {

        }
    }
}
