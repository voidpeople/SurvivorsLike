using SurvivorsLike.UI.Lobby;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    public class BattlePanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _chapterSelectPanel;
        [SerializeField] private Button _chapterPanelButton;
        [SerializeField] private Button _battleStartButton;

        //BattlePanelPresenter의 OnGameStartClicked()가 구독한다.
        public event Action OnGameStartClicked;

        public void Init()
        {
            _chapterPanelButton.onClick.AddListener(() => _chapterSelectPanel.SetActive(true));
            _battleStartButton.onClick.AddListener(() => OnGameStartClicked?.Invoke());
        }

        //플레이어의 입력을 막고자 할 때~
        public void SetInteractable(bool interactable)
        {
            //interactable에 false을 설정하면 버튼의 입력을 막게 된다.
            _battleStartButton.interactable = interactable;
        }

        //책임 분리 원칙
        //아틀라스에서 스프라이트를 찾는 행위는 "데이터 조회"을 의미하고 이것은 View의 책임이 아님~
        public void SetChapterPanelButtonImage(Sprite newSprite)
        {
            Image img = _chapterPanelButton.GetComponent<Image>();
            img.sprite = newSprite;
        }

        public void Destroy()
        {
            _battleStartButton?.onClick.RemoveAllListeners();
        }
    }
}
