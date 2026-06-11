using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace SurvivorsLike
{
    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        // ─── private 필드 ────────────────────────────────────────────────────
        private Dictionary<string, ObjectPool<GameObject>> _poolDic = new();
        private Dictionary<GameObject, string> _poolKeyDic = new();
        //나중에 Release을 위해 핸들 보관
        private Dictionary<string, AsyncOperationHandle<GameObject>> _asyncOpHandleDic = new();
        // 풀마다 독립적인 IPoolable 캐시 — GetComponent 반복 호출 방지
        private Dictionary<GameObject, IPoolable> _iPoolableCacheDic = new();

        private bool _isPrewarming;


        // ─── Unity Lifecycle ─────────────────────────────────────────────────
        protected override void OnDestroy()
        {
            foreach (string poolKey in new List<string>(_poolDic.Keys))
            {
                ReleasePool(poolKey);
            }

            base.OnDestroy();
        }


        // ─── Public Methods ───────────────────────────────────────────────────
        public GameObject Get(string poolKey)
        {
            if (_poolDic.TryGetValue(poolKey, out var pool))
            {
                return pool.Get();
            }

            Debug.LogError($"{nameof(PoolManager)}::Get => Unregistered key: {poolKey}");
            return null;
        }

        //EnemyController enemy = PoolManager.Instance.Get<EnemyController>("enemy/spiderBot");
        public T Get<T>(string poolKey) where T : Component
        {
            GameObject obj = Get(poolKey);
            if (obj != null)
                return obj.GetComponent<T>();

            return null;
        }

        public void Return(IPoolable poolable)
        {
            MonoBehaviour mono = poolable as MonoBehaviour;
            if (_poolKeyDic.TryGetValue(mono.gameObject, out string key))
            {
                if (_poolDic.TryGetValue(key, out var pool))
                    pool.Release(mono.gameObject);
                else
                {
                    Object.Destroy(mono.gameObject);
                    Debug.LogError($"PoolManager::Return - ObjectPool이 존재하지 않는 오브젝트를 반환 하려함~ - PoolKey: {key}, GameObject: {mono.gameObject.name}");
                }
            }
            else
            {
                Object.Destroy(mono.gameObject);
                Debug.LogError($"PoolManager::Return - PoolKey가 존재하지 않는 오브젝트를 반환 하려함~ - PoolKey: {key}, GameObject: {mono.gameObject.name}");
            }
        }

        //TODO: 추후에 대규모 오브젝트 생성시 드랍 발생할 경우 Time Budget 방식으로 수정할 것~
        //사전에 오브젝트 생성
        public async UniTask PreCreateAsync(
            string poolKey,
            int count,
            int batchSize = 10,
            CancellationToken ct = default)
        {
            if (!_poolDic.TryGetValue(poolKey, out var pool))
            {
                Debug.LogError($"[PoolManager] PreCreateAsync 실패 — 미등록: {poolKey}");
                return;
            }

            _isPrewarming = true;

            GameObject[] tempObjs = new GameObject[count];
            for (int ii = 0; ii < count; ++ii)
            {
                tempObjs[ii] = pool.Get();

                //일단 기본적으로 한 프레임에 10개씩 생성~
                if ((ii + 1) % batchSize == 0)
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            for (int ii = 0; ii < count; ++ii)
            {
                pool.Release(tempObjs[ii]);
            }

            _isPrewarming = false;
        }

        //풀 생성~
        public async UniTask CreatePoolAsync(
            string poolKey,
            int defaultCapacity = 100,
            int maxSize = 300,
            CancellationToken ct = default)
        {
            //중복 방지
            if (_poolDic.ContainsKey(poolKey))
                return;

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(poolKey);

            GameObject prefab = null;
            try
            {
                prefab = await handle.ToUniTask(cancellationToken: ct);
            }
            finally
            {
                if (prefab == null)
                    Addressables.Release(handle);
            }

            if (prefab == null)
            {
                Debug.LogError($"[PoolManager] 프리팹 로드 실패: {poolKey}");
                Addressables.Release(handle);
                return;
            }

            _asyncOpHandleDic.Add(poolKey, handle);

            ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Object.Instantiate(prefab);
                    IPoolable poolableObj = obj.GetComponent<IPoolable>();
                    if (poolableObj == null)
                    {
                        Debug.LogError($"PoolManager::CreatePoolAsync() - IPoolable가 없는 오브젝트 있음~ : poolKey - {poolKey} ");
                        return null;
                    }
                    _poolKeyDic.Add(obj, poolKey);
                    _iPoolableCacheDic.Add(obj, poolableObj);

                    return obj;
                },
                //GameObject Get(string poolKey) 함수에서
                //return pool.Get(); 로직을 실행할 때 actionOnGet 로직이 실행됨~
                actionOnGet: obj =>
                {
                    obj.SetActive(true);

                    //Prewarm 중 일 때는 OnSpawn 호출 차단
                    if (_isPrewarming == false)
                    {
                        if (_iPoolableCacheDic.TryGetValue(obj, out var poolable))
                        {
                            poolable.OnSpawn();
                        }
                    }
                },
                //void Return(IPoolable poolable) 함수에서
                //pool.Release(mono.gameObject); 로직이 실행될 때 actionOnRelease 로직이 실행됨~
                actionOnRelease: obj =>
                {
                    //Prewarm 중 일 때는 OnDespawn 호출 차단
                    if (_isPrewarming == false)
                    {
                        if (_iPoolableCacheDic.TryGetValue(obj, out var poolable))
                        {
                            poolable.OnDespawn();
                        }
                    }
                    obj.SetActive(false);
                },
                actionOnDestroy: obj =>
                {
                    _iPoolableCacheDic.Remove(obj);
                    _poolKeyDic.Remove(obj);
                    Object.Destroy(obj);
                },
                collectionCheck: false,
                //풀의 인스턴스 생성 후 생성하여 채워야 할 오브젝트 갯수
                defaultCapacity: defaultCapacity,
                //풀이 보유할 수 있는 최대 오브젝트의 수 (이 사이즈를 초과하는 오브젝트는 자동으로 Destroy가 된다.)
                maxSize: maxSize);
            _poolDic.Add(poolKey, newPool);
        }

        //인게임에서 나올 떄 해당 챕터의 오브젝트들 해제~
        public void ReleasePool(string poolKey)
        {
            if (_poolDic.TryGetValue(poolKey, out var pool))
            {
                //풀 내 오브젝트 Destroy
                pool.Dispose();
                _poolDic.Remove(poolKey);
            }

            if (_asyncOpHandleDic.TryGetValue(poolKey, out var handle))
            {
                //메모리 해제
                Addressables.Release(handle);
                _asyncOpHandleDic.Remove(poolKey);
            }
        }
    }
}
