using NUnit.Framework;
using SurvivorsLike;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    private SkillBase[] _skills;

    private void Awake()
    {
        _skills = GetComponents<SkillBase>();
    }

    public void UseAllSkill()
    {
        foreach(SkillBase skill in _skills)
        {
            skill.UseSkill();
        }
    }

    public void UseSkill(int skillId)
    {

    }
}
