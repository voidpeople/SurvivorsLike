namespace SurvivorsLike
{
    public class BattlePanelModel
    {
        public const string GameSceneName = "04_InGame";

        public bool CanStart { get; private set; } = true;

        public void SetCanStart(bool canStart) => CanStart = canStart;
    }
}
