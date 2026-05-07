using System.Collections.Generic;
using UnityEngine;


namespace SurvivorsLike
{

    [CreateAssetMenu(fileName = "SystemUIDataSO", menuName = "Scriptable Objects/SystemUIDataSO")]
    public class SystemUIConfigSO : ScriptableObject
    {
        [Header("Dialog Icons")]
        [SerializeField] private Sprite _DialogInfoIcon;
        [SerializeField] private Sprite _DialogErrorIcon;

        [Header("Dialog Colos")]
        [SerializeField] private Color _DialogInfoColor;
        [SerializeField] private Color _DialogErrorColor;

        private Dictionary<DialogType, Sprite> _DialogIconMap;
        private Dictionary<DialogType, Color> _DialogColorMap;

        public void Init()
        {
            _DialogIconMap = new Dictionary<DialogType, Sprite>
            {
                { DialogType.NetworkError, _DialogErrorIcon },
                { DialogType.Maintenance, _DialogInfoIcon },
                { DialogType.SessionExpired, _DialogErrorIcon },
                { DialogType.ForceUpdate, _DialogInfoIcon },
                { DialogType.Notice, _DialogInfoIcon },
                { DialogType.Alert, _DialogInfoIcon },
                { DialogType.Confirm, _DialogInfoIcon }
            };

            _DialogColorMap = new Dictionary<DialogType, Color>
            {
                { DialogType.NetworkError, _DialogErrorColor },
                { DialogType.Maintenance, _DialogInfoColor },
                { DialogType.SessionExpired, _DialogErrorColor },
                { DialogType.ForceUpdate, _DialogInfoColor },
                { DialogType.Notice, _DialogInfoColor },
                { DialogType.Alert, _DialogInfoColor },
                { DialogType.Confirm, _DialogInfoColor }
            };
        }

        public Sprite GetDialogIcon(DialogType type)
            => _DialogIconMap.TryGetValue(type, out var s) ? s : null;

        public Color GetDialogColor(DialogType type)
            => _DialogColorMap.TryGetValue(type, out var s) ? s : Color.gray;
    }

}
