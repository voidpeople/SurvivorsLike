using SurvivorsLike;
using UnityEngine;


namespace SurvivorsLike
{
    public class UserDataManager : SingletonMonoBehaviour<UserDataManager>
    {
        public UserData GetUserData { get; private set; }
    }
}
