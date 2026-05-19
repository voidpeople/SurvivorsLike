using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace SurvivorsLike
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private Renderer _groundRenderer;
        [SerializeField] private Light _mainLight;

        private AsyncOperationHandle<Material> _matAssetHandle;

        public async UniTask LoadAssetsAsync(MapDataSO mapData, CancellationToken ct)
        {
            await UniTask.WhenAll(
                LoadMaterialAsync(mapData.groundMaterialKey, ct)
            );
        }
        private async UniTask LoadMaterialAsync(string key, CancellationToken ct)
        {
            _matAssetHandle = Addressables.LoadAssetAsync<Material>(key);
            await _matAssetHandle.ToUniTask(cancellationToken: ct);
        }

        public async UniTask SetupMapAsync(MapDataSO mapData, CancellationToken ct)
        {
            _groundRenderer.sharedMaterial = _matAssetHandle.Result;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = mapData.ambientLightColor;

            _mainLight.color = mapData.mainLightColor;
        }

        public void ReleaseAssets()
        {
            if (_matAssetHandle.IsValid()) Addressables.Release(_matAssetHandle);
        }
    }
}
