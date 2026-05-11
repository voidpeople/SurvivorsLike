using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SurvivorsLike.UI.Lobby
{
    public class StageCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtStageNum;
        [SerializeField] private TextMeshProUGUI _txtStageName;
        [SerializeField] private Image _imgStageBg;

        public void Setup(ChapterDataSO data, int index)
        {
            _txtStageNum.text = $"STAGE {index + 1}";
            _txtStageName.text = data.displayName;
            if (data.thumbnail != null)
                _imgStageBg.sprite = data.thumbnail;
        }
    }
}
