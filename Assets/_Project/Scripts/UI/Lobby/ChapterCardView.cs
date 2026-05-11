using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SurvivorsLike.UI.Lobby
{
    public class ChapterCardView : MonoBehaviour
    {
        [SerializeField] private Image _imgChapterBg;

        public void Setup(ChapterDataSO data, int index)
        {
            if (data.thumbnail != null)
                _imgChapterBg.sprite = data.thumbnail;
        }
    }
}
