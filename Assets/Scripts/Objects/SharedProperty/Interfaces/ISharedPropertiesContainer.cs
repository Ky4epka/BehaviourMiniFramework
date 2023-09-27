using UnityEngine.Events;
using System.Collections.Generic;
using Main.Events;
using System;
using UnityEngine;

namespace Main.Objects
{
    public interface ISharedPropertiesContainer: ISerializationCallbackReceiver
    {
        ISharedProperty SharedProperty(Type sharedPropType);
        T SharedProperty<T>() where T: ISharedProperty;
        IReadOnlyCollection<ISharedProperty> PropertyCollection { get; }

        bool HasProperty(Type propType);
        bool HasProperty<T>() where T: ISharedProperty;

        bool DeleteSharedProperty(Type propType);
        bool DeleteSharedProperty<T>() where T : ISharedProperty;
        void ClearSharedProperties();
        void MergeSharedProperties(ISharedPropertiesContainer source);
        void AssignSharedProperties(ISharedPropertiesContainer source);

        void SanitizeSerializedProperties();
    }
}