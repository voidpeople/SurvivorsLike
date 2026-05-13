using System;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _selectedSprite;

    public event Action<TabButton> OnSelected;

    private bool _isSelected;

    private void Awake()
    {
        Deselect();
    }

    public void Select()
    {
        _isSelected = true;
        _image.sprite = _selectedSprite;
    }

    public void Deselect()
    {
        _isSelected = false;
        _image.sprite = _normalSprite;
    }

    // Button onClick에서 호출
    public void OnClick()
    {
        OnSelected?.Invoke(this);
    }
}
