using System;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    public class GameResultPanelView : MonoBehaviour
    {
        [SerializeField] private Button _confirmButton;

        public event Action OnResultConfirmed;

        public void Init()
        {
            _confirmButton.onClick.AddListener(() => OnResultConfirmed?.Invoke());
        }

        public void SetInteractable(bool interactable)
        {
            //interactable에 false을 설정하면 버튼의 입력을 막게 된다.
            _confirmButton.interactable = interactable;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Destroy()
        {
            _confirmButton.onClick.RemoveAllListeners();
        }
    }
}
