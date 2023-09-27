using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours.Attributes;

namespace Main.Objects.Behaviours.Tools
{
}

namespace Main.Aggregator.Events.Tools.PrefabInjector
{
    public sealed class PrefabProperty : SharedPropertyEvent<GameObject>
    {
        public PrefabProperty() : base()
        {
        }
    }

    public sealed class PrefabVisibleStateProperty : SharedPropertyEvent<bool>
    {
        public PrefabVisibleStateProperty() : base()
        {
        }
    }
}

namespace Main.Aggregator.Properties.Tools.PrefabInjector
{
    public sealed class PrefabProperty : SharedPropertyReference<GameObject, Aggregator.Events.Tools.PrefabInjector.PrefabProperty>
    {
        public override string GroupTag => "Tools";

        public override string SharedName => "TestPrefab";
    }


    public sealed class PrefabVisibleSateProperty : SharedProperty<bool, Aggregator.Events.Tools.PrefabInjector.PrefabVisibleStateProperty>
    {
        public override string GroupTag => "Tools";

        public override string SharedName => "TestPrefabVisible";
    }
}


namespace Main.Objects.Behaviours.Tools
{

    public abstract class PrefabInjectorBaseBehaviour : ObjectBehavioursBase
    {
        public abstract ISharedPropertyReference<GameObject> Prefab { get; protected set; }
        public abstract ISharedProperty<bool> PrefabVisibleState { get; protected set; }

        protected GameObject iControllingObject = null;

        public void PrefabViewer(IEventData eventData)
        {
            if (iControllingObject != null) GameObject.Destroy(iControllingObject);

            iControllingObject = (Prefab.Value != null) ? GameObject.Instantiate(Prefab.Value, transform) : null;
        }

        public void PrefabVisibleStateViewer(IEventData eventData)
        {
            iControllingObject?.SetActive(PrefabVisibleState.Value);
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable()) return false;

            iControllingObject?.SetActive(PrefabVisibleState.Value);
            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable()) return false;

            iControllingObject?.SetActive(false);
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            AddEventListener(Prefab.EventType, (System.Action<IEventData>)PrefabViewer);
            AddEventListener(PrefabVisibleState.EventType, (System.Action<IEventData>)PrefabVisibleStateViewer);
            Prefab.EventValueFor((System.Action<IEventData>)PrefabViewer);
            PrefabVisibleState.EventValueFor((System.Action<IEventData>)PrefabViewer);
        }

        protected override void OnDestroy()
        {
            RemoveEventListener(Prefab.EventType, (System.Action<IEventData>)PrefabViewer);
            RemoveEventListener(PrefabVisibleState.EventType, (System.Action<IEventData>)PrefabVisibleStateViewer);

            if (iControllingObject != null)
                GameObject.Destroy(iControllingObject);

            base.OnDestroy();
        }
    }

}