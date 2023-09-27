namespace Main.BehaviourInterop.Runnable
{
    public interface IState<T> where T : IStateContext
    {
        bool Prepare(T context);
        bool Handle(T context);
    }
}
