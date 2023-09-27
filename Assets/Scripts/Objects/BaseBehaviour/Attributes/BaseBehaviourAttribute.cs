using System;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        public class BaseBehaviourAttribute : Attribute, IBaseBehaviourBinaryAttribute
        {
            public virtual BehaviourBinaryAttributeArea WorkingArea
            {
                get;
                set;
            } = BehaviourBinaryAttributeArea.Local;

            public virtual bool includeInheritedBehaviours { get; set; }

            public virtual bool Handle(Type pivot_type, IObjectBehavioursBase source, IObjectBehavioursBase target, BehaviourBinaryAttributeHandleDirection direction)
            {
                return true;
            }
        }



    }

}