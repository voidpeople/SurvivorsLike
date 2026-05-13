

namespace SurvivorsLike
{
    public class BattlePanelModel
    {
        public const string GameSceneName = "04_InGame";

        //게임을 시작할 수 있는지 없는지는 이곳에서 검사로직 추가
        public bool CanStart { get; private set; } = true;

        public void SetCanStart(bool canStart) => CanStart = canStart;
    }
}
