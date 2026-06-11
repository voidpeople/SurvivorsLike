using UnityEngine;


namespace SurvivorsLike
{
    public interface ISkillOwner
    {
        Transform Transform { get; }
        Transform FirePoint { get; }   //없는 캐릭터는 null 반환 허용
    }
}
