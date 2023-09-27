using System;
using Main.Events;
using Main.Other;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public class EnabledStateEventAttribute : ManagedDelegateOfEventAttribute
        {
            public EnabledStateEventAttribute(): base()
            {
            }

        }
    }

}