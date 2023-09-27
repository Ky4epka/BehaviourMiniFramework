namespace Main.BehaviourInterop.Runnable
{
    public interface IState
    {
        bool Prepare(IStateContext context);
        bool Handle(IStateContext context);
    }
}
