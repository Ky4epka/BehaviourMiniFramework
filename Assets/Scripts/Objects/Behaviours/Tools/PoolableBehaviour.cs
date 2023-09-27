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

namespace Main.Aggregator.Events.Behaviours.Tools.PoolableBehaviour
{
    public sealed class OnPoolRentEvent: EventDataBase
    {

    }

    public sealed class OnPoolReturnEvent : EventDataBase
    {

    }

    public sealed class DoPoolReturnEvent : EventDataBase
    {

    }
}


namespace Main.Aggregator.Events.Behaviours.Tools.PoolableBehaviour
{
    public class ReturnToPoolDelayProperty : SharedPropertyEvent<float>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tools.PoolableBehaviour
{
    public class ReturnToPoolDelayProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Tools.PoolableBehaviour.ReturnToPoolDelayProperty>
    {
        public override string GroupTag => "Poolable";
        public override string SharedName => "ReturnToPoolDelay";
    }
}

namespace Main.Objects.Behaviours.Tools
{
 

    public class PoolableBehaviour : ObjectBehavioursBase, IDataPool_Element
    {
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Tools.PoolableBehaviour.ReturnToPoolDelayProperty ReturnToPoolDelay { get; protected set; }

        protected DataPool_ElementData fDataPool_ElementData;
        protected Coroutine iReturnDelayCoroutine = null;

        [EnabledStateEvent]
        public void DoPoolReturnEvent(Aggregator.Events.Behaviours.Tools.PoolableBehaviour.DoPoolReturnEvent eventData)
        {
            if (MathKit.NumbersEquals(ReturnToPoolDelay.Value, float.Epsilon))
                DoReturn();
            else {
                if (iReturnDelayCoroutine == null)
                    iReturnDelayCoroutine = StartCoroutine(ReturnDelayCoroutine());
            }
        }

        protected IEnumerator ReturnDelayCoroutine()
        {
            yield return new WaitForSeconds(ReturnToPoolDelay.Value);
            iReturnDelayCoroutine = null;
            DoReturn();
        }

        public void DataPool_Element_SetData(DataPool_ElementData data)
        {
            fDataPool_ElementData = data;
        }

        public DataPool_ElementData DataPool_Element_GetData()
        {
            return fDataPool_ElementData;
        }

        public void Dispose()
        {
            DoReturn();
        }

        public virtual void OnRent()
        {
            Event<Aggregator.Events.Behaviours.Tools.PoolableBehaviour.OnPoolRentEvent>(Container);
            gameObject.SetActive(true);
        }

        public virtual void OnReturn()
        {
            Event<Aggregator.Events.Behaviours.Tools.PoolableBehaviour.OnPoolReturnEvent>(Container);

            if (gameObject)
                gameObject.SetActive(false);

        }

        public void DoReturn()
        {
            if (Application.isPlaying)
                DataPool_Element_GetData().fOwner.ReturnElement(this);
        }
    }

}