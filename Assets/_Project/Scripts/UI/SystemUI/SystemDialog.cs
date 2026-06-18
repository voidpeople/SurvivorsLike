using System;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    public class SystemDialog : MonoBehaviour
    {
        [Title("TopBar")]
        [SerializeField] private Image _topBarImage;
        [SerializeField] private Image _iconImage;

        [Title("Messages")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;

        [Title("Buttons")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Image _confirmButtonImage;
        [SerializeField] private TextMeshProUGUI _confirmButtonText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Image _canceButtonImage;
        [SerializeField] private TextMeshProUGUI _cancelButtonText;

        private SystemUIConfigSO _systemUIConfigSO;

        public DialogConfig Config { get; private set; }

        public void Init(SystemUIConfigSO systemUIConfigSO)
        {
            Debug.Assert(systemUIConfigSO != null, $"{nameof(SystemDialog)}::Init=> systemUIConfigSO is null");

            _systemUIConfigSO = systemUIConfigSO;
        }

        public void Show(DialogConfig config, Action onClose = null)
        {
            Config = config;

            _titleText.text = config.Title;
            _messageText.text = config.Message;
            _confirmButtonText.text = config.ConfirmText;

            bool isDual = (_systemUIConfigSO.GetButtonType(config.Type) == DialogButtonType.DualButton);
            _cancelButton.gameObject.SetActive(isDual);
            if (isDual)
                _cancelButtonText.text = config.CancelText;

            _iconImage.sprite = _systemUIConfigSO.GetIcon(config.Type);
            ApplyColor(_systemUIConfigSO.GetColor(config.Type));
            BindButtons(config, onClose);
        }

        private void ApplyColor(Color color)
        {
            _topBarImage.color = color;
            _confirmButtonImage.color = color;
            _canceButtonImage.color = color;
        }

        private void BindButtons(DialogConfig config, Action onClose)
        {
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            _confirmButton.onClick.AddListener(() =>
            {
                onClose?.Invoke();
                config.OnConfirm?.Invoke();
            });

            _cancelButton.onClick.AddListener(() =>
            {
                onClose?.Invoke();
                config.OnCancel?.Invoke();
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
