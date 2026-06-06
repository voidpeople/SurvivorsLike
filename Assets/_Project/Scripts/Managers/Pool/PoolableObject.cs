using UnityEngine;


namespace SurvivorsLike
{
    public class PoolableObject : MonoBehaviour
    {
        public string Address { get; internal set; }


        public void Return()
        {
            //PoolManager.Instance.Return(this);
        }
    }
}
