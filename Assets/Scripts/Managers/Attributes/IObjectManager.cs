using Main.Objects;
using Main.Objects.Behaviours;
using System.Collections.Generic;
using System;

namespace Main.Managers
{
    public interface IObjectManager: IDisposable, IContainerCollection, IManagedObject
    {
        new bool Disposed { get; }
    }
}
