using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace SurvivorsLike
{
    public class UserDataManager : SingletonMonoBehaviour<UserDataManager>
    {
        public UserData UserData { get; private set; }

        public async UniTask<bool> LoadUserDataAsync(string userId, CancellationToken ct)
        {
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

        //현재 선택된 챕터 아이디를 서버 DB에 저장
        public async UniTask<bool> SaveSelectedChapterIdAsync(int selectedChapterId, CancellationToken ct)
        {
            UserData.selectedChapterId = selectedChapterId;
            return await FirebaseManager.Instance.SaveUserDataAsync(UserData, ct);
        }
    }
}
