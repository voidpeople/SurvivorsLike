using SurvivorsLike;
using UnityEngine;



/*
Player(루트)
    ├─ PlayerController.cs     ← 이동, 입력, 상태
    ├─ PlayerStats.cs          ← HP, 스탯
    ├─ CharacterController     ← 물리 이동
    │
    └─ RobotKyle (자식)        ← 순수 비주얼만 담당
         ├─ Animator
         └─ SkinnedMeshRenderer
*/

namespace SurvivorsLike
{
    public class PlayerController : MonoBehaviour
    {

    }
}
