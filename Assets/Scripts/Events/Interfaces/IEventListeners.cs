using UnityEngine.Events;
using System;

namespace Main.Events
{

    public interface IEventListeners<DELEGATE>
    {
        void AddListener(DELEGATE listener);
        void RemoveListener(DELEGATE listener);

        void Clear();

    }

    public interface IEventListeners: IEventListeners<Delegate>, IInvokable
    {
    }

}