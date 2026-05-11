using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace SurvivorsLike.UI.Lobby
{
    public class StageScrollController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _btnPrev;
        [SerializeField] private Button _btnNext;
        [SerializeField] private StageCardView _cardPrefab;

        private int _stageCount;
        private int _currentIndex;
        private float[] _snapPositions;
        private float _dragStartNormPos;
        private bool _isSnapping;
        private bool _isSetupDone;
        private const float SnapDuration = 0.25f;

        public event Action<int> OnCardCentered;

        private void Start()
        {
            _btnPrev.onClick.AddListener(PrevStage);
            _btnNext.onClick.AddListener(NextStage);

            // SetupStages 호출 전이면 Content의 현재 자식 수 기준으로 초기화 (정적 카드 폴백)
            if (!_isSetupDone)
            {
                _stageCount = _scrollRect.content.childCount;

                // 카드 수에 맞게 Content 너비 재계산 (수동으로 카드를 추가/삭제해도 올바르게 동작)
                Canvas.ForceUpdateCanvases();
                float cardWidth = _scrollRect.viewport.rect.width;
                var sd = _scrollRect.content.sizeDelta;
                _scrollRect.content.sizeDelta = new Vector2(_stageCount * cardWidth, sd.y);

                BuildSnapPositions();
                SnapTo(0, instant: true);
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        //동적 카드 추가
        public void SetupStages(ChapterDataSO[] chapters)
        {
            // 기존 카드 제거 (SetParent(null)로 즉시 분리 후 Destroy)
            for (int i = _scrollRect.content.childCount - 1; i >= 0; i--)
            {
                var child = _scrollRect.content.GetChild(i);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            _stageCount = chapters.Length;
            _currentIndex = 0;

            for (int i = 0; i < chapters.Length; i++)
            {
                var card = Instantiate(_cardPrefab, _scrollRect.content);
                card.Setup(chapters[i], i);
            }

            // Content 너비를 카드 수 × Viewport 너비로 설정
            Canvas.ForceUpdateCanvases();
            float cardWidth = _scrollRect.viewport.rect.width;
            var sd = _scrollRect.content.sizeDelta;
            _scrollRect.content.sizeDelta = new Vector2(_stageCount * cardWidth, sd.y);

            DOTween.Kill(this);
            _isSetupDone = true;
            BuildSnapPositions();
            SnapTo(0, instant: true);
        }

        private void BuildSnapPositions()
        {
            _snapPositions = new float[_stageCount];
            if (_stageCount <= 1) return;
            for (int i = 0; i < _stageCount; i++)
                _snapPositions[i] = (float)i / (_stageCount - 1);
        }

        //드래그 시작할 떄 호출됨~
        public void OnBeginDrag(PointerEventData eventData)
        {
            _isSnapping = false;
            DOTween.Kill(this);
            _dragStartNormPos = _scrollRect.horizontalNormalizedPosition;
        }

        //드래그가 끝나면 호출됨~
        public void OnEndDrag(PointerEventData eventData)
        {
            float delta = _scrollRect.horizontalNormalizedPosition - _dragStartNormPos;
            float threshold = _stageCount > 1 ? 0.5f / (_stageCount - 1) : 1f;

            if (delta > threshold && _currentIndex < _stageCount - 1)
                _currentIndex++;
            else if (delta < -threshold && _currentIndex > 0)
                _currentIndex--;

            _scrollRect.StopMovement();
            SnapTo(_currentIndex);
        }

        //이전 스테이지
        public void PrevStage()
        {
            if (_currentIndex <= 0) return;
            _currentIndex--;
            SnapTo(_currentIndex);
        }

        //다음 스테이지
        public void NextStage()
        {
            if (_currentIndex >= _stageCount - 1) return;
            _currentIndex++;
            SnapTo(_currentIndex);
        }

        public int GetCurrentStageIndex() => _currentIndex;

        //내부 스냅~
        private void SnapTo(int index, bool instant = false)
        {
            if (_stageCount == 0) return;
            float targetPos = _stageCount > 1 ? _snapPositions[index] : 0f;

            if (instant)
            {
                _scrollRect.horizontalNormalizedPosition = targetPos;
                _isSnapping = false;
                OnCardCentered?.Invoke(index);
            }
            else
            {
                _isSnapping = true;
                DOTween.To(
                    () => _scrollRect.horizontalNormalizedPosition,
                    x => _scrollRect.horizontalNormalizedPosition = x,
                    targetPos,
                    SnapDuration
                ).SetEase(Ease.OutCubic)
                 .SetId(this)
                 .OnComplete(() =>
                 {
                     _isSnapping = false;
                     OnCardCentered?.Invoke(index);
                 });
            }

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            if (_btnPrev != null) _btnPrev.interactable = _currentIndex > 0;
            if (_btnNext != null) _btnNext.interactable = _currentIndex < _stageCount - 1;
        }
    }
}
