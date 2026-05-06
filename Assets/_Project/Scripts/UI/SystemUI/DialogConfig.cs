using System;
using UnityEngine;


namespace SurvivorsLike.UI
{
    public enum DialogType
    {
        NetworkError, //네트워크 오류
        Maintenance, //서버 점검
        SessionExpired, //세션 만료, 중복 로그인
        ForceUpdate, //강제 업데이트
        Notice, //공지 사항
        Alert,   //확인 버튼이 있는 단순 알림
        Confirm  //확인 + 취소 같은 사용자 선택 (게임을 종료 하시겟습니까?)
    }

    public class DialogConfig
    {
        public string Title { get; set; } = "알림";
        public string Message { get; set; } = string.Empty;        
        public DialogType Type { get; set; } = DialogType.Alert;
        
        public string ConfirmText { get; set; } = "확인";
        public string CancelText { get; set; } = "취소";

        //확인 버튼 콜백 액션
        public Action OnConfirm { get; set; }
        //취소 버튼 콜백 액션
        public Action OnCancel { get; set; }
    }

}
