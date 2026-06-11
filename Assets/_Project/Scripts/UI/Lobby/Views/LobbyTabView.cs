using System;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    //MVP 패턴 - View

    public class LobbyTabView : MonoBehaviour
    {
        [Header("탭 UI")]
        [SerializeField] private TabButton[] _tabButtons;     // 탭 버튼 배열 — 인덱스가 LobbyTabType 값과 대응
        [SerializeField] private GameObject[] _tabPanels;     // 각 탭의 패널 — _tabButtons와 인덱스 동기화 필수

        //Presenter가 OnTabClicked을 통해 뷰의 변화를 통보 받는다.
        public event Action<LobbyTabType> OnTabClicked;

        public void Init()
        {
            for (int ii = 0; ii < _tabButtons.Length; ++ii)
            {
                int tabIndex = ii; // 클로저 캡처
                _tabButtons[ii].OnSelected += btn =>
                {
                    //OnTabClicked을 구독하는 Presenter에게 현재 선택 된 탭 버튼의 로비 탭 타입을 통보해 준다.
                    OnTabClicked?.Invoke((LobbyTabType)tabIndex);
                };
            }
        }
        
        public void SetTabSelect(LobbyTabType tabType)
        {
            int tabIndex = (int)tabType;
            for (int ii = 0; ii < _tabButtons.Length; ++ii)
            {
                _tabButtons[ii].Deselect();
            }
            _tabButtons[tabIndex].Select();

            //현재 선택된 탭 버튼으 패널만 활성화 하고 나머지는 비 활성화
            UpdateTabPanels(tabIndex);
        }

        //현재 선택된 탭 버튼으 패널만 활성화 하고 나머지는 비 활성화
        private void UpdateTabPanels(int tabIndex)
        {
            for (int ii = 0; ii < _tabPanels.Length; ii++)
            {
                _tabPanels[ii].SetActive(ii == tabIndex);
            }
        }

        public void Destroy()
        {
        }
    }
}
