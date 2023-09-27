using System.Collections;
using System.Collections.Generic;
using Main.Objects;
using Main.Events;
using System;

namespace Main.Managers
{

    [Serializable]
    public class ReadonlyContainerCollection: EventProcessor, IReadonlyContainerCollection
    {
        protected bool iDisposed = false;
        public int Count => iContainers.Count;
        public bool IsReadOnly => false;

        public bool Disposed => iDisposed;

        [UnityEngine.SerializeField]
        protected ListEx<IBehaviourContainer> iContainers { get; } =
            new ListEx<IBehaviourContainer>(
                Configuration.OBJECT_MANAGER_START_STORAGE_SIZE,
                true,
                Configuration.OBJECT_MANAGER_STORAGE_SIZE_GROW);

        public IBehaviourContainer First => (Count > 0) ? this[0] : null;
        public IBehaviourContainer Last => (Count > 0) ? this[Count - 1] : null;
        public IBehaviourContainer this[int index] => iContainers[index];

        public bool Contains(IBehaviourContainer item) => iContainers.Contains(item);
        public IEnumerator<IBehaviourContainer> GetEnumerator() => iContainers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => iContainers.GetEnumerator();

        public ReadonlyContainerCollection() : base()
        {
        }

        public virtual void Dispose()
        {
        }
    }

    [Serializable]
    public class ContainerCollection : ReadonlyContainerCollection, IContainerCollection
    {

        public void OnBehaviourContainerDestroyedEvent(Aggregator.Events.Tools.OnBehaviourContainerDestroyedEvent eventData)
        {
            Remove(eventData.Sender as IBehaviourContainer);
        }

        public virtual void Add(IBehaviourContainer item)
        {
            iContainers.Add(item);
            item.AddEventListener(this);
        }

        public virtual bool Remove(IBehaviourContainer item)
        {
            if (!iContainers.Remove(item))
            {
                item.RemoveEventListener(this);
                return true;
            }

            return false;
        }


        public virtual void Clear()
        {
            foreach (IBehaviourContainer element in this)
            {
                element.RemoveEventListener(this);
            }

            iContainers.Clear();
        }

        public void CopyTo(IBehaviourContainer[] array, int arrayIndex) => iContainers.CopyTo(array, arrayIndex);

        public ContainerCollection(): base()
        {
            AddEventListener<Aggregator.Events.Tools.OnBehaviourContainerDestroyedEvent>(OnBehaviourContainerDestroyedEvent);
        }

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Clear();
            base.Dispose();
        }
    }
}
