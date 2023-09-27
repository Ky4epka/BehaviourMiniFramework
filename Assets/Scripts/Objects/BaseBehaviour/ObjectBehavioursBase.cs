using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Main.Events;
using System.Reflection;
using Main.Other;
using Main.Objects.Behaviours.Attributes;
using Main.Managers;

namespace Main.Objects
{
}

namespace Main.Other.Tools.Attributes
{
}

namespace Main.Objects.Behaviours
{

    public struct DelegateMeta
    {
        IDelegateActivableAttribute Attribute;
    }


    [DefaultExecutionOrder(KnownExecutionOrder.ObjectBehaviourOrder)]
    [RequireComponent(typeof(BehaviourContainer))]
    [ExecuteInEditMode]
    [Main.Other.Tools.Attributes.InitializableStaticClass]
    public class ObjectBehavioursBase : CachedMonoBehaviour, IObjectBehavioursBase
    {
        protected class ManagedMethodInfo
        {
            public MethodInfo methodInfo { get; set; }
            public IDelegateActivableAttribute methodAttribute { get; set; }

            public ManagedMethodInfo(MethodInfo minfo, IDelegateActivableAttribute attr)
            {
                methodInfo = minfo;
                methodAttribute = attr;
            }
        }

        public virtual IBehaviourContainer Container
        {
            get
            {
                if (iContainer == null)
                {
                    iContainer = GetComponent<IBehaviourContainer>();
                }

                return iContainer;
            }
        }

        public Type cachedType
        {
            get
            {
                if (icachedType == null) icachedType = GetType();

                return icachedType;
            }
        }

        public TypeReflector TypeReflector
        {            
            get 
            { 
                if (iTypeReflector == null)
                
                if (!iTypeReflectors.TryGetValue(cachedType, out iTypeReflector))
                {
                    iTypeReflector = new TypeReflector(cachedType);
                    iTypeReflectors.Add(cachedType, iTypeReflector);
                }


                return iTypeReflector;
            }
        }

        protected TypeReflector iTypeReflector = null;
        protected static Dictionary<Type, TypeReflector> iTypeReflectors { get; } = new Dictionary<Type, TypeReflector>();
        protected static Dictionary<Type, Dictionary<Type, List<ManagedMethodInfo>>> iManagedMethodsAttributes = new Dictionary<Type, Dictionary<Type, List<ManagedMethodInfo>>>();
        protected static bool iManagedMethodsCached = false;


        protected Type icachedType = null;
        [SerializeField]
        protected IBehaviourContainer iContainer = null;
        protected bool iIsDoEnableWorked = false;
        protected bool iIsEnabled = false;
        [NonSerialized]
        protected Dictionary<Type, List<IManagedMethodData>> iManagedMethodsData = new Dictionary<Type, List<IManagedMethodData>>();

        public IObjectManager MasterManager
        {
            get => Container.MasterManager;
            set => Container.MasterManager = value;
        }

        protected virtual IBehaviourContainer DefaultRegisterContainer()
        {
            return Container;

        }

        public static bool IsBehaviourAttributesCollected(Type behaviourType)
        {
            return iBehaviourAttributes.ContainsKey(behaviourType);
        }

        protected static Dictionary<Type, // Behaviour type
            Dictionary<Type, // Behaviour attribute category type (EnableDependent, Lifecycle)
                Dictionary<Attributes.BehaviourBinaryAttributeArea, // Working area of behaviour attribute
                    List<IBaseBehaviourBinaryAttribute>>>> iBehaviourAttributes = new Dictionary<Type, Dictionary<Type, Dictionary<BehaviourBinaryAttributeArea, List<IBaseBehaviourBinaryAttribute>>>>();
        //new List<IEnableDependentBehaviourAttribute>[Enum.GetValues(typeof(BehaviourBinaryAttributeArea)).Length];
        //protected static Dictionary<Type, List<List<ILifecycleBehaviourAttribute>[]>> iLifecycleAreasAttributes = new Dictionary<Type, List<List<ILifecycleBehaviourAttribute>[]>>();
            //new List<ILifecycleBehaviourAttribute>[Enum.GetValues(typeof(BehaviourBinaryAttributeArea)).Length];




        public List<IBaseBehaviourBinaryAttribute> GetEnableDependentAttributes(BehaviourBinaryAttributeArea area)
        {
            return iBehaviourAttributes[cachedType][typeof(IEnableDependentBehaviourAttribute)][area];
        }

