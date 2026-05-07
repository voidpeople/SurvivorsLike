using System;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivorsLike
{
    //MVP 패턴 - View

    public class LobbyTabView : MonoBehaviour
    {
        [Header("탭 버튼")]
        [SerializeField] private ToggleGroup _tabToggleGroup;
        [SerializeField] private Toggle[] _tabToggles;

        [Header("탭 버튼 패널")]
        [SerializeField] private GameObject[] _tabPanels;

        //Presenter가 OnTabClicked을 통해 뷰의 변화를 통보 받는다.
        public event Action<LobbyTabType> OnTabClicked;

        public void Init()
        {
            for (int ii = 0; ii < _tabToggles.Length; ++ii)
            {
                int tabIndex = ii;
                _tabToggles[ii].onValueChanged.AddListener(inOn =>
                {
                    //토글이 꺼지는 건 무시~
                    if (inOn == false)
                        return;

                    //OnTabClicked을 구독하는 Presenter에게 현재 선택 된
                    //탭 버튼을 통보해 준다.
                    OnTabClicked?.Invoke((LobbyTabType)tabIndex);
                });
            }
        }

        public void SetTabSelect(LobbyTabType tabType)
        {
            int tabIndex = (int)tabType;

            //SetIsOnWithoutNotify()함수는 통보 없이 해당 토글을 선택하게 해 준다.
            _tabToggles[tabIndex].SetIsOnWithoutNotify(true);

            //현재 선택된 탭 버튼으 패널만 활성화 하고 나머지는 비 활성화
            UpdateTabPanels(tabIndex);
        }

        //현재 선택된 탭 버튼으 패널만 활성화 하고 나머지는 비 활성화
        public void UpdateTabPanels(int tabIndex)
        {
            for (int ii = 0; ii < _tabPanels.Length; ii++)
            {
                _tabPanels[ii].SetActive(ii == tabIndex);
            }
        }

        public void Destroy()
        {
            foreach (var toggle in _tabToggles)
                toggle?.onValueChanged.RemoveAllListeners();
        }
    }
}
