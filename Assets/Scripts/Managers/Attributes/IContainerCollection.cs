using System.Collections.Generic;
using Main.Objects;
using Main.Events;
using System;

namespace Main.Managers
{
    public interface IReadonlyContainerCollection: IEventProcessor, IReadOnlyList<IBehaviourContainer>, IDisposable
    {
        bool Disposed { get; }

        /// <summary>
        /// item[0] or null if list is empty
        /// </summary>
        IBehaviourContainer First { get; }
        /// <summary>
        /// item[Count - 1] or null if list is empty
        /// </summary>
        IBehaviourContainer Last { get; }
    }

    public interface IContainerCollection: IReadonlyContainerCollection, ICollection<IBehaviourContainer>
    {
        new int Count { get; }
    }
}
