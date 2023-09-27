using System;
using Main.Other;
using Main.Events;

namespace Main.Objects.Behaviours.Attributes
{
    public class EventDelegateData: ManagedMethodDataBase
    {

    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ManagedDelegateOfEventAttribute : MethodBaseAttribute, IDelegateActivableAttribute
    {
        public bool NotUseInEditMode { get; set; }

        public IManagedMethodData CreateData()
        {
            return new EventDelegateData();
        }
    }
}