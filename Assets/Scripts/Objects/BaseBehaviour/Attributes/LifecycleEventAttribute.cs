using System;
using Main.Events;
using Main.Other;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {
        /// <summary>
        /// Event that bind in Awake method and unbind in OnDestroy method
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class LifecycleEventAttribute : ManagedDelegateOfEventAttribute
        {
        }
    }

}