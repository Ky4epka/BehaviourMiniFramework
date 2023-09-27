using System;
using System.Collections;
using System.Collections.Generic;

namespace Main.Objects
{
    using UnityEngine;

    [System.Serializable]
    public class SharedPropertiesContainer : ISharedPropertiesContainer
    {
        public IBehaviourContainer Container { get; set; }
        [NonSerialized]
        protected Dictionary<Type, ISharedProperty> iSharedProperties = null;
        /// <summary>
        /// This feature only for editor mode
        /// </summary>
        [SerializeReference]
        [HideInInspector]        
        public ISharedProperty[] SerializedPropValues = new ISharedProperty[0];

        protected Dictionary<Type, ISharedProperty> SharedProperties => 
            (iSharedProperties == null) ? 
                iSharedProperties = new Dictionary<Type, ISharedProperty>() : 
                iSharedProperties;

        public SharedPropertiesContainer()
        {
            Container = null;
        }

        public SharedPropertiesContainer(IBehaviourContainer container)
        {
            Container = container;
        }

        public ISharedProperty SharedProperty(Type sharedPropType)
        {
            ISharedProperty result;

            if (Type.Missing.Equals(sharedPropType))
            {
                Debug.Log($"Argument {nameof(sharedPropType)} has missing type.");
            }

            if (!SharedProperties.TryGetValue(sharedPropType, out result))
            {
                result = (ISharedProperty)Activator.CreateInstance(sharedPropType);
                result.Container = Container;
                result.EmitEvents = true;
                SharedProperties.Add(sharedPropType, result);
            }
            else if (Type.Missing.Equals(sharedPropType))
            {
                SharedProperties.Remove(sharedPropType);
            }
            else if (!object.ReferenceEquals(result.Container, Container))
                result.Container = Container;

            return result;
        }

        public void MergeSharedProperties(ISharedPropertiesContainer source)
        {
            foreach (ISharedProperty property in source.PropertyCollection)
            {
                if (!property.IsSerializable)
                    continue;

                SharedProperty(property.GetType()).Value = property.Value;
            }
        }

        public IReadOnlyCollection<ISharedProperty> PropertyCollection => SharedProperties.Values;

        public T SharedProperty<T>() where T : ISharedProperty => (T)SharedProperty(typeof(T));

        public bool DeleteSharedProperty(Type propType)
        {
            if (!HasProperty(propType))
                return false;

            SharedProperty(propType).Dispose();
            var result = SharedProperties.Remove(propType);
            return result;
        }

        public bool DeleteSharedProperty<T>() where T : ISharedProperty => DeleteSharedProperty(typeof(T));

        public void AssignSharedProperties(ISharedPropertiesContainer source)
        {
            List<ISharedProperty> thisPropList = new List<ISharedProperty>(PropertyCollection);

            foreach (ISharedProperty thisProperty in thisPropList)
            {
                if (!source.HasProperty(thisProperty.GetType()))
                    DeleteSharedProperty(thisProperty.GetType());
            }

            thisPropList.Clear();
            
            foreach (ISharedProperty sourceProperty in source.PropertyCollection)
            {
                SharedProperty(sourceProperty.GetType()).Value = sourceProperty.Value;
            }
        }

        public bool HasProperty(Type propType)
        {
            ISharedProperty result;
            SharedProperties.TryGetValue(propType, out result);
            return result != null;
        }

        public bool HasProperty<T>() where T : ISharedProperty
        {
            return HasProperty(typeof(T));
        }

        /// <summary>
        /// Validating and removing missing types properties
        /// </summary>
        public void SanitizeSerializedProperties()
        {
            List<Type> propTypes = new List<Type>(SharedProperties.Keys);

            foreach (var propType in propTypes)
            {
                if (!(propType is object))
                {
                    Debug.LogWarning($"Property '{propType.FullName}' has missing type. Deleted...");
                    DeleteSharedProperty(propType);
                }
                else if (!object.Equals(SharedProperty(propType).Container, Container))
                    SharedProperty(propType).Container = Container;
            }
            
            propTypes.Clear();
        }

        public void OnBeforeSerialize()
        {            
            int counter = 0;
            int serLength = 0;

            foreach (KeyValuePair<Type, ISharedProperty> keyvalue in SharedProperties)
            {
                if ((keyvalue.Value == null) ||
                    (!keyvalue.Value.IsSerializable) ||
                    !(keyvalue.Key is object))
                    continue;

                serLength++;
            }

            SerializedPropValues = new ISharedProperty[serLength];

            foreach (KeyValuePair<Type, ISharedProperty> keyvalue in SharedProperties)
            {
                if ((keyvalue.Value == null) ||
                    (!keyvalue.Value.IsSerializable) ||
                    !(keyvalue.Key is object))
                    continue;

                SerializedPropValues[counter] = keyvalue.Value as ISharedProperty;
                counter++;
            }
        }

        public void OnAfterDeserialize()
        {
            if (SerializedPropValues == null)
                return;

            foreach (ISharedProperty prop in SerializedPropValues)
            {
                if ((prop == null ||
                    !(prop.GetType() is object)))
                {
                    continue;
                }

                if (!prop.IsSerializable)
                    continue;

                ISharedProperty shared = SharedProperty(prop.GetType());

                try
                {
                    shared.BeginDisableEventsAndHandlers();

                    if ((typeof(UnityEngine.Object).IsAssignableFrom(prop.ValueType)) && (prop.Value == null))
                        shared.Value = null;
                    else
                        shared.Value = prop.Value;
                }
                finally
                {
                    shared.EndDisableEventsAndHanlders();
                }
            }
        }

        public void ClearSharedProperties()
        {
            List<Type> propKeys = new List<Type>(SharedProperties.Keys);

            foreach (var propKey in propKeys)
                DeleteSharedProperty(propKey);
        }
    }

}
