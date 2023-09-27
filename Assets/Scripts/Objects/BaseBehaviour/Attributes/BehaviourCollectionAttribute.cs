using System;

namespace Main.Objects.Behaviours
{
    namespace Attributes
    {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class BehaviourCollectionAttribute : BaseBehaviourAttribute, IEnableDependentBehaviourAttribute
        {
            public Type[] Behaviours
            {
                get => iBehaviours;
                set
                {
                    foreach (Type beh_type in value)
                    {
                        if (!typeof(IObjectBehavioursBase).IsAssignableFrom(beh_type)) throw new TypeNotAssignableFromBehaviourBaseException(beh_type);
                    }

                    iBehaviours = value;
                }
            }

            protected Type[] iBehaviours;

            public BehaviourCollectionAttribute(Type behaviour) 
            {
                Behaviours = new Type[1] { behaviour };
            }

            public BehaviourCollectionAttribute(Type[] behaviours)
            {
                Behaviours = behaviours;
            }
        }
    }

}