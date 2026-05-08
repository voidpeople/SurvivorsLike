using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorsLike
{
    public class BattlePanelView : MonoBehaviour
    {
        [SerializeField] private Button _battleStartButton;

        //BattlePanelPresenter의 OnGameStartClicked()가 구독한다.
        public event Action OnGameStartClicked;

        public void Init()
        {
            _battleStartButton.onClick.AddListener(() => OnGameStartClicked?.Invoke());
        }

        //플레이어의 입력을 막고자 할 때~
        public void SetInteractable(bool interactable)
        {
            //interactable에 false을 설정하면 버튼의 입력을 막게 된다.
            _battleStartButton.interactable = interactable;
        }

        public void Destroy()
        {
            _battleStartButton?.onClick.RemoveAllListeners();
        }
    }
}
