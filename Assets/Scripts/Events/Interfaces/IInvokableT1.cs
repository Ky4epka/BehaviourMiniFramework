namespace Main.Events
{
    public interface IInvokable<in T0>
    {
        void Invoke(T0 param1);
    }

}