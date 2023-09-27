namespace Main.Events
{
    using System;
    using Main.Objects;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    [RoutableType]
    public abstract class EventDataBase : IEventData, IEventData_Zeroable
    {
        public IEventProcessor Sender {
            get => iSender;
            set
            {
                if (iSenderSetFlag)
                    throw new Exception("Trying to change an already set sender value");

                iSenderSetFlag = true;
                iSender = value;
            }
        }

        public IEventProcessor Receiver
        {
            get => iReceiver;
            set
            {
                if (iReceiverSetFlag)
                    throw new Exception("Trying to change an already set receiver value");

                iReceiverSetFlag = true;
                iReceiver = value;
            }
        }

        public Type thisType => (ithisType != null) ? ithisType : ithisType = GetType();

        protected Type ithisType = null;
        protected IEventProcessor iSender = null;
        protected bool iSenderSetFlag = false;
        protected IEventProcessor iReceiver = null;
        protected bool iReceiverSetFlag = false;


        protected delegate EventDataBase ConstructorFunc();


        protected static ConstructorFunc BuildConstructor(Type eventDataType)
        {
            if (!typeof(EventDataBase).IsAssignableFrom(eventDataType))
                throw new TypeAccessException();


            ParameterExpression[] ctorParams = new ParameterExpression[0]
                {
                };
            NewExpression ctor = Expression.New(                
                eventDataType.GetConstructor( new Type[0] {  }),
                ctorParams
                );

            return (ConstructorFunc)Expression.Lambda
                (typeof(ConstructorFunc), ctor, ctorParams).Compile();
        }

        protected static Dictionary<Type, ConstructorFunc> iConstructors = new Dictionary<Type, ConstructorFunc>();

        public static EventDataBase InstanceConstructor(Type eventDatType, IEventProcessor sender, IEventProcessor receiver)
        {
            ConstructorFunc ctor;
            EventDataBase result;

            if (!iConstructors.TryGetValue(eventDatType, out ctor))
            {
                ctor = BuildConstructor(eventDatType);
                iConstructors.Add(eventDatType, ctor);
            }

            result = ctor();
            result.Sender = sender;
            result.Receiver = receiver;

            return result;
        }

        public virtual void Invoke()
        {
            EventDataBase.Invoke(this);
        }

        public virtual void InvokeFor(Delegate listener)
        {
            EventDataBase.InvokeFor(this, listener);
        }

        public static void Invoke(IEventData instance)
        {
            instance.Receiver.InvokeEvent(instance.thisType, instance);
        }

        public static void InvokeFor(IEventData instance, Delegate listener)
        {
            try
            {
                listener.Method.Invoke(listener.Target, new object[1] { instance });
            }
            catch (Exception e)
            {
                GLog.LogException(e);
            }
        }

        public virtual void Zero()
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void Dispose()
        {
        }

    }

}