        public Dictionary<BehaviourBinaryAttributeArea, List<IBaseBehaviourBinaryAttribute>> GetAllEnableDependentAttributes()
        {
            return iBehaviourAttributes[cachedType][typeof(IEnableDependentBehaviourAttribute)];
        }

        public List<IBaseBehaviourBinaryAttribute> GetLifecycleAttributes(BehaviourBinaryAttributeArea area)
        {
            return iBehaviourAttributes[cachedType][typeof(ILifecycleBehaviourAttribute)][area];
        }

        public Dictionary<BehaviourBinaryAttributeArea, List<IBaseBehaviourBinaryAttribute>> GetAllLifecycleAttributes()
        {
            return iBehaviourAttributes[cachedType][typeof(ILifecycleBehaviourAttribute)];
        }

        public bool HandleBehaviourAttributes<AttrType>(Dictionary<BehaviourBinaryAttributeArea, List<IBaseBehaviourBinaryAttribute>> attr_array, BehaviourBinaryAttributeHandleDirection direction) where AttrType : IBaseBehaviourBinaryAttribute
        {
            List<IBaseBehaviourBinaryAttribute> attrList;

            if (attr_array.TryGetValue(BehaviourBinaryAttributeArea.Local, out attrList))
            {
                foreach (AttrType attr in attrList)
                {
                    if (!attr.Handle(cachedType, this, this, direction))
                        return false;
                }
            }

            if (attr_array.TryGetValue(BehaviourBinaryAttributeArea.Container, out attrList))
            {
                foreach (IObjectBehavioursBase behaviour in GetComponents<IObjectBehavioursBase>())
                {
                    if (behaviour.Equals(this))
                        continue;

                    foreach (AttrType attr in attrList)
                    {
                        if (!attr.Handle(cachedType, this, behaviour, direction))
                            return false;
                    }
                }
            }
            /*
            else if (attr_array[(int)BehaviourBinaryAttributeArea.ContainerAndChilds].Count > 0)
            {
                foreach (IObjectBehavioursBase behaviour in GetComponentsInChildren<IObjectBehavioursBase>())
                {
                    if (behaviour.Equals(this))
                        continue;

                    foreach (AttrType attr in
                             attr_array[(int)BehaviourBinaryAttributeArea.ContainerAndChilds])
                    {
                        if (!attr.Handle(cachedType, this, behaviour, direction))
                            return false;
                    }
                }
            }*/

            return true;
        }

        public static void CollectBehaviourAttributes(Type behaviourType)
        {
            if (IsBehaviourAttributesCollected(behaviourType))
                return;

            Dictionary<Type, // Behaviour attribute category type (EnableDependent, Lifecycle)
                Dictionary<Attributes.BehaviourBinaryAttributeArea, // Working area of behaviour attribute
                    List<IBaseBehaviourBinaryAttribute>>> typeAttributes = null;

            if (!iBehaviourAttributes.TryGetValue(behaviourType, out typeAttributes))
            {
                typeAttributes = new Dictionary<Type, Dictionary<BehaviourBinaryAttributeArea, List<IBaseBehaviourBinaryAttribute>>>();
                iBehaviourAttributes.Add(behaviourType, typeAttributes);
            }

            IEnableDependentBehaviourAttribute edb_attr = null;
            ILifecycleBehaviourAttribute lb_attr = null;
            Dictionary<Attributes.BehaviourBinaryAttributeArea, // Working area of behaviour attribute
                    List<IBaseBehaviourBinaryAttribute>> enabledStateBehaviourAttributes;
            Dictionary<Attributes.BehaviourBinaryAttributeArea, // Working area of behaviour attribute
                    List<IBaseBehaviourBinaryAttribute>> lifecycleStateBehaviourAttributes;

            if (!typeAttributes.TryGetValue(typeof(IEnableDependentBehaviourAttribute), out enabledStateBehaviourAttributes))
            {
                enabledStateBehaviourAttributes = new Dictionary<BehaviourBinaryAttributeArea, List<IBaseBehaviourBinaryAttribute>>();
                typeAttributes.Add(typeof(IEnableDependentBehaviourAttribute), enabledStateBehaviourAttributes);
            }

            if (!typeAttributes.TryGetValue(typeof(ILifecycleBehaviourAttribute), out lifecycleStateBehaviourAttributes))
            {
                lifecycleStateBehaviourAttributes = new Dictionary<BehaviourBinaryAttributeArea, List<IBaseBehaviourBinaryAttribute>>();
                typeAttributes.Add(typeof(ILifecycleBehaviourAttribute), lifecycleStateBehaviourAttributes);
            }

            foreach (Attribute attr in behaviourType.GetCustomAttributes(true))
            {
                List<IBaseBehaviourBinaryAttribute> attributeList;

                if (attr is IEnableDependentBehaviourAttribute)
                {
                    edb_attr = attr as IEnableDependentBehaviourAttribute;
                    //edb_attr.WorkingArea = BehaviourBinaryAttributeArea.Container;

                    if (!enabledStateBehaviourAttributes.TryGetValue(edb_attr.WorkingArea, out attributeList))
                    {
                        attributeList = new List<IBaseBehaviourBinaryAttribute>();
                        enabledStateBehaviourAttributes.Add(edb_attr.WorkingArea, attributeList);
                    }

                    attributeList.Add(edb_attr);
                }
                else if (attr is ILifecycleBehaviourAttribute)
                {
                    lb_attr = attr as ILifecycleBehaviourAttribute;
                    //lb_attr.WorkingArea = BehaviourBinaryAttributeArea.Container;

                    if (!lifecycleStateBehaviourAttributes.TryGetValue(edb_attr.WorkingArea, out attributeList))
                    {
                        attributeList = new List<IBaseBehaviourBinaryAttribute>();
                        lifecycleStateBehaviourAttributes.Add(edb_attr.WorkingArea, attributeList);
                    }

                    attributeList.Add(lb_attr);
                }
            }

        }

