using System;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {
        public enum BehaviourBinaryAttributeArea
        {
            Local = 0,
            Container
        }

        public enum BehaviourBinaryAttributeHandleDirection
        {
            Zero = 0,
            One
        }

        public interface IBaseBehaviourBinaryAttribute
        {
            BehaviourBinaryAttributeArea WorkingArea { get; set; }
            bool includeInheritedBehaviours { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pivot_type"></param>
            /// <param name="target"></param>
            /// <returns>If returns false then breaks the current action</returns>
            bool Handle(Type pivot_type, IObjectBehavioursBase source, IObjectBehavioursBase target, BehaviourBinaryAttributeHandleDirection direction);
        }

        /// <summary>
        /// Attributes that handling on awake/destroy events
        /// </summary>
        public interface ILifecycleBehaviourAttribute: IBaseBehaviourBinaryAttribute
        {
        }

        /// <summary>
        /// Attributes that handling on enable/disable events
        /// </summary>
        public interface IEnableDependentBehaviourAttribute : IBaseBehaviourBinaryAttribute
        {
        }
    }

}