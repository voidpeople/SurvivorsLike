using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    //인증, 계정 탈퇴, 소셜 로그인, 계정 수정 등등 담당~
    public class AccountManager : SingletonMonoBehaviour<AccountManager>
    {
        public string UserId { get; private set; }

        public async UniTask<bool> SetupAsync(CancellationToken ct)
        {
            //파이어베이스 SDK 초기화
            bool isFirebaseInit = false;
            for (int ii = 0; ii < 3; ++ii)
            {
                isFirebaseInit = await FirebaseManager.Instance.InitAsync(ct);
                if (isFirebaseInit == true)
                    break;

                await UniTask.Delay(1000, cancellationToken: ct);
            }
            if (isFirebaseInit == false)
                return false;

            //익명 로그인
            for (int ii = 0; ii < 3; ++ii)
            {
                //익명으로 로그인
                UserId = await FirebaseManager.Instance.PlayAsGuestAsync(ct);
                if (UserId != null)
                    break;

                await UniTask.Delay(1000, cancellationToken: ct);
            }
            if (UserId == null)
                return false;

            ////유저 데이터 로드
            //UserData userData = null;
            //for (int ii = 0; ii < 3; ++ii)
            //{
            //    userData = await FirebaseManager.Instance.LoadUserDataAsync(userId, ct);
            //    if (userData != null)
            //        break;

            //    await UniTask.Delay(1000, cancellationToken: ct);
            //}
            //if (userData == null)
            //    return false;

            return true;
        }
    }
}
