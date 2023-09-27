using System;
using Main.Events;
using Main.Objects.Behaviours;
using Main.Managers;
using Main.Other;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Main.Aggregator.Events.Tools
{
}

namespace Main.Objects
{
    [DefaultExecutionOrder(KnownExecutionOrder.BehaviourContainerOrder)]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class BehaviourContainer : CachedMonoBehaviour, IBehaviourContainer, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Object ring
        /// </summary>
        public virtual IObjectManager MasterManager
        {
            get => iManager;

            set
            {
                if (iManager == value) return;

                if (iManager != null)
                {
                    iManager.Remove(this);
                    EventBus.RemoveEventListener(iManager);
                }

                iManager = value;

                if (iManager != null)
                {
                    iManager.Add(this);
                    EventBus.AddEventListener(iManager);
                }
            }
        }

        [NonSerialized]
        protected static Dictionary<Type, TypeReflector> iTypeReflectors = new Dictionary<Type, TypeReflector>();

        [NonSerialized]
        protected TypeReflector iTypeReflector = null;
        [NonSerialized]
        protected IObjectManager iManager = null;
        [NonSerialized]
        protected IEventProcessor iEventBus_ = null;

        protected IEventProcessor EventBus => (iEventBus_ ?? (iEventBus_ = new EventProcessor()));

        [SerializeField]
        protected SharedPropertiesContainer iSharedPropertyContainer;

        public ISharedPropertiesContainer SharedPropertyContainer
        {
            get
            {
                // Late binding container prop correction
                if (iSharedPropertyContainer != null)
                    (iSharedPropertyContainer as SharedPropertiesContainer).Container = this;
                else
                    iSharedPropertyContainer = new SharedPropertiesContainer(this);

                return iSharedPropertyContainer;
            }
        }
        public void AddEventListener(Type event_id, Delegate listener) => EventBus.AddEventListener(event_id, listener);
        public void RemoveEventListener(Type event_id, Delegate listener) => EventBus.RemoveEventListener(event_id, listener);
        public bool AddEventListener(IEventProcessor listener) => EventBus.AddEventListener(listener);
        public bool RemoveEventListener(IEventProcessor listener) => EventBus.RemoveEventListener(listener);
        public bool BindMutuallyListener(IEventProcessor listener) => EventBus.BindMutuallyListener(listener);
        public bool UnbindMutuallyListener(IEventProcessor listener) => EventBus.UnbindMutuallyListener(listener);


        public virtual void OnBehaviourAdd(IObjectBehavioursBase behaviour)
        {
        }

        public virtual void OnBehaviourDestroy(IObjectBehavioursBase behaviour)
        {

        }

        public virtual void OnBehaviourEnabledState(IObjectBehavioursBase behaviour)
        {
        }


        public void AddEventListener<T>(Action<T> listener) where T : IEventData => EventBus.AddEventListener<T>(listener);
        public void RemoveEventListener<T>(Action<T> listener) where T : IEventData => EventBus.RemoveEventListener<T>(listener);
 
        public T Event<T>(IEventProcessor sender) where T : IEventData => (T)Event(typeof(T), sender);
        public IEventData Event(Type eventType, IEventProcessor sender) => EventDataBase.InstanceConstructor(eventType, sender, this);

        public void InvokeEvent(Type key, IEventData eventData) => EventBus.InvokeEvent(key, eventData);
        public void InvokeEvent<T>(T eventData) where T : IEventData => EventBus.InvokeEvent<T>(eventData);
        public void ClearAllEventListeners() => EventBus.ClearAllEventListeners();

        public ISharedProperty SharedProperty(Type sharedPropType) => SharedPropertyContainer.SharedProperty(sharedPropType);
        public T SharedProperty<T>() where T : ISharedProperty => SharedPropertyContainer.SharedProperty<T>();

        public IReadOnlyCollection<ISharedProperty> PropertyCollection => SharedPropertyContainer.PropertyCollection;

        public void OnAfterDeserialize()
        {
            SharedPropertyContainer.OnAfterDeserialize();
        }

        public void OnBeforeSerialize()
        {
            SharedPropertyContainer.OnBeforeSerialize();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            SanitizeSerializedProperties();

            EventBus.RemoveEventListener<Aggregator.Events.Tools.DoBehaviourEnableStateEvent>(BehaviourEnableStateEvent);
            EventBus.AddEventListener<Aggregator.Events.Tools.DoBehaviourEnableStateEvent>(BehaviourEnableStateEvent);
        }