        protected virtual bool DoEnable()
        {
         /*   if (!HandleBehaviourAttributes<IEnableDependentBehaviourAttribute>(
                    GetAllEnableDependentAttributes(), 
                    BehaviourBinaryAttributeHandleDirection.One
                    )
                )
                return false;
         */
            ActivateDelegateAttribute(typeof(EnabledStateEventAttribute));
            ActivateDelegateAttribute(typeof(SharedPropertyViewerAttribute));
            ActivateDelegateAttribute(typeof(SharedPropertyHandlerAttribute));
            Event<Aggregator.Events.Tools.OnBehaviourEnableStateEvent>(Container).Invoke(cachedType, true);
            iIsEnabled = true;
            return true;
        }

        protected virtual bool DoDisable()
        {
            /*if (!HandleBehaviourAttributes<IEnableDependentBehaviourAttribute>(
                    GetAllEnableDependentAttributes(), 
                    BehaviourBinaryAttributeHandleDirection.Zero
                    )
                )
                return false;
            */
            Event<Aggregator.Events.Tools.OnBehaviourEnableStateEvent>(Container).Invoke(cachedType, false);
            DeactivateDelegateAttribute(typeof(SharedPropertyHandlerAttribute));
            DeactivateDelegateAttribute(typeof(SharedPropertyViewerAttribute));
            DeactivateDelegateAttribute(typeof(EnabledStateEventAttribute));
            iIsEnabled = false;
            
            return true;
        }


        protected override void Awake()
        {
            base.Awake();

            if (!IsManagedMethodsReadyToUse)
                ProvideDelegatesForManagedMethods();

            if (!IsBehaviourAttributesCollected(cachedType))
                CollectBehaviourAttributes(cachedType);

            Container.OnBehaviourAdd(this);
            InjectSharedProperties();
            /*
            HandleBehaviourAttributes<ILifecycleBehaviourAttribute>(
                GetAllLifecycleAttributes(), 
                BehaviourBinaryAttributeHandleDirection.One
            );
            */
            ActivateDelegateAttribute(typeof(LifecycleEventAttribute));
        }

        protected override void Start()
        {
            base.Start();

            PostActivateDelegateAttribute(typeof(SharedPropertyViewerAttribute));
            PostActivateDelegateAttribute(typeof(SharedPropertyHandlerAttribute));
        }

        protected override void OnDestroy()
        {
            /*
            HandleBehaviourAttributes<ILifecycleBehaviourAttribute>(
                GetAllLifecycleAttributes(), 
                BehaviourBinaryAttributeHandleDirection.Zero
            );
            */
            DeactivateDelegateAttribute(typeof(LifecycleEventAttribute));
            Container.OnBehaviourDestroy(this);
            base.OnDestroy();
        }

        protected virtual void OnEnable()
        {
            if (!iIsEnabled &&
                !DoEnable())
            {
                enabled = false;
            }
        }

