using TMPro;
using UnityEngine;
using UnityEngine.U2D;
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

        public void Init(SkillOptionData optionData, SpriteAtlas atlas)
        {
            _nameLabel.text = optionData.SkillName;
            _descLabel.text = optionData.Description;
            _statusLabel.text = optionData.IsUpgrade
                ? $"Lv {optionData.NextLevel - 1}  →  {optionData.NextLevel}"
                : "NEW!";
            _statusImage.color = optionData.IsUpgrade ? _colorUpgrade : _colorNew;
            _iconImage.sprite = atlas.GetSprite(optionData.IconName);
        }
    }
}
