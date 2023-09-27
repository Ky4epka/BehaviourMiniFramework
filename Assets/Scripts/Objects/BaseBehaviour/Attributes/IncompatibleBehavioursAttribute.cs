using System;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {

        /// <summary>
        /// This attribute determines a list of incompatibles behaviours with current behaviour
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        public class IncompatibleBehavioursAttribute : BehaviourCollectionAttribute, IEnableDependentBehaviourAttribute
        {
            public override BehaviourBinaryAttributeArea WorkingArea
            {
                get;
                set;
            } = BehaviourBinaryAttributeArea.Container;

            public override bool includeInheritedBehaviours
            {
                get;
                set;
            } = true;

            public bool ForceDisable
            {
                get;
                set;
            } = false;

            public IncompatibleBehavioursAttribute(params Type[] behaviours) : base(behaviours)
            {
            }

            public IncompatibleBehavioursAttribute(Type behaviour, bool deactivate_target = true, bool include_inhertied = true) : base(behaviour)
            {
                ForceDisable = deactivate_target;
                includeInheritedBehaviours = include_inhertied;
            }

            public override bool Handle(Type pivot_type, IObjectBehavioursBase source, IObjectBehavioursBase target, BehaviourBinaryAttributeHandleDirection direction)
            {
                if ((direction == BehaviourBinaryAttributeHandleDirection.Zero))
                    return true;

                foreach (Type beh_type in iBehaviours)
                {
                    if (beh_type.Equals(target.cachedType) || 
                            (includeInheritedBehaviours && 
                             beh_type.IsAssignableFrom(target.cachedType)))
                    {
                        if (!target.enabled)
                            continue;

                        if (ForceDisable)
                            target.enabled = false;
                        else
                            return false;
                    };
                }

                return true;
            }
        }
    }

}