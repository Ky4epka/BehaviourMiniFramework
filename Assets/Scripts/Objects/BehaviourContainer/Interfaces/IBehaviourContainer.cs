using Main.Events;
using Main.Objects.Behaviours;
using Main.Managers;
using System.Runtime.InteropServices;

namespace Main.Objects
{

    public interface IBehaviourContainer: 
        IEventProcessor,
        IComponent, 
        IBehaviour, 
        ISharedPropertiesContainer,
        IManagedObject,
        IAssignable
        
    {
        ISharedPropertiesContainer SharedPropertyContainer { get; }

        void OnBehaviourAdd(IObjectBehavioursBase behaviour);
        void OnBehaviourDestroy(IObjectBehavioursBase behaviour);
    }

}
