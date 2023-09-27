using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;

namespace Main.Aggregator.Events.MapCell
{
    public sealed class PointerEnterEvent : EventDataBase
    {
        public PointerEventData EventData { get; private set; }

        public PointerEnterEvent() : base()
        {
        }

        public void Invoke(PointerEventData eventData)
        {
            EventData = eventData;
            base.Invoke();
        }
    }

    public sealed class PointerLeaveEvent : EventDataBase
    {
        public PointerEventData EventData { get; private set; }

        public PointerLeaveEvent() : base()
        {
        }

        public void Invoke(PointerEventData eventData)
        {
            EventData = eventData;
            base.Invoke();
        }
    }

    public sealed class PointerClickEvent : EventDataBase
    {
        public PointerEventData EventData { get; private set; }

        public PointerClickEvent() : base()
        {
        }

        public void Invoke(PointerEventData eventData)
        {
            EventData = eventData;
            base.Invoke();
        }
    }

    public sealed class PointerDownEvent : EventDataBase
    {
        public PointerEventData PointerEventData { get; private set; }

        public PointerDownEvent() : base()
        {
        }

        public void Invoke(PointerEventData eventData)
        {
            PointerEventData = eventData;
            base.Invoke();
        }
    }

    public sealed class PointerUpEvent : EventDataBase
    {
        public PointerEventData EventData { get; private set; }

        public PointerUpEvent() : base()
        {
        }

        public void Invoke(PointerEventData eventData)
        {
            EventData = eventData;
            base.Invoke();
        }
    }

}

namespace Main
{

    public class MapCell_PointerEventWrapper : ObjectBehavioursBase, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Event<Aggregator.Events.MapCell.PointerClickEvent>(Container).Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Event<Aggregator.Events.MapCell.PointerDownEvent>(Container).Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Event<Aggregator.Events.MapCell.PointerEnterEvent>(Container).Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Event<Aggregator.Events.MapCell.PointerLeaveEvent>(Container).Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Event<Aggregator.Events.MapCell.PointerUpEvent>(Container).Invoke(eventData);
        }
    }

}