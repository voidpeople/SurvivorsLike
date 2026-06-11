using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    //View은 결정을 하지 않는다. 보고만 한다.
    //결정은 Presenter에서 한다.
    public class ChapterSelectPanelView : MonoBehaviour
    {
        [Header("UI 참조")]
        [SerializeField] private TextMeshProUGUI _chapterNameText;
        [SerializeField] private ChapterScrollController _chapterScrollCtrl;
        [SerializeField] private Button _selectButton;   // 챕터 선택 확정 버튼
        [SerializeField] private Button _exitButton;     // 선택 없이 패널 닫기 버튼

        public event Action<int> OnFinishScrollChapter;
        public event Action OnSelectChapter;
        public event Action OnExitPanel;

        public void Init()
        {
            _chapterScrollCtrl.OnCardCentered += FinishScrollChapter;
            _selectButton.onClick.AddListener(() => OnSelectChapter?.Invoke());
            _exitButton.onClick.AddListener(() => OnExitPanel?.Invoke());
        }

        public void SetupChapterCards(IReadOnlyList<ChapterDataSO> chapterList)
        {
            _chapterScrollCtrl.SetupChapters(chapterList);
        }

        private void FinishScrollChapter(int index)
        {
            OnFinishScrollChapter?.Invoke(index);
        }

        public void SetChapterName(string name)
        {
            _chapterNameText.text = name;
        }

        public int GetCurrentChapterIndex()
        {
            return _chapterScrollCtrl.GetCurrentChapterIndex();
        }

        public void ScrollToChapter(int index)
        {
            _chapterScrollCtrl.ScrollToIndex(index, instant: true);
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public void Destroy()
        {
            _chapterScrollCtrl.OnCardCentered -= FinishScrollChapter;
            _selectButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }
    }
}
