using System;
using System.Collections.Generic;
using Main.Events;
using Main.Managers;
using Main.Objects.Behaviours.Attributes;
using Main.Other;

namespace Main.Objects.Behaviours
{

    public interface IObjectBehavioursBase: IEventProcessor, IComponent, IBehaviour, ISharedPropertiesContainer, IAttributeDelegatesCollection, UnityEngine.ISerializationCallbackReceiver
    {
        Type cachedType { get; }
        TypeReflector TypeReflector { get; }
        IBehaviourContainer Container { get; }
        IObjectManager MasterManager { get; set; }
        bool IsManagedMethodsReadyToUse { get; }

        List<IBaseBehaviourBinaryAttribute> GetEnableDependentAttributes(BehaviourBinaryAttributeArea area);
        //bool HandleBehaviourAttributes<AttrType>(List<AttrType>[] attr_array, BehaviourBinaryAttributeHandleDirection direction) where AttrType : IBaseBehaviourBinaryAttribute;

    }
}