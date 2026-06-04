using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Google.MiniJSON;
using System;
using System.Collections.Generic;
using System.Threading;
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

        public async UniTask<bool> InitAsync(CancellationToken ct)
        {
            // Firebase SDK 초기화
            bool isFirebaseInit = await InitFirebaseAsync(ct); 
            if (isFirebaseInit == false)
                return false;

            // DB 인스턴스 준비
            InitFirestore();  

            return true;
        }

        //Firebase 초기화 함수
        private async UniTask<bool> InitFirebaseAsync(CancellationToken ct)
        {
            //CheckAndFixDependenciesAsyncg함수는 Firebase를 사용하기 전에 반드시 필요한 네이티브 라이브러리가
            //기기에 준비되어 있는지 확인하고, 없으면 자동으로 수정을 시도한다.
            DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask().AttachExternalCancellation(ct);
            if(dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase 초기화 성공!");
            }
            else
            {
                //시스템 오류창 띄우기
                Debug.LogError($"Firebase 초기화 실패 : {dependencyStatus}");
                return false;
            }

            return true;
        }

        //서버DB와 연동되는 DB 인스턴스를 가져온다.
        private void InitFirestore()
        {
            _db = FirebaseFirestore.DefaultInstance;
        }

        //익명으로 로그인~
        public async UniTask<string> PlayAsGuestAsync(CancellationToken ct)
        {
            //FirebaseAuth.DefaultInstance은 앱 전체에서 공유하는 Auth 인스턴스임~
            _auth = FirebaseAuth.DefaultInstance;

            //이미 로그인 되어 있는지 검사
            if (_auth.CurrentUser != null)
            {
                Debug.Log($"이미 로그인 되어 있음! : {_auth.CurrentUser.UserId}");
                return _auth.CurrentUser.UserId;
            }

            try
            {
                // 익명 로그인: 이메일/비밀번호 없이 Firebase가 임시계정을 자동 생성해준다.
                // 같은 기기에서 재실행해도 동일한 UserId가 유지된다
                AuthResult result = await _auth.SignInAnonymouslyAsync().AsUniTask().AttachExternalCancellation(ct);
                Debug.Log($"익명 로그인 성공 - UserId : {result.User.UserId}");
                return result.User.UserId;
            }
            catch (OperationCanceledException)
            {
                throw;  // 일관성 유지, 상위 전파 명시
            }
            catch (FirebaseException e)
            {
                Debug.LogError($"익명 로그인 실패 : {e.Message}");
                return null;  // null 반환으로 실패 전달
            }
        }

        public async UniTask<UserData> LoadUserDataAsync(string userId, CancellationToken ct)
        {
            try
            {
                //Document는 Firestore 데이터베이스의 기본 저장 단위입니다.
                //JSON과 유사한 구조로 데이터를 저장하며, 고유한 ID를 가집니다.
                //그리고 DocumentReference은 그런 문서(Document)을 참조하는 객체의 타입입니다.
                DocumentReference docRef = _db.Collection("users").Document(userId)
                            .Collection("profile").Document("data");
                //DocumentReference가 가리키는 문서 데이터로 부터 특정 순간의 데이터를 가져온다.
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync().AsUniTask().AttachExternalCancellation(ct);
                //Source.Server을 인자로 넣어주지 않으면 로컬의 캐시 데이터를 가져와 사용한다.
                //DocumentSnapshot snapshot = await docRef.GetSnapshotAsync(Source.Server)
                //                                        .AsUniTask()
                //                                        .AttachExternalCancellation(ct);

                if (snapshot.Exists == false)
                {
                    Debug.Log("저장 데이터가 없음~ 신규 유저 생성 할 것~");

                    var newUserData = new UserData()
                    {
                        UserId = userId,
                        NickName = GenerateRandomNickName(), //자동생성 함수 추가
                        Level = 1,
                        Gold = 1000,
                        Gem = 100,
                        SelectedChapterId = 1,
                        LastClearedChapterId = 0,
                    };

                    //신규 유저 데이터를 서버에 저장
                    bool isSaved = await SaveUserDataAsync(newUserData, ct);
                    if(isSaved == false)
                    {
                        Debug.LogError("신규 유저 데이터 서버 저장 실패!");
                        return null;
                    }

                    return newUserData;
                }

                //기존 유저는 서버로 부터 받은 데이터 그대로 반환
                return snapshot.ConvertTo<UserData>();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (FirebaseException e)
            {
                Debug.LogError($"Firestore 데이터 로드 실패 (FirebaseException) : {e.Message}");
                return null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        //유저 데이터를 파이어베이스 서버에 저장
        public async UniTask<bool> SaveUserDataAsync(UserData userData, CancellationToken ct)
        {
            try
            {
                // Firestore 경로: users/{userId}/profile 문서에 저장
                // 구조: users (컬렉션) → userId (문서) → profile (하위 컬렉션) → data (문서)
                DocumentReference docRef = _db.Collection("users").Document(userData.UserId)
                                              .Collection("profile").Document("data");

                var dicData = new Dictionary<string, object>
                {
                    { "userId", userData.UserId },
                    { "nickName", userData.NickName },
                    { "level", userData.Level },
                    { "gold", userData.Gold },
                    { "gem", userData.Gem },
                    { "selectedChapterId", userData.SelectedChapterId },
                    { "lastClearedChapterId", userData.LastClearedChapterId },
                };

                //SetAsync함수를 통해 데이터를 서버에 업로드~
                await docRef.SetAsync(dicData).AsUniTask().AttachExternalCancellation(ct);;
                Debug.Log("서버에 유저 데이터 저장 완료~");
                return true;
            }
            catch (OperationCanceledException)
            {
                throw;  // 취소는 상위로 전파
            }
            catch (FirebaseException e)
            {
                Debug.LogError($"Firestore 데이터 저장 실패 (FirebaseException) : {e.Message}");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        private string GenerateRandomNickName()
        {
            //밀리세컨드 단위로 동일한 시간에 닉네임을 생성하지 않는 이상 중복 불가~
            long uid = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return $"player_{uid}";
        }
    }
}
