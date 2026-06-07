using R3;
using System;
using System.Collections.Generic;
using UnityEngine;



namespace SurvivorsLike
{
    public class SkillController : MonoBehaviour
    {
        //8개의 스킬 슬롯
        private const int MaxSkillSlot = 8;

        private readonly List<SkillBase> _skillList = new List<SkillBase>(MaxSkillSlot);

        private bool _isPlaying;

        private readonly CompositeDisposable _disposables = new();

        private void Awake()
        {
            _isPlaying = false;
        }

        private void Start()
        {
            InGameEventBus.OnInGameStart
                .Take(1)
                .Subscribe(_ => OnGameStart())
                .AddTo(_disposables);
        }

        public void Init()
        {
            _skillList.Clear();
        }

        public bool AddSkill(SkillDataSO data)
        {
            if (_skillList.Count >= MaxSkillSlot)
                return false;

            SkillBase skill = SkillFactory.Create(data);
            skill.Init(data);
            _skillList.Add(skill);

            return true;
        }

        public void UpgradeSkill(int skillId, int newLevel)
        {
            int count = _skillList.Count;
            for (int ii = 0; ii < count; ++ii)
            {
                if(_skillList[ii].SkillId == skillId)
                {
                    _skillList[ii].SetLevel(newLevel);
                    return;
                }
            }
        }

        private void Update()
        {
            if (_isPlaying == false)
                return;

            float dt = Time.deltaTime;
            int count = _skillList.Count;
            for(int ii = 0; ii < count; ++ii)
            {
                _skillList[ii].Tick(dt);
                _skillList[ii].TryUseSkill();
            }
        }

        void OnGameStart()
        {
            _isPlaying = true;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
