using UnityEngine;


namespace SurvivorsLike
{
    //record은 클래스 타입의 한 종류로
    //생성자에서 받은 값을 절대 바꾸지 않고 그대로 전달하는,
    //컴파일러가 비교·출력까지 자동으로 만들어 주는 불변 데이터 묶음.
    public record SkillOptionData(
        int    SkillId,
        string SkillName,
        string IconName,
        string Description, //스킬에 대한 간단한 설명
        bool   IsUpgrade,   //이 값이 true이면 새로운 스킬이 아닌 기존 스킬을 업그레이드 하는 거임~
        int    NextLevel    //스킬의 다음 레벨
    );
}
