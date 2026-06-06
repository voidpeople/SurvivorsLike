using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace SurvivorsLike
{
    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        private readonly Dictionary<string, ObjectPool<GameObject>> _poolDic = new();
        //나중에 Release을 위해 핸들 보관
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _asyncOpHandleDic = new();

        public GameObject Get(string poolKey)
        {
            if (_poolDic.TryGetValue(poolKey, out var pool))
            {
                return pool.Get();
            }

            Debug.LogError($"[PoolManager] 미등록 키: {poolKey}");
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

        //반환
        //public void Return(PoolableObject poolableObj)
        //{
        //    if (_poolDic.TryGetValue(poolableObj.PoolKey, out var pool))
        //        pool.Release(poolableObj.gameObject);
        //    else
        //        Object.Destroy(poolableObj.gameObject);
        //}

        public void Return(IPoolable poolable)
        {
            MonoBehaviour mono = poolable as MonoBehaviour;
            if (_poolDic.TryGetValue(poolable.PoolKey, out var pool))
                pool.Release(mono.gameObject);
            else
                Object.Destroy(mono.gameObject);
        }

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
            catch
            {
                Addressables.Release(handle);
                throw;
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
                    //GameObject obj = Object.Instantiate(prefab);
                    //PoolableObject poolableObj = obj.GetComponent<PoolableObject>();
                    //if(poolableObj == null)
                    //    poolableObj = obj.AddComponent<PoolableObject>();
                    //poolableObj.PoolKey = poolKey;

                    //return obj;

                    GameObject obj = Object.Instantiate(prefab);
                    IPoolable poolableObj = obj.GetComponent<IPoolable>();
                    if (poolableObj == null)
                    {
                        Debug.LogError($"PoolManager::CreatePoolAsync() - IPoolable가 없는 오브젝트 있음~ : poolKey - {poolKey} ");
                        return null;
                    }                        
                    poolableObj.PoolKey = poolKey;

                    return obj;
                },
                //오브젝트을 풀에서 꺼낼 때 활성화 되게 설정
                actionOnGet: obj => obj.SetActive(true),
                //오브젝트을 풀에 넣을 때 비 활성화 설정
                actionOnRelease: obj => obj.SetActive(false),
                //풀의 maxSize을 초과하면 넘치는 오브젝트를 완전히 삭제 할 때 호출하는 로직 등록
                actionOnDestroy: obj => Object.Destroy(obj),
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

        protected override void OnDestroy()
        {
            foreach (string poolKey in new List<string>(_poolDic.Keys))
            {
                ReleasePool(poolKey);
            }

            base.OnDestroy();
        }
    }
}