        protected virtual void OnDisable()
        {
            if (!gameObject.activeSelf)
                return;
            
            if (iIsEnabled && 
                !DoDisable()) enabled = true;

        }

        protected override void OnValidate()
        {
            if (Application.isEditor && false)
            {
                ActivateDelegateAttribute(typeof(LifecycleEventAttribute));
                PostActivateDelegateAttribute(typeof(SharedPropertyViewerAttribute));
                PostActivateDelegateAttribute(typeof(SharedPropertyHandlerAttribute));
            }

            base.OnValidate();
            InjectSharedProperties();
        }


        public bool AddEventListener(IEventProcessor listener) => Container.AddEventListener(listener);
        public bool RemoveEventListener(IEventProcessor listener) => Container.RemoveEventListener(listener);
        public bool BindMutuallyListener(IEventProcessor listener) => Container.BindMutuallyListener(listener);
        public bool UnbindMutuallyListener(IEventProcessor listener) => Container.UnbindMutuallyListener(listener);
        public void AddEventListener(Type event_id, Delegate listener) => Container.AddEventListener(event_id, listener);
        public void RemoveEventListener(Type event_id, Delegate listener) => Container.RemoveEventListener(event_id, listener);

        public T Event<T>(IEventProcessor sender) where T : IEventData => Container.Event<T>(sender);

