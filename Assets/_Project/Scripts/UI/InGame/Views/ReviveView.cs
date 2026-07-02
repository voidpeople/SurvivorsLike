using System;
using UnityEngine;
using UnityEngine.UI;



public class ReviveView : MonoBehaviour
{
    [SerializeField] private Button _useEnergyButton;
    [SerializeField] private Button _watchAdButton;
    [SerializeField] private Button _closeButton;

    public event Action OnUseEnergyClicked;
    public event Action OnWatchAdClicked;
    public event Action OnCloseClicked;

    public void Init()
    {
        _useEnergyButton.onClick.AddListener(() => OnUseEnergyClicked?.Invoke());
        _watchAdButton.onClick.AddListener(() => OnWatchAdClicked?.Invoke());
        _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
    }

    public void SetInteractable(bool interactable)
    {
        _useEnergyButton.interactable = interactable;
        _watchAdButton.interactable = interactable;
        _closeButton.interactable = interactable;
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
        _useEnergyButton.onClick.RemoveAllListeners();
        _watchAdButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();
    }
}
