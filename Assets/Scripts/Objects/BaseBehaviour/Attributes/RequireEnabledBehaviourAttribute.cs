using System;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {


        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
        public class RequireEnabledBehaviourAttribute : BehaviourCollectionAttribute, IEnableDependentBehaviourAttribute
        {
            public bool ForceEnable = false;
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

            public RequireEnabledBehaviourAttribute(Type behaviour, bool forceEnable = false, bool includeInherited = true, BehaviourBinaryAttributeArea area = BehaviourBinaryAttributeArea.Container) : base(behaviour)
            {
                ForceEnable = forceEnable;
                includeInheritedBehaviours = includeInherited;
                WorkingArea = area;
            }

            public RequireEnabledBehaviourAttribute(Type[] behaviours, bool forceEnable = false, bool includeInherited = true, BehaviourBinaryAttributeArea area = BehaviourBinaryAttributeArea.Container) : base(behaviours)
            {
                ForceEnable = forceEnable;
                includeInheritedBehaviours = includeInherited;
                WorkingArea = area;
            }

            public override bool Handle(Type pivot_type, IObjectBehavioursBase source, IObjectBehavioursBase target, BehaviourBinaryAttributeHandleDirection direction)
            {
                foreach (Type behaviour in Behaviours)
                {
                    if (target.cachedType.Equals(behaviour) ||
                        (includeInheritedBehaviours &&
                         target.cachedType.IsAssignableFrom(behaviour)))
                    {
                        switch (direction)
                        {
                            case BehaviourBinaryAttributeHandleDirection.One:
                                if (!target.enabled)
                                {
                                    if (ForceEnable)
                                        target.enabled = true;
                                    else
                                        return false;
                                }

                                break;
                            case BehaviourBinaryAttributeHandleDirection.Zero:
                                break;
                        }

                    }
                }

                return true;
            }

        }
    }

}