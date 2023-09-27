using System;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class ForceDisableDependentsAttribute : BaseBehaviourAttribute, IEnableDependentBehaviourAttribute
        {
            public override bool Handle(Type pivot_type, IObjectBehavioursBase source, IObjectBehavioursBase target, BehaviourBinaryAttributeHandleDirection direction)
            {
                if (direction == BehaviourBinaryAttributeHandleDirection.One)
                    return true;

                RequireEnabledBehaviourAttribute req_attr;

                if (target == null)
                    UnityEngine.Debug.Log("Target is null");

                foreach (IEnableDependentBehaviourAttribute attr in target.GetEnableDependentAttributes(BehaviourBinaryAttributeArea.Container))
                {
                    req_attr = attr as RequireEnabledBehaviourAttribute;

                    if (req_attr == null)
                        continue;

                    foreach (Type behaviour in (req_attr.Behaviours))
                    {
                        if (behaviour.Equals(pivot_type) || 
                            (attr.includeInheritedBehaviours && 
                             behaviour.IsAssignableFrom(pivot_type)))
                        {
                            target.enabled = false;
                            return true;
                        }
                    }
                }

                return true;
            }
        }
    }

}