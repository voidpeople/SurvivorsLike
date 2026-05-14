using Cysharp.Threading.Tasks;
using UnityEngine;


namespace SurvivorsLike
{
    //인증, UserData 보관, 계정 탈퇴, 소셜 로그인, 계정 수정 등등 담당~
    public class AccountManager : SingletonMonoBehaviour<AccountManager>
    {
        public UserData UserData { get; private set; }

        public async UniTask PlayAsGuestAsync()
        {
            //익명으로 로그인
            string userID = await FirebaseManager.Instance.PlayAsGuestAsync();
            UserData = await FirebaseManager.Instance.LoadUserDataAsync(userID);
        }
    }
}
