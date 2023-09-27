using System;
using System.Reflection;
using Main.Events;

namespace Main.Objects.Behaviours
{
    public class ManagedEventAttributeDelegateInfo
    {
        public Delegate methodDelegate;
        public Attribute bindedAttribute;
        public Type sourceType;
        public object sourceInstance;
        public Type extractedEventType;

        public ManagedEventAttributeDelegateInfo(Delegate _methodDelegate, Attribute _bindedAttribute, Type _sourceType, object _sourceInstance)
        {
            methodDelegate = _methodDelegate;
            bindedAttribute = _bindedAttribute;
            sourceType = _sourceType;
            sourceInstance = _sourceInstance;
            ParameterInfo[] parameters = _methodDelegate.Method.GetParameters();

//            if (parameters.Length != 1)
  //              throw new Exception($"Managed event listener must have one parameter of type '{nameof(IEventData)}'");

            extractedEventType = parameters[0].ParameterType;

    //        if (!typeof(IEventData).IsAssignableFrom(extractedEventType))
      //          throw new Exception($"Managed event listener must be inherited from '{nameof(IEventData)}'");
        }
    }
}