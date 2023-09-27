using System;
using Main.Events;
using UnityEngine;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {
        /// <summary>
        /// ...
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class EnabledWhileAliveAttribute : BaseBehaviourAttribute, ILifecycleBehaviourAttribute
        {
            protected IObjectBehavioursBase Listener = null;

            public override bool Handle(Type pivot_type, IObjectBehavioursBase source, IObjectBehavioursBase target, BehaviourBinaryAttributeHandleDirection direction)
            {
                Listener = source;

                switch (direction)
                {
                    case BehaviourBinaryAttributeHandleDirection.One:
                        target.enabled = true;
                        break;
                    case BehaviourBinaryAttributeHandleDirection.Zero:
                        target.enabled = false;
                        break;
                }

                return true;
            }
        }
    }

}