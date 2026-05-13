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
