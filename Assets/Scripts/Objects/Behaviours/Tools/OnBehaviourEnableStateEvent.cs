using System;
using Main.Events;
using Main.Objects.Behaviours;

namespace Main.Aggregator.Events.Tools
{

    public class OnBehaviourEnableStateEvent : EventDataBase
    {
        public Type BehaviourType { get; private set; }
        public bool State { get; private set; }

        public OnBehaviourEnableStateEvent() : base()
        {
        }

        public void Invoke(Type behaviourType, bool state)
        {
            if (!typeof(IObjectBehavioursBase).IsAssignableFrom(behaviourType))
                throw new TypeNotAssignableFromBehaviourBaseException(behaviourType);

            BehaviourType = behaviourType;
            State = state;
            base.Invoke();
        }
    }

    public class DoBehaviourEnableStateEvent: OnBehaviourEnableStateEvent
    {

    }
}
