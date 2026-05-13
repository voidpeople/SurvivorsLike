using SurvivorsLike;
using UnityEngine;



public class UserDataManager : SingletonMonoBehaviour<UserDataManager>
{
    public UserData GetUserData { get; private set; }
}
