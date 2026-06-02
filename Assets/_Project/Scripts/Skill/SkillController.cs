using NUnit.Framework;
using SurvivorsLike;
using UnityEngine;


public class SkillController : MonoBehaviour
{
    //SkillDataSO 데이터로 부터 사용하는 스킬들에 대한
    //스킬 인스턴스를 생성하여 _skills에 추가한다.
    private SkillBase[] _skills;

    private void Awake()
    {

    }

    public void Init()
    {

    }

    public void UseAllSkill()
    {
        foreach(SkillBase skill in _skills)
        {
            skill.UseSkill();
        }
    }

    public void StopAllSkill()
    {
        foreach (SkillBase skill in _skills)
        {
            skill.StopSkill();
        }
    }

    public void UseSkill(int skillId)
    {

    }

    public void StopSkill(int skillId)
    {

    }

    private void Update()
    {
        //_skills
    }
}
