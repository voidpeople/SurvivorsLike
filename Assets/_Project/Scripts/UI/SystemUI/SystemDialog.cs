using DG.Tweening;
using SurvivorsLike.UI;
using System;
using TMPro;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.STP;

namespace SurvivorsLike.UI
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

        public DialogConfig Config { get; protected set; }
        private Action _onClose;

        public void Init(SystemUIConfigSO systemUIConfigSO)
        {
            _systemUIConfigSO = systemUIConfigSO;
        }

        public void Show(DialogConfig config, Action onClose = null)
        {
            Config = config;

            _titleText.text = config.Title;
            _messageText.text = config.Message;

            _confirmButtonText.text = config.ConfirmText;

            bool isConfirm = (config.Type == DialogType.Confirm);
            _cancelButton.gameObject.SetActive(isConfirm);

            if (isConfirm)
                _cancelButtonText.text = config.CancelText;

            _iconImage.sprite = _systemUIConfigSO.GetDialogIcon(config.Type);

            SetColor(_systemUIConfigSO.GetDialogColor(config.Type));
            InitButtons(config, onClose);
        }

        private void SetColor(Color color)
        {
            _topBarImage.color = color;
            _confirmButtonImage.color = color;
            _canceButtonImage.color = color;
        }

        private void InitButtons(DialogConfig config, Action onClose)
        {
            _onClose = onClose;

            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            _confirmButton.onClick.AddListener(() =>
            {
                config.OnConfirm?.Invoke();
                _onClose?.Invoke();
            });

            _cancelButton.onClick.AddListener(() =>
            {
                config.OnCancel?.Invoke();
                _onClose?.Invoke();
            });
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
