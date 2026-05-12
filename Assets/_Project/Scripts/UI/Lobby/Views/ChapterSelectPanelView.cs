using SurvivorsLike.UI.Lobby;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    public class ChapterSelectPanelView : MonoBehaviour
    {
        [SerializeField] private ChapterScrollController _chapterScrollCtrl;
        [SerializeField] private Button _selectButton; //챕터 선택 후 나가기
        [SerializeField] private Button _exitButton;   //그냥 나가기

        private void Start()
        {
        }
    }
}
