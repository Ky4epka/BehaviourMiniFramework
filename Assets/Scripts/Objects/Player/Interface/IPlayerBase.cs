using Main.Objects;

namespace Main.Player
{
    public interface IPlayerBase: IBehaviourContainer
    {
        CommonPlayerBehaviour Data { get; }
    }

}