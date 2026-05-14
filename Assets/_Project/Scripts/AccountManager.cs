using Cysharp.Threading.Tasks;
using UnityEngine;


namespace SurvivorsLike
{
    //인증, UserData 보관, 계정 탈퇴, 소셜 로그인, 계정 수정 등등 담당~
    public class AccountManager : SingletonMonoBehaviour<AccountManager>
    {
        public UserData UserData { get; private set; }

        public async UniTask<bool> InitAsync()
        {
            bool isFirebaseInit = false;
            for (int ii = 0; ii < 3; ++ii)
            {
                isFirebaseInit = await FirebaseManager.Instance.InitAsync();
                if (isFirebaseInit == true)
                    break;
            }
            if (isFirebaseInit == false)
                return false;


            string userId = null;
            for (int ii = 0; ii < 3; ++ii)
            {
                //익명으로 로그인
                userId = await FirebaseManager.Instance.PlayAsGuestAsync();
                if (userId != null)
                    break;
            }
            if (userId == null)
                return false;

            UserData = await FirebaseManager.Instance.LoadUserDataAsync(userId);

            return true;
        }

        public async UniTask PlayAsGuestAsync()
        {

            
        }
    }
}
