using System;
using Main.Events;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {
        /// <summary>
        /// ...
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class DisableOnDieAttribute : BaseBehaviourAttribute, ILifecycleBehaviourAttribute
        {
            protected IObjectBehavioursBase Listener = null;

            public override bool Handle(Type pivot_type, IObjectBehavioursBase source, IObjectBehavioursBase target, BehaviourBinaryAttributeHandleDirection direction)
            {
                Listener = source;

                switch (direction)
                {
                    case BehaviourBinaryAttributeHandleDirection.One:
                        target.enabled = false;
                        break;
                    case BehaviourBinaryAttributeHandleDirection.Zero:
                        break;
                }

                return true;
            }
        }
    }

}