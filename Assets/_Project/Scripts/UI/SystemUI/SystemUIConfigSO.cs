using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{
    [CreateAssetMenu(fileName = "SystemUIDataSO", menuName = "SurvivorsLike/Data/SystemUIDataSO")]
    public class SystemUIConfigSO : ScriptableObject
    {
        [Header("Dialog Icons")]
        [SerializeField] private Sprite _infoIcon;
        [SerializeField] private Sprite _warningIcon;
        [SerializeField] private Sprite _errorIcon;

        [Header("Dialog Colors")]
        [SerializeField] private Color _infoColor;
        [SerializeField] private Color _warningColor;
        [SerializeField] private Color _errorColor;

        // DialogType → Severity 매핑 (아이콘/색상 결정)
        private static readonly Dictionary<DialogType, DialogSeverity> _severityMap = new()
        {
            { DialogType.AuthError,      DialogSeverity.Error   },
            { DialogType.NetworkError,   DialogSeverity.Error   },
            { DialogType.SessionExpired, DialogSeverity.Error   },
            { DialogType.ForceUpdate,    DialogSeverity.Warning },
            { DialogType.Confirm,        DialogSeverity.Warning },
            { DialogType.Maintenance,    DialogSeverity.Info    },
            { DialogType.Notice,         DialogSeverity.Info    },
            { DialogType.Alert,          DialogSeverity.Info    },
            { DialogType.CriticalError,  DialogSeverity.Error   },
        };

        // DialogType → ButtonType 매핑 (버튼 구성 결정)
        private static readonly Dictionary<DialogType, DialogButtonType> _buttonTypeMap = new()
        {
            { DialogType.AuthError,      DialogButtonType.DualButton   },
            { DialogType.NetworkError,   DialogButtonType.DualButton   },
            { DialogType.ForceUpdate,    DialogButtonType.DualButton   },
            { DialogType.Confirm,        DialogButtonType.DualButton   },
            { DialogType.Notice,         DialogButtonType.DualButton   },
            { DialogType.SessionExpired, DialogButtonType.SingleButton },
            { DialogType.Maintenance,    DialogButtonType.SingleButton },
            { DialogType.Alert,          DialogButtonType.SingleButton },
            { DialogType.CriticalError,  DialogButtonType.SingleButton },
        };

        public Sprite GetIcon(DialogType type)
            => _severityMap[type] switch
            {
                DialogSeverity.Error   => _errorIcon,
                DialogSeverity.Warning => _warningIcon,
                _                      => _infoIcon
            };

        public Color GetColor(DialogType type)
            => _severityMap[type] switch
            {
                DialogSeverity.Error   => _errorColor,
                DialogSeverity.Warning => _warningColor,
                _                      => _infoColor
            };

        public DialogButtonType GetButtonType(DialogType type)
            => _buttonTypeMap[type];
    }

}
