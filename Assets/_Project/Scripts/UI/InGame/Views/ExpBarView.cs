using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    public class ExpBarView : MonoBehaviour
    {
        [SerializeField] private Image _expBar;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("채워지는 연출")]
        [SerializeField] private float _fillSpeed = 1.5f;        // 기본 속도(비율/초)
        [SerializeField] private float _catchUpPerLevel = 1.0f;  // 밀린 레벨당 가속 계수
        [SerializeField] private int _snapThreshold = 8;       // 이 이상 밀리면 연출 생략(스냅)

        private int _displayedLevel = 1;
        private int _targetLevel = 1;
        private float _displayedRatio;
        private float _targetRatio;

        private void Awake()
        {
            UpdateLevelText();
        }

        public void SetValues(int level, float ratio)
        {
            _targetLevel = level;
            _targetRatio = Mathf.Clamp01(ratio);

            if (_targetLevel - _displayedLevel >= _snapThreshold)
            {
                _displayedLevel = _targetLevel;
                _displayedRatio = _targetRatio;
                UpdateLevelText();
            }
        }

        private void Update()
        {
            if (_displayedLevel < _targetLevel)
            {
                _displayedRatio = Mathf.MoveTowards(_displayedRatio, 1f, _fillSpeed * Time.deltaTime);
                //Mathf.Epsilon - float가 표현할 수 있는 최소 값으로, "이보다 작으면 0으로 간주"하는 기준 상수.
                //if (_displayedRatio >= 1f - Mathf.Epsilon)은 float 연산 오차로 인해 정확히 1f에 도달하지 못하는 경우를 방어하기 위한 안전 코드
                if (_displayedRatio >= 1f - Mathf.Epsilon)
                {
                    _displayedLevel++;
                    _displayedRatio = 0f;
                    UpdateLevelText();
                }
            }
            else
            {
                //_displayedRatio와 _targetRatio의 값이 같으면 종료
                if (Mathf.Approximately(_displayedRatio, _targetRatio))
                    return;

                _displayedRatio = Mathf.MoveTowards(_displayedRatio, _targetRatio, _fillSpeed * Time.deltaTime);
            }
            _expBar.fillAmount = _displayedRatio;
        }

        private void UpdateLevelText()
        {
            if (_levelText != null)
                _levelText.SetText("LV.{0}", (float)_displayedLevel);
        }
    }
}
