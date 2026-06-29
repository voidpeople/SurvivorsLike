using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    public class SkillCardView : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private TextMeshProUGUI _descLabel;
        [SerializeField] private TextMeshProUGUI _statusLabel;
        [SerializeField] private Image _statusImage;
        [SerializeField] private Button _button;

        private static readonly Color _colorUpgrade = new(0.12f, 0.28f, 0.65f);
        private static readonly Color _colorNew = new(0.10f, 0.50f, 0.25f);

        public Button Button => _button;

        public void Init(SkillOptionData option)
        {
            _nameLabel.text = option.SkillName;
            _descLabel.text = option.Description;
            _statusLabel.text = option.IsUpgrade
                ? $"Lv {option.NextLevel - 1}  →  {option.NextLevel}"
                : "NEW!";
            _statusImage.color = option.IsUpgrade ? _colorUpgrade : _colorNew;

            // 아이콘 로드는 Addressables로 비동기 처리
        }
    }
}