        public IEventData Event(Type eventType, IEventProcessor sender) => Container.Event(eventType, sender);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeType">Attribute type must implements IActivableAttribute </param>
        public void ActivateDelegateAttribute(Type attributeType)
        {
            if (!typeof(IDelegateActivableAttribute).IsAssignableFrom(attributeType))
                throw new InvalidCastException("Attribute type must implements '" + nameof(IDelegateActivableAttribute) + "'" );

            var dataList = ManagedMethodsDataByAttribute(attributeType);

            if (dataList != null)
            {
                foreach (var data in dataList)
                {
                    data.Activate();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeType">Attribute type must implements IActivableAttribute </param>
        public void PostActivateDelegateAttribute(Type attributeType)
        {
            if (!typeof(IDelegateActivableAttribute).IsAssignableFrom(attributeType))
                throw new InvalidCastException("Attribute type must implements '" + nameof(IDelegateActivableAttribute) + "'");

            var dataList = ManagedMethodsDataByAttribute(attributeType);

            if (dataList != null)
            {
                foreach (var data in dataList)
                {
                    data.PostActivate();
                }
            }
        }

        public void DeactivateDelegateAttribute(Type attributeType)
        {
            if (!typeof(IDelegateActivableAttribute).IsAssignableFrom(attributeType))
                throw new InvalidCastException("Attribute type must implements '" + nameof(IDelegateActivableAttribute) + "'");

            var dataList = ManagedMethodsDataByAttribute(attributeType);

            if (dataList != null)
            {
                foreach (var data in dataList)
                {
                    data.Deactivate();
                }
            }
        }

        public IEnumerable<IManagedMethodData> ManagedMethodsDataByAttribute (Type managedDelegateAttributeType)
        {
            List<IManagedMethodData> result;

            if (iManagedMethodsData.TryGetValue(managedDelegateAttributeType, out result))
                return result;

            return null;
        }

        public void ClearDelegatesCacheByAttribute(Type managedDelegateAttributeType)
        {
            List<IManagedMethodData> result;

            if (iManagedMethodsData.TryGetValue(managedDelegateAttributeType, out result))
                result.Clear();
        }

        public void InjectSharedProperties()
        {
            TypeReflector.InjectProperties(this, BindingFlags.Instance | BindingFlags.Public, true);
        }

        public bool IsManagedMethodsReadyToUse => iManagedMethodsData.ContainsKey(cachedType);

        public void ClearAllEventListeners() => Container.ClearAllEventListeners();

        public void AddEventListener<T>(Action<T> listener) where T : IEventData => Container.AddEventListener<T>(listener);

        public void RemoveEventListener<T>(Action<T> listener) where T : IEventData => Container.RemoveEventListener<T>(listener);

        public void InvokeEvent(Type key, IEventData eventData) => Container.InvokeEvent(key, eventData);

        public void InvokeEvent<T>(T eventData) where T : IEventData => Container.InvokeEvent<T>(eventData);

        public ISharedProperty SharedProperty(Type sharedPropType) => Container.SharedProperty(sharedPropType);

        public T SharedProperty<T>() where T : ISharedProperty => Container.SharedProperty<T>();

        public void MergeSharedProperties(ISharedPropertiesContainer source) => Container.SharedPropertyContainer.MergeSharedProperties(source);

        public bool DeleteSharedProperty(Type propType) => Container.DeleteSharedProperty(propType);

        public bool DeleteSharedProperty<T>() where T : ISharedProperty => Container.DeleteSharedProperty<T>();

        public void AssignSharedProperties(ISharedPropertiesContainer source) => Container.AssignSharedProperties(source);

        public bool HasProperty(Type propType) => Container.HasProperty(propType);

        public bool HasProperty<T>() where T : ISharedProperty => Container.HasProperty<T>();

        public void SanitizeSerializedProperties() => Container.SanitizeSerializedProperties();

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (!IsManagedMethodsReadyToUse)
                ProvideDelegatesForManagedMethods();

            if (!IsBehaviourAttributesCollected(cachedType))
                CollectBehaviourAttributes(cachedType);
        }

        public void ClearSharedProperties() => Container.ClearSharedProperties();

        public IReadOnlyCollection<ISharedProperty> PropertyCollection => Container.PropertyCollection;

        protected static List<ManagedMethodInfo> ManagedMethodsByAttributeType(Type behaviourType, Type attributeType)
        {
            Dictionary<Type, List<ManagedMethodInfo>> iTypeMethodsAttributes;

            if (iManagedMethodsAttributes.TryGetValue(behaviourType, out iTypeMethodsAttributes))
            {
                CollectManagedMethodsForBehaviour(behaviourType);

                List<ManagedMethodInfo> result;
                if (iTypeMethodsAttributes.TryGetValue(attributeType, out result))
                    return result;
                else
                    return null;
            }

            return null;
        }

        protected static void CollectManagedMethodsForBehaviour(Type behaviourType)
        {
            Dictionary<Type, List<ManagedMethodInfo>> iTypeMethodsAttributes = null;

            if (!iManagedMethodsAttributes.TryGetValue(behaviourType, out iTypeMethodsAttributes))
                iManagedMethodsAttributes.Add(
                    behaviourType, 
                    iTypeMethodsAttributes = new Dictionary<Type, List<ManagedMethodInfo>>()
                    );

            IDelegateActivableAttribute attribute;

            try
            {
                iTypeMethodsAttributes.Clear();
                foreach (var mr in behaviourType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    attribute = null;

                    try
                    {
                        foreach (var attr in mr.GetCustomAttributes(true))
                        {
                            if (typeof(IDelegateActivableAttribute).IsAssignableFrom(attr.GetType()))
                            {
                                attribute = attr as IDelegateActivableAttribute;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        GLog.Log(e);
                    }

                    if (attribute == null)
                        continue;

                    List<ManagedMethodInfo> attributeList;

                    if (!iTypeMethodsAttributes.TryGetValue(attribute.GetType(), out attributeList))
                    {
                        attributeList = new List<ManagedMethodInfo>();
                        iTypeMethodsAttributes.Add(attribute.GetType(), attributeList);
                    }

                    attributeList.Add(new ManagedMethodInfo(mr, attribute));
                }
            }
            catch (Exception e)
            {
                GLog.Log(e);
            }

        }

        protected void ProvideDelegatesForManagedMethods()
        {
            Dictionary<Type, List<ManagedMethodInfo>> iManagedMethods;

            if (!iManagedMethodsAttributes.TryGetValue(cachedType, out iManagedMethods))
            {
                CollectManagedMethodsForBehaviour(cachedType);
            }

            iManagedMethodsAttributes.TryGetValue(cachedType, out iManagedMethods);
            
            if (iManagedMethods == null)
                return;

            foreach (var keyval in iManagedMethods)
            {
                List<IManagedMethodData> dataList = null;

                if (!iManagedMethodsData.TryGetValue(keyval.Key, out dataList))
                {
                    dataList = new List<IManagedMethodData>(keyval.Value.Count);
                    iManagedMethodsData.Add(keyval.Key, dataList);
                }
                else
                {
                    dataList.Clear();
                    dataList.Capacity = keyval.Value.Capacity;
                }

                foreach (var methodInfo in keyval.Value)
                {
                    IManagedMethodData methodData = methodInfo.methodAttribute.CreateData();

                    if (!methodData.Prepare(cachedType, this, methodInfo.methodAttribute, methodInfo.methodInfo, methodInfo.methodAttribute.NotUseInEditMode))
                        continue;

                    dataList.Add(methodData);
                }
            }

        }

    }

}