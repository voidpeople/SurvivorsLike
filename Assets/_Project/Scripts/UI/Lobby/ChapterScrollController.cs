using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurvivorsLike
{
    public class ChapterScrollController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _btnPrev;
        [SerializeField] private Button _btnNext;
        [SerializeField] private ChapterCardView _cardPrefab;

        private int _chapterCount;
        private int _currentIndex;
        private float[] _snapPositions;
        private float _dragStartNormPos;
        private bool _isSnapping;
        private bool _isSetupDone;
        private const float SnapDuration = 0.25f;

        public event Action<int> OnCardCentered;

        private void Start()
        {
            _btnPrev.onClick.AddListener(PrevChapter);
            _btnNext.onClick.AddListener(NextChapter);

            // SetupChapters 호출 전이면 Content의 현재 자식 수 기준으로 초기화 (정적 카드 폴백)
            if (!_isSetupDone)
            {
                _chapterCount = _scrollRect.content.childCount;

                // 카드 수에 맞게 Content 너비 재계산 (수동으로 카드를 추가/삭제해도 올바르게 동작)
                Canvas.ForceUpdateCanvases();
                float cardWidth = _scrollRect.viewport.rect.width;
                var sd = _scrollRect.content.sizeDelta;
                _scrollRect.content.sizeDelta = new Vector2(_chapterCount * cardWidth, sd.y);

                BuildSnapPositions();
                SnapTo(0, instant: true);
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
            _btnPrev.onClick.RemoveAllListeners();
            _btnNext.onClick.RemoveAllListeners();
        }

        //동적 카드 추가
        public void SetupChapters(IReadOnlyList<ChapterDataSO> chapterList)
        {
            // 기존 카드 제거 (SetParent(null)로 즉시 분리 후 Destroy)
            for (int i = _scrollRect.content.childCount - 1; i >= 0; i--)
            {
                var child = _scrollRect.content.GetChild(i);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            _chapterCount = chapterList.Count;
            _currentIndex = 0;

            for (int i = 0; i < chapterList.Count; i++)
            {
                var card = Instantiate(_cardPrefab, _scrollRect.content);
                card.Setup(chapterList[i], i);
            }

            // Content 너비를 카드 수 × Viewport 너비로 설정
            Canvas.ForceUpdateCanvases();
            float cardWidth = _scrollRect.viewport.rect.width;
            var sd = _scrollRect.content.sizeDelta;
            _scrollRect.content.sizeDelta = new Vector2(_chapterCount * cardWidth, sd.y);

            DOTween.Kill(this);
            _isSetupDone = true;
            BuildSnapPositions();
            SnapTo(0, instant: true);
        }

        private void BuildSnapPositions()
        {
            _snapPositions = new float[_chapterCount];
            if (_chapterCount <= 1) return;
            for (int i = 0; i < _chapterCount; i++)
                _snapPositions[i] = (float)i / (_chapterCount - 1);
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
            float threshold = _chapterCount > 1 ? 0.5f / (_chapterCount - 1) : 1f;

            if (delta > threshold && _currentIndex < _chapterCount - 1)
                _currentIndex++;
            else if (delta < -threshold && _currentIndex > 0)
                _currentIndex--;

            _scrollRect.StopMovement();
            SnapTo(_currentIndex);
        }

        //이전 스테이지
        public void PrevChapter()
        {
            if (_currentIndex <= 0) return;
            _currentIndex--;
            SnapTo(_currentIndex);
        }

        //다음 스테이지
        public void NextChapter()
        {
            if (_currentIndex >= _chapterCount - 1) return;
            _currentIndex++;
            SnapTo(_currentIndex);
        }

        public int GetCurrentChapterIndex() => _currentIndex;

        public void ScrollToIndex(int index, bool instant = false)
        {
            if (index < 0 || index >= _chapterCount) return;
            _currentIndex = index;
            SnapTo(index, instant);
        }

        //내부 스냅~
        private void SnapTo(int index, bool instant = false)
        {
            if (_chapterCount == 0) return;
            float targetPos = _chapterCount > 1 ? _snapPositions[index] : 0f;

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
            if (_btnNext != null) _btnNext.interactable = _currentIndex < _chapterCount - 1;
        }
    }
}
