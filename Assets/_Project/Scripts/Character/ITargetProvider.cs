using UnityEngine;


namespace SurvivorsLike
{
    public interface ITargetProvider
    {
        ITargetable GetNearest();                              //가장 가까운 적 한마리 반환
        int GetInRange(float radius, ITargetable[] targets);   //일정 영역안의 적들 반환
    }
}
