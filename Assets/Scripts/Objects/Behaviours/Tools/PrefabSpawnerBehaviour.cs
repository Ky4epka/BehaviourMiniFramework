using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Aggregator.Enum.Behaviours.Tools.PrefabSpawner
{
    public enum SpawnSelection
    {
        Random = 0
    }
}

namespace Main.Aggregator.Events.Behaviours.Tools.PrefabSpawner
{
    public class SpawnSelectionProperty : SharedPropertyEvent<Main.Aggregator.Enum.Behaviours.Tools.PrefabSpawner.SpawnSelection>
    {
    }
}

namespace Main.Aggregator.Properties.Behaviours.Tools.PrefabSpawner
{
    public class SpawnSelectionProperty : SharedEnumProperty<Main.Aggregator.Enum.Behaviours.Tools.PrefabSpawner.SpawnSelection, Main.Aggregator.Events.Behaviours.Tools.PrefabSpawner.SpawnSelectionProperty>
    {
        public override string GroupTag => "Prefab spawner";
        public override string SharedName => "SpawnTactic";
    }
}


namespace Main.Aggregator.Events.Behaviours.Tools.PrefabSpawner
{
    public class PrefabListProperty : SharedPropertyEvent<List<GameObject>>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tools.PrefabSpawner
{
    public class PrefabListProperty : SharedPropertyReference<List<GameObject>, Main.Aggregator.Events.Behaviours.Tools.PrefabSpawner.PrefabListProperty>
    {
        public override string GroupTag => "Prefab spawner";
        public override string SharedName => "PrefabList";
    }
}

namespace Main.Aggregator.Events.Behaviours.Tools.PrefabSpawner
{
    public class DoSpawnEvent: EventDataBase
    {

    }

    public class OnSpawnEvent : EventDataBase
    {

    }
}

namespace Main.Objects.Behaviours.Tools
{
    [RequireComponent(typeof(SpriteRenderer))]
    [DisallowMultipleComponent]
    public class PrefabSpawnerBehaviour: ObjectBehavioursBase
    {
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Tools.PrefabSpawner.SpawnSelectionProperty SpawnSelection { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Tools.PrefabSpawner.PrefabListProperty PrefabList { get; protected set; }

        [EnabledStateEvent]
        public void DoSpawnEvent(Aggregator.Events.Behaviours.Tools.PrefabSpawner.DoSpawnEvent eventData)
        {
            DoSpawn();
            Event<Aggregator.Events.Behaviours.Tools.PrefabSpawner.OnSpawnEvent>(Container).Invoke();
        }

        public virtual bool CheckSpawn()
        {
            return true;
        }

        protected virtual GameObject CreatePrefab()
        {
            return null;
        }

        protected virtual void DoSpawn()
        {
            switch (SpawnSelection.Value)
            {
                case Aggregator.Enum.Behaviours.Tools.PrefabSpawner.SpawnSelection.Random:
                    CreatePrefab();
                    break;
            }
        }
    }
}