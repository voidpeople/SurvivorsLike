using UnityEngine;

namespace SurvivorsLike
{
    public class SkillSelectionView : MonoBehaviour
    {
        [SerializeField] private SkillCardView[] _cards;  //인스펙터에서 3개 연결

        public SkillCardView[] Cards => _cards;

        public void Show(SkillOptionData[] options)
        {
            for (int i = 0; i < _cards.Length; i++)
                _cards[i].Init(options[i]);

            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
