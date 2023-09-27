namespace Main.BehaviourInterop.Runnable
{
    public enum RunnableStatus
    {
        Unknown = 0x01,
        Running = 0x02,
        Runned = 0x04,
        Pausing = 0x08,
        Paused = 0x10,
        Stopping = 0x20,
        Stopped = 0x40
    }
}
