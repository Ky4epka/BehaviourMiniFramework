using System;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class Unique : BaseBehaviourAttribute, IEnableDependentBehaviourAttribute
        {
            public bool ForceDisable
            {
                get;
                set;
            }

            public Unique(bool forceDisable = true, bool includeInherited = true, BehaviourBinaryAttributeArea area = BehaviourBinaryAttributeArea.Container): base()
            {
                ForceDisable = forceDisable;
                includeInheritedBehaviours = includeInherited;
                WorkingArea = area;
            }

            public override bool Handle(Type pivot_type, IObjectBehavioursBase source, IObjectBehavioursBase target, BehaviourBinaryAttributeHandleDirection direction)
            {
                if (direction == BehaviourBinaryAttributeHandleDirection.Zero)
                    return true;

                if ((source.Equals(target.cachedType) ||
                    (includeInheritedBehaviours &&
                     (source.cachedType.IsAssignableFrom(target.cachedType) ||
                     target.cachedType.IsAssignableFrom(source.cachedType)) &&
                     target.enabled)))
                {
                    if (ForceDisable)
                        target.enabled = false;
                    else
                        return false;
                };

                return true;
            }
        }
    }

}