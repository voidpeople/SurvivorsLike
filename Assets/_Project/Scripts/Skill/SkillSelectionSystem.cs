using System.Collections.Generic;


namespace SurvivorsLike
{
    //SkillSelectionView에서 보여줄 3개의 스킬 정보를 랜덤하게 선택하여 제공
    public class SkillSelectionSystem
    {
        private static readonly System.Random _random = new();

        public static SkillOptionData[] GetOptions(
            IReadOnlyList<SkillBase> ownedSkills,
            IReadOnlyDictionary<int, SkillDataSO> allSkills,
            bool isSlotFull, //모든 스킬 슬롯이 채워졌다면 true
            int count = 3)
        {
            Dictionary<int, int> ownedSkillLevelMap = GetOwnedSkillLevelMap(ownedSkills);
            List<SkillOptionData> skillOptionDataList = GetSkillOptionDataList(allSkills, ownedSkillLevelMap, isSlotFull);
            if (skillOptionDataList.Count == 0)
            {
                return System.Array.Empty<SkillOptionData>();
            }
            //리스트 랜덤 섞기
            ShuffleDataList(skillOptionDataList);

            int resultCount = System.Math.Min(count, skillOptionDataList.Count);
            SkillOptionData[] result = new SkillOptionData[resultCount];
            for (int ii = 0; ii < resultCount; ++ii)
            {
                result[ii] = skillOptionDataList[ii];
            }

            return result;
        }

        //현재 소유한 스킬들 각각에 대한 현재 레벨이 담긴 Map 컨테이너 반환
        private static Dictionary<int, int> GetOwnedSkillLevelMap(IReadOnlyList<SkillBase> ownedSkills)
        {
            //Dictionary<스킬아이디, 현재 스킬레벨>
            Dictionary<int, int> ownedSkillLevelMap = new();
            for (int ii = 0; ii < ownedSkills.Count; ++ii)
            {
                ownedSkillLevelMap[ownedSkills[ii].SkillId] = ownedSkills[ii].CurrentLevel;
            }

            return ownedSkillLevelMap;
        }

        private static List<SkillOptionData> GetSkillOptionDataList(
            IReadOnlyDictionary<int, SkillDataSO> allSkills,
            Dictionary<int, int> ownedSkillLevelMap,
            bool isSlotFull)
        {
            List<SkillOptionData> skillCandidates = new();

            foreach (SkillDataSO skillData in allSkills.Values)
            {
                bool isOwned = ownedSkillLevelMap.TryGetValue(skillData.Id, out int currentLevel);

                if (isOwned && currentLevel < skillData.MaxLevel)
                    skillCandidates.Add(MakeUpgradeSkillOptionData(skillData, currentLevel));
                else if (!isOwned && !isSlotFull)
                    skillCandidates.Add(MakeNewSkillOptionData(skillData));
            }

            return skillCandidates;
        }

        //기존 보유 스킬의 업그레이드 옵션 데이터
        private static SkillOptionData MakeUpgradeSkillOptionData(SkillDataSO skillData, int currentLevel)
        {
            return new SkillOptionData(
                skillData.Id,
                skillData.DisplayName,
                skillData.IconKey,
                skillData.Description,
                IsUpgrade: true,
                NextLevel: currentLevel + 1);
        }

        //새로운 스킬 옵션 데이터
        private static SkillOptionData MakeNewSkillOptionData(SkillDataSO skillData)
        {
            return new SkillOptionData(
                skillData.Id,
                skillData.DisplayName,
                skillData.IconKey,
                skillData.Description,
                IsUpgrade: false,
                NextLevel: 1);
        }
        
        //스킬 옵션 데이터들을 랜덤 섞기
        private static void ShuffleDataList(List<SkillOptionData> dataList)
        {
            for (int ii = dataList.Count - 1; ii > 0; --ii)
            {
                int jj = _random.Next(ii + 1);

                SkillOptionData temp = dataList[ii];
                dataList[ii] = dataList[jj];
                dataList[jj] = temp;
            }
        }
    }
}
