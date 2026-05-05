using System;
using UnityEngine;


namespace SurvivorsLike.UI
{
    public enum DialogType
    {
        Alert,   //확인 버튼이 있는 단순 알림 타입
        Confirm  //확인 + 취소 같은 사용자 선택 타입
    }

    public class DialogConfig
    {
        public string Title { get; set; } = "알림";
        public string Message { get; set; } = string.Empty;        
        public DialogType Type { get; set; } = DialogType.Alert;
        
        public string ConfirmText { get; set; } = "확인";
        public string CancelText { get; set; } = "취소";

        public Sprite IconSprite { get; set; } = null;
        public Color DialogColor = Color.white;

        //확인 버튼 콜백 액션
        public Action OnConfirm { get; set; }
        //취소 버튼 콜백 액션
        public Action OnCancel { get; set; }
    }

}
