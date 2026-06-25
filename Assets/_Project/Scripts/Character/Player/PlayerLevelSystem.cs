using UnityEngine;


namespace SurvivorsLike
{
    public class PlayerLevelSystem : MonoBehaviour
    {
        InGamePlayerLevelDataSO _levelData;

        public void Init(InGamePlayerLevelDataSO levelData)
        {
            _levelData = levelData;
        }

        public void AddExp(int exp)
        {
        }
    }
}
