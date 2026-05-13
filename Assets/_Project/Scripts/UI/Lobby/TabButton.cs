using System;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : Button
{
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _selectedSprite;

    public event Action<TabButton> OnSelected;

    //private bool _isSelected;

    protected override void Awake()
    {
        base.Awake();
        transition = Transition.None; // Transition은 코드로 제어
        Deselect();
    }

    // Button 클릭 이벤트 내부 처리
    protected override void OnEnable()
    {
        base.OnEnable();
        onClick.AddListener(HandleClick);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        OnSelected?.Invoke(this);
    }

    public void Select()
    {
        image.sprite = _selectedSprite;
    }

    public void Deselect()
    {
        image.sprite = _normalSprite;
    }
}
