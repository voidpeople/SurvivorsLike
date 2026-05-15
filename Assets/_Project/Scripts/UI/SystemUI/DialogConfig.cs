using System;
using UnityEngine;


namespace SurvivorsLike
{
    public enum DialogType
    {
        AuthError,        // 인증 오류
        NetworkError,     // 네트워크 오류
        Maintenance,      // 서버 점검
        SessionExpired,   // 세션 만료, 중복 로그인
        ForceUpdate,      // 강제 업데이트
        Notice,           // 공지 사항
        Alert,            // 단순 알림
        Confirm,          // 사용자 의사결정
        CriticalError     // 예기치 못한 치명적 오류
    }

    public enum DialogButtonType
    {
        SingleButton,   // 확인 1개   - 단순 알림, 점검, 공지
        DualButton,     // 확인+취소  - 의사결정, 네트워크 재시도, 강제업데이트
    }

    public enum DialogSeverity
    {
        Info,       // 파란색 - 일반 정보, 공지, 점검 안내
        Success,    // 초록색 - 완료, 보상 수령, 구매 성공
        Warning,    // 주황색 - 주의 필요, 강제업데이트, 결제 확인
        Error,      // 빨간색 - 오류, 인증 실패, 네트워크 단절, 세션 만료
    }


    public class DialogConfig
    {
        public DialogType Type { get; private set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public string ConfirmText { get; set; }
        public string CancelText { get; set; }

        public Action OnConfirm { get; set; }
        public Action OnCancel { get; set; }

        // 시스템 타입 팩토리 메서드 - Type/버튼구성/색상이 고정, 콜백만 주입
        public static DialogConfig NetworkError(Action onRetry, Action onQuit) => new()
        {
            Type        = DialogType.NetworkError,
            Title       = "네트워크 오류",
            Message     = "서버와의 연결이 끊어졌습니다.\n잠시 후 다시 시도해주세요.",
            ConfirmText = "재시도",
            CancelText  = "종료",
            OnConfirm   = onRetry,
            OnCancel    = onQuit
        };

        public static DialogConfig AuthError(Action onRetry, Action onQuit) => new()
        {
            Type        = DialogType.AuthError,
            Title       = "인증 오류",
            Message     = "인증에 실패했습니다.\n다시 시도해주세요.",
            ConfirmText = "재시도",
            CancelText  = "종료",
            OnConfirm   = onRetry,
            OnCancel    = onQuit
        };

        public static DialogConfig Maintenance(string message) => new()
        {
            Type        = DialogType.Maintenance,
            Title       = "서버 점검",
            Message     = message,
            ConfirmText = "확인",
            OnConfirm   = () => Application.Quit()
        };

        public static DialogConfig SessionExpired(Action onConfirm) => new()
        {
            Type        = DialogType.SessionExpired,
            Title       = "세션 만료",
            Message     = "다른 기기에서 로그인되었습니다.\n게임을 재시작합니다.",
            ConfirmText = "확인",
            OnConfirm   = onConfirm
        };

        public static DialogConfig ForceUpdate(Action onUpdate) => new()
        {
            Type        = DialogType.ForceUpdate,
            Title       = "업데이트 필요",
            Message     = "최신 버전으로 업데이트가 필요합니다.",
            ConfirmText = "업데이트",
            CancelText  = "종료",
            OnConfirm   = onUpdate,
            OnCancel    = () => Application.Quit()
        };

        public static DialogConfig Notice(string title, string message, Action onConfirm = null) => new()
        {
            Type        = DialogType.Notice,
            Title       = title,
            Message     = message,
            ConfirmText = "확인",
            CancelText  = "닫기",
            OnConfirm   = onConfirm
        };

        public static DialogConfig CriticalError(string message = "예기치 못한 오류가 발생했습니다.\n앱을 재시작해 주세요.") => new()
        {
            Type        = DialogType.CriticalError,
            Title       = "오류",
            Message     = message,
            ConfirmText = "확인",
            OnConfirm   = () => Application.Quit()
        };

        // 범용 타입 팩토리 메서드 - 내용과 콜백을 호출부에서 주입
        public static DialogConfig Alert(string title, string message,
            string confirmText = "확인", Action onConfirm = null) => new()
        {
            Type        = DialogType.Alert,
            Title       = title,
            Message     = message,
            ConfirmText = confirmText,
            OnConfirm   = onConfirm
        };

        public static DialogConfig Confirm(string title, string message,
            string confirmText = "확인", string cancelText = "취소",
            Action onConfirm = null, Action onCancel = null) => new()
        {
            Type        = DialogType.Confirm,
            Title       = title,
            Message     = message,
            ConfirmText = confirmText,
            CancelText  = cancelText,
            OnConfirm   = onConfirm,
            OnCancel    = onCancel
        };
    }

}
