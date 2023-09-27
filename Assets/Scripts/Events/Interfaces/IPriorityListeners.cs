namespace Main.Events
{
    public interface IPriorityListeners<DELEGATE>
    {
        void AddListener(DELEGATE listener, ListenerPriority prio);
        void RemoveListener(DELEGATE listener);

        void Clear();

    }

}