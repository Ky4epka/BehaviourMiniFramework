using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Player;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using System;


namespace Main.Aggregator.Events.Managers.PoolManager.PoolableFabric
{
    public class ElementTypeProperty : SharedPropertyEvent<Main.Managers.ElementTypeWrapper>
    {
    }
}
namespace Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric
{
    public class ElementTypeProperty : SharedPropertyReference<Main.Managers.ElementTypeWrapper, Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.ElementTypeProperty>
    {
        public override string GroupTag => "Fabric";
        public override string SharedName => "ElementType";
    }
}


namespace Main.Aggregator.Events.Managers.PoolManager.PoolableFabric
{
    public class PrototypeProperty : SharedPropertyEvent<GameObject>
    {
    }
}
namespace Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric
{
    public class PrototypeProperty : SharedPropertyReference<GameObject, Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.PrototypeProperty>
    {
        public override string GroupTag => "Fabric";
        public override string SharedName => "Prototype";
    }
}


namespace Main.Aggregator.Events.Managers.PoolManager.PoolableFabric
{
    public class RootProperty : SharedPropertyEvent<Transform>
    {
    }
}
namespace Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric
{
    public class RootProperty : SharedPropertyReference<Transform, Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.RootProperty>
    {
        public override string GroupTag => "Fabric";
        public override string SharedName => "Root";
    }
}


namespace Main.Aggregator.Events.Managers.PoolManager.PoolableFabric
{
    public class InitialCapacityProperty : SharedPropertyEvent<int>
    {
    }
}
namespace Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric
{
    public class InitialCapacityProperty : SharedProperty<int, Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.InitialCapacityProperty>
    {
        public override string GroupTag => "Fabric";
        public override string SharedName => "InitialCapacity";
    }
}


namespace Main.Aggregator.Events.Managers.PoolManager.PoolableFabric
{
    public class GrowQuotaProperty : SharedPropertyEvent<int>
    {
    }
}
namespace Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric
{
    public class GrowQuotaProperty : SharedProperty<int, Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.GrowQuotaProperty>
    {
        public override string GroupTag => "Fabric";
        public override string SharedName => "GrowQuota";
    }
}


namespace Main.Aggregator.Events.Managers.PoolManager.PoolableFabric
{
    public class UseAutoGrowProperty : SharedPropertyEvent<bool>
    {
    }
}
namespace Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric
{
    public class UseAutoGrowProperty : SharedProperty<bool, Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.UseAutoGrowProperty>
    {
        public override string GroupTag => "Fabric";
        public override string SharedName => "UseAutoGrow";
    }
}

namespace Main.Managers
{
    using Main.Objects.Behaviours.Tools;

    public interface IPoolableBehaviourFabric: IDataPoolElementFabric
    {
        GameObject Prototype { get;}
        Transform Root { get;}
        Type ElementType { get;}
    }

    public class ElementTypeWrapper : Main.Other.TypeWrapper
    {
        public override Type BaseType => typeof(PoolableBehaviour);
    }


    [RequireComponent(typeof(PoolManagerContainer))]
    [Unique]
    [DisallowMultipleComponent]
    public class PoolableFabricBehaviour : ObjectBehavioursBase, IPoolableBehaviourFabric
    {

        [SharedProperty]
        public Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.PrototypeProperty PrototypeProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.RootProperty RootProperty { get; protected set; }
        [SharedProperty(DefaultConstructorIfDefaultReferenceValue = true)]
        public Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.ElementTypeProperty ElementTypeProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.InitialCapacityProperty InitialCapacityProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.UseAutoGrowProperty UseAutoGrowProperty { get; protected set; }
        [SharedProperty]
        public Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.GrowQuotaProperty GrowQuotaProperty { get; protected set; }

        public GameObject Prototype { get => PrototypeProperty.Value; }
        public Transform Root { get => RootProperty.Value; }
        public Type ElementType => ElementTypeProperty.Value.SelectedType;

        protected IDataPool iPoolInstance = null;

        [SharedPropertyHandler(typeof(Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.ElementTypeProperty))]
        public bool ElementTypePropertyHandler(ISharedProperty property, ElementTypeWrapper old_value, ref ElementTypeWrapper new_value)
        {
            if (typeof(PoolableBehaviour).IsAssignableFrom(new_value?.SelectedType?.GetType()))
            {
                GLog.LogError(nameof(PoolableFabricBehaviour), $"Couldnot set property {nameof(ElementType)}. Selected type is not assignable from {typeof(PoolableBehaviour)}");
                return false;
            }

            return true;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.ElementTypeProperty))]
        public void ElementTypePropertyViewer(Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.ElementTypeProperty eventData)
        {
            PoolManager.Instance.DeletePool(eventData.PropertyValue.SelectedType);
            iPoolInstance = PoolManager.Instance.CreatePool(eventData.PropertyValue.SelectedType, this, InitialCapacityProperty.Value, UseAutoGrowProperty.Value);
        }

        [SharedPropertyHandler(typeof(Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.PrototypeProperty))]
        public bool PrototypePropertyHandler(ISharedProperty property, GameObject old_value, ref GameObject new_value)
        {
            if (!new_value.GetComponent(ElementTypeProperty.Value.SelectedType))
            {
                GLog.LogError(nameof(PoolableFabricBehaviour), $"Couldnot set property {nameof(PrototypeProperty)}. Selected prefab not containse component of type '{ElementTypeProperty.Value.SelectedType?.FullName ?? "null"}'");
                new_value = old_value;
                return false;
            }

            return true;
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.InitialCapacityProperty))]
        public void InitialCountPropertyViewer(Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.InitialCapacityProperty eventData)
        {
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.UseAutoGrowProperty))]
        public void UseAutoGrowPropertyViewer(Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.UseAutoGrowProperty eventData)
        {
            if (iPoolInstance != null)
                iPoolInstance.SetUseAutogrow(eventData.PropertyValue);
        }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Managers.PoolManager.PoolableFabric.GrowQuotaProperty))]
        public void GrowQuotaPropertyViewer(Main.Aggregator.Events.Managers.PoolManager.PoolableFabric.GrowQuotaProperty eventData)
        {
            if (iPoolInstance != null)
                iPoolInstance.SetGrowQuota(eventData.PropertyValue);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying)
                ElementTypeProperty.DirtyValue();
        }

        public IDataPool_Element BuildElement()
        {
            GameObject instance = GameObject.Instantiate(PrototypeProperty.Value, RootProperty.Value);
            instance.name = instance.name.Replace("(Clone)", "") + " [pool]";
            instance.SetActive(false);
            PoolableBehaviour poolableBeh = instance.GetComponent<PoolableBehaviour>();

            if (!poolableBeh)
                poolableBeh = instance.AddComponent<PoolableBehaviour>();

            return poolableBeh;
        }

        public void DestroyElement(IDataPool_Element element)
        {
            if (element as PoolableBehaviour)
                GameObject.Destroy((element as PoolableBehaviour).gameObject);
        }

    }
}
