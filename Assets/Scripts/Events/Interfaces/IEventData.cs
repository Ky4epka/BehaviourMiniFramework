namespace Main.Events
{
    public interface IEventData: IInvokableNonParams, System.IDisposable
    {
        System.Type thisType { get; }
        /// <summary>
        /// This value sets only once time. This action need to avoid a constructors in inherited event.
        /// </summary>
        IEventProcessor Sender { get; set; }
        /// <summary>
        /// This value sets only once time. This action need to avoid a constructors in inherited event.
        /// </summary>
        IEventProcessor Receiver { get; set; }
        void InvokeFor(System.Delegate listener);
    }

}