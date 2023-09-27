namespace Main.BehaviourInterop.Runnable
{
    public interface IRunnable
    {
        bool Run();
        bool Pause();
        bool Stop();
        bool Reset();
    }
}