        public TypeReflector TypeReflector
        {
            get
            {
                if (iTypeReflector == null)
                {
                    Type thisType = GetType();

                    if (!iTypeReflectors.TryGetValue(thisType, out iTypeReflector))
                    {
                        iTypeReflector = new TypeReflector(thisType);
                        iTypeReflectors.Add(thisType, iTypeReflector);
                    }
                }

                return iTypeReflector;
            }
        }

        public void BehaviourEnableStateEvent(Aggregator.Events.Tools.DoBehaviourEnableStateEvent eventData)
        {
            IObjectBehavioursBase behaviour = GetComponent(eventData.BehaviourType) as IObjectBehavioursBase;

            if ((behaviour != null) && (eventData.State != behaviour.enabled))
            {
                behaviour.enabled = eventData.State;
            }           
        }

        public void DeleteComponentWithDependencies(Type componentType)
        {
            Component component = GetComponent(componentType);

            if (component == null)
                return;

            Component[] components = GetComponents<Component>();

            foreach (Component current in components)
            {
                IEnumerable<RequireComponent> requiredList = current.GetType().GetCustomAttributes<RequireComponent>(true);

                foreach (RequireComponent required in requiredList)
                {
                    if ((required.m_Type0 == componentType) ||
                        (required.m_Type1 == componentType) ||
                        (required.m_Type2 == componentType))
                    {
                        DeleteComponentWithDependencies(current.GetType());
                    }
                }
            }

            GameObject.DestroyImmediate(component);
        }

        public void ActualizeSharedProperties()
        {
            foreach (var beh in GetComponents<ObjectBehavioursBase>())
                beh.InjectSharedProperties();
        }

        protected override void Awake()
        {
            base.Awake();
            EventBus.AddEventListener<Aggregator.Events.Tools.DoBehaviourEnableStateEvent>(BehaviourEnableStateEvent);
            TypeReflector.InjectProperties(this, BindingFlags.Instance | BindingFlags.Public, true);
        }

        protected override void OnDestroy()
        {
            EventBus.RemoveEventListener<Aggregator.Events.Tools.DoBehaviourEnableStateEvent>(BehaviourEnableStateEvent);
            Event<Aggregator.Events.Tools.OnBehaviourContainerDestroyedEvent>(this).Invoke();
            base.OnDestroy();
        }

        public void MergeSharedProperties(ISharedPropertiesContainer source) => SharedPropertyContainer.MergeSharedProperties(source);

        public bool DeleteSharedProperty(Type propType) => SharedPropertyContainer.DeleteSharedProperty(propType);

        public bool DeleteSharedProperty<T>() where T : ISharedProperty => SharedPropertyContainer.DeleteSharedProperty<T>();

        public void Assign(IAssignable source)
        {
            if (!(source is IBehaviourContainer))
                return;

            IBehaviourContainer sourceCont = source as IBehaviourContainer;
            IObjectBehavioursBase[] thisBehaviours = GetComponents<IObjectBehavioursBase>();
            IObjectBehavioursBase[] sourceBehaviours = sourceCont.GetComponents<IObjectBehavioursBase>();

            foreach (IObjectBehavioursBase thisBehaviour in thisBehaviours)
            {
                if (sourceCont.GetComponent(thisBehaviour.cachedType) == null)
                    DeleteComponentWithDependencies(thisBehaviour.GetType());
            }

            foreach (IObjectBehavioursBase sourceBehaviour in sourceBehaviours)
            {
                if (GetComponent(sourceBehaviour.cachedType) == null)
                    gameObject.AddComponent(sourceBehaviour.cachedType);
            }

            AssignSharedProperties(sourceCont);
        }

        public void AssignSharedProperties(ISharedPropertiesContainer source) => SharedPropertyContainer.AssignSharedProperties(source);

        public bool HasProperty(Type propType) => SharedPropertyContainer.HasProperty(propType);

        public bool HasProperty<T>() where T : ISharedProperty => SharedPropertyContainer.HasProperty<T>();

        public void SanitizeSerializedProperties() => SharedPropertyContainer.SanitizeSerializedProperties();

        public void RebuildSharedProperties()
        {
            ClearSharedProperties();
            ActualizeSharedProperties();
        }

        public void ClearSharedProperties() => SharedPropertyContainer.ClearSharedProperties();
    }

}
