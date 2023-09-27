namespace Main.BehaviourInterop.Runnable
{
    public interface IStateContext
    {
        bool SetState(IState state);

        bool HandleState();
    }
}
