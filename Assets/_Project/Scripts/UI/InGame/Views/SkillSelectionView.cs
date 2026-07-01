using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;


namespace SurvivorsLike
{
    public class SkillSelectionView : MonoBehaviour
    {
        private const string SKILL_ICON_ATLAS_KEY = "atlas/skillicon";

        [SerializeField] private SkillCardView[] _cards;

        private AsyncOperationHandle<SpriteAtlas> _atlasHandle;

        public SkillCardView[] Cards => _cards;
        public SkillOptionData[] CurrentOptions { get; private set; }

        public async UniTask ShowAsync(SkillOptionData[] options, CancellationToken ct)
        {
            CurrentOptions = options;

            _atlasHandle = Addressables.LoadAssetAsync<SpriteAtlas>(SKILL_ICON_ATLAS_KEY);
            try
            {
                SpriteAtlas atlas = await _atlasHandle.ToUniTask(cancellationToken: ct);
                for (int ii = 0; ii < _cards.Length; ++ii)
                {
                    bool hasOption = ii < options.Length;
                    _cards[ii].gameObject.SetActive(hasOption);
                    if (hasOption)
                        _cards[ii].Init(options[ii], atlas);
                }
                gameObject.SetActive(true);
            }
            catch (System.OperationCanceledException)
            {
                if (_atlasHandle.IsValid())
                    Addressables.Release(_atlasHandle);
                throw;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            if (_atlasHandle.IsValid())
                Addressables.Release(_atlasHandle);
        }
    }
}
