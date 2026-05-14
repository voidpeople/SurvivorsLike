using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    //인증, UserData 보관, 계정 탈퇴, 소셜 로그인, 계정 수정 등등 담당~
    public class AccountManager : SingletonMonoBehaviour<AccountManager>
    {
        public UserData UserData { get; private set; }

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
            string userId = null;
            for (int ii = 0; ii < 3; ++ii)
            {
                //익명으로 로그인
                userId = await FirebaseManager.Instance.PlayAsGuestAsync(ct);
                if (userId != null)
                    break;

                await UniTask.Delay(1000, cancellationToken: ct);
            }
            if (userId == null)
                return false;

            //유저 데이터 로드
            UserData = null;
            for (int ii = 0; ii < 3; ++ii)
            {
                UserData = await FirebaseManager.Instance.LoadUserDataAsync(userId, ct);
                if (UserData != null)
                    break;

                await UniTask.Delay(1000, cancellationToken: ct);
            }
            if (UserData == null)
                return false;

            return true;
        }
    }
}
