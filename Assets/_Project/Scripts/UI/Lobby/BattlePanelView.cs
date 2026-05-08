using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivorsLike
{
    public class BattlePanelView : MonoBehaviour
    {
        [SerializeField] private Button _battleStartButton;

        public event Action OnGameStartClicked;

        public void Init()
        {
            _battleStartButton.onClick.AddListener(() => OnGameStartClicked?.Invoke());
        }

        public void SetInteractable(bool interactable)
        {
            _battleStartButton.interactable = interactable;
        }

        public void Destroy()
        {
            _battleStartButton?.onClick.RemoveAllListeners();
        }
    }
}
