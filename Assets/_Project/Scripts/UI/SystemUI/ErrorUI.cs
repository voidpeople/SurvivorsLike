using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ErrorUI : BaseSystemUI
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button actionButton;

    private Action _onAction;

    //public void SetData(SystemUIData data, Action onAction)
    //{
    //    titleText.text = data.title;
    //    messageText.text = data.message;

    //    _onAction = onAction;

    //    actionButton.onClick.RemoveAllListeners();
    //    actionButton.onClick.AddListener(() => _onAction?.Invoke());
    //}
}
