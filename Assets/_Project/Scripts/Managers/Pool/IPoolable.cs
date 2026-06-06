
namespace SurvivorsLike
{
    public interface IPoolable
    {
        string PoolKey { get; set; }

        void OnGetFromPool();
        void OnReturnToPool();
        void ReturnToPool();
    }
}
