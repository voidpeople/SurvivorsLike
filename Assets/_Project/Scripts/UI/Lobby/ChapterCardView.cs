using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SurvivorsLike.UI.Lobby
{
    //View은 받은 데이터를 그대로 표시하는 역활을 해야 한다.
    //아틀라스르 조회하는 기능은 외부에서 할 것~
    public class ChapterCardView : MonoBehaviour
    {
        [SerializeField] private Image _chapterImage;

        public void Setup(ChapterDataSO data, int index)
        {
            //View에서 스프라이트를 조회하여 설정 하는건 MVP 패턴에 위반?
            if (data.thumbnail != null)
                _chapterImage.sprite = data.thumbnail;
        }
    }
}
