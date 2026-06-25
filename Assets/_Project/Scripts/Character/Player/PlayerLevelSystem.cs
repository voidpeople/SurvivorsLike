using R3;
using UnityEngine;


namespace SurvivorsLike
{
    public class PlayerLevelSystem : MonoBehaviour
    {
        InGamePlayerLevelDataSO _levelData;

        private readonly ReactiveProperty<int> _level = new(1);       //현재 레벨
        private readonly ReactiveProperty<int> _currentExp = new(0);  //현재 경험치
        private readonly ReactiveProperty<int> _requiredExp = new(0); //다음 레벨업 경험치
        private readonly Subject<int> _onLevelUp = new();             //레벨업 이벤트

        public ReadOnlyReactiveProperty<int> Level => _level;
        public ReadOnlyReactiveProperty<int> CurrentExp => _currentExp;
        public ReadOnlyReactiveProperty<int> RequiredExp => _requiredExp;
        public Observable<int> OnLevelUp => _onLevelUp;


        public void Init(InGamePlayerLevelDataSO levelData)
        {
            _levelData = levelData;
            _requiredExp.Value = _levelData.GetRequiredExp(_level.Value);
        }

        private void OnDestroy()
        {
            _onLevelUp.Dispose();
            _level.Dispose();
            _currentExp.Dispose();
            _requiredExp.Dispose();
        }

        public void AddExp(int exp)
        {
            if (exp <= 0)
                return;
            if (_level.Value >= _levelData.MaxLevel)
                return; // 최대 레벨 차단

            _currentExp.Value += exp;

            //레벨업이 가능하다면 시도~
            TryLevelUp();
        }

        public void TryLevelUp()
        {
            while((_level.Value < _levelData.MaxLevel)
               && (_currentExp.Value >= _requiredExp.Value))
            {
                _currentExp.Value -= _requiredExp.Value;
                _requiredExp.Value = _levelData.GetRequiredExp(_level.Value);
                //레벨업 구독 이벤트 발행
                _onLevelUp.OnNext(_level.Value);
            }

            // 최대 레벨 도달 시 잔여 Exp 정리
            if (_level.Value >= _levelData.MaxLevel)
                _currentExp.Value = 0;
        }
    }
}
