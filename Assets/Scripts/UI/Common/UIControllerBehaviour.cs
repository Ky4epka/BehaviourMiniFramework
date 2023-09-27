using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Events;
using Main.Events.KeyCodePresets;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.UI;
using System;

namespace Main.Aggregator.Events.UI.UIController
{
    public sealed class DoNavigateEvent: EventDataBase
    {
        public UINavigate Value { get; private set; }

        public void Invoke(UINavigate value)
        {
            Value = value;
            base.Invoke();
        }
    }

    public sealed class OnNavigateEvent : EventDataBase
    {
        public UINavigate Value { get; private set; }

        public void Invoke(UINavigate value)
        {
            Value = value;
            base.Invoke();
        }
    }

    public sealed class SelectedElementProperty: SharedPropertyEvent<UIElementBase>
    {
    }
}

namespace Main.Aggregator.Properties.UI.UIController
{
    public sealed class SelectedElementProperty : SharedPropertyReference<UIElementBase, Events.UI.UIController.SelectedElementProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "SelectedElement";
    }
}

namespace Main.UI
{
    public interface IUIController: IUIElementBehaviourBase
    {

    }

    public enum UINavigate
    {
        Unknown = 0,
        Up,
        Down,
        Left,
        Right
    }

    public class UIControllerBehaviour : UIElementBehaviourBase, IUIController
    {
        public KeyActionPreset KeyPreset { get; } = new KeyActionPreset();

        [SharedProperty]
        public Aggregator.Properties.UI.UIController.SelectedElementProperty SelectedElement { get; protected set; }

        [EnabledStateEvent]
        public void DoNavigateEvent(Aggregator.Events.UI.UIController.DoNavigateEvent eventData)
        {
            DoNavigate(eventData.Value);
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.UI.UIController.SelectedElementProperty))]
        public void SelectedElementPropertyViewer(Aggregator.Events.UI.UIController.SelectedElementProperty eventData)
        {
            if (eventData.PrevValue != null)
            {
                eventData.PrevValue.SharedProperty<Aggregator.Properties.UI.SelectedProperty>().Value = false;

                if (!eventData.PrevValue.Parent?.Equals(eventData.PropertyValue.Parent) ?? false)
                {
                    (eventData.PrevValue.Parent as IUIElementBase).SharedProperty<Aggregator.Properties.UI.PageVisibilityProperty>().Value = false;
                }
            }

            if (eventData.PropertyValue != null)
            {
                Aggregator.Properties.UI.SelectedProperty prop = eventData.PropertyValue.SharedProperty<Aggregator.Properties.UI.SelectedProperty>();
                prop.Value = true;
                (eventData.PropertyValue.Parent as IUIElementBase).SharedProperty<Aggregator.Properties.UI.PageVisibilityProperty>().Value = true;

                if (eventData.PropertyValue.Parent.Equals(ElementBase))
                {
                    SelectedElement.Value = eventData.PropertyValue.FirstChild as UIElementBase;
                    eventData.PropertyValue.Event<Aggregator.Events.UI.DoAction>(this.Container).Invoke();
                }
            }
        }

        protected IUIElementBase GetSelectedRecurse(Transform pivot)
        {
            IUIElementBase result = ElementBase.ComponentByTransform<IUIElementBase>(pivot, 0, true,
                (IUIElementBase filtered) =>
                {
                    return 
                        filtered.
                            GetComponent<UIElementBase>()?.
                            SharedProperty<Aggregator.Properties.UI.SelectedProperty>().
                            Value ?? false;
                });


            return result;
        }
        protected bool DoNavigate(UINavigate direction)
        {
            IUIElementBase element = SelectedElement.Value;

            if (element == null)
            {
                element = GetSelectedRecurse(transform);
            }

            if (element == null)
            {
                if (element == null)
                    element = FirstChild as IUIElementBase;

                if (element == null)
                    return false;
            }
            else
            {
                switch (direction)
                {
                    case UINavigate.Up:
                        if (element.PrevSibling == null)
                            element = element.LastSibling as IUIElementBase;
                        else
                            element = element.PrevSibling as IUIElementBase;
                        break;
                    case UINavigate.Down:

                        if (element.NextSibling == null)
                            element = element.FirstSibling as IUIElementBase;
                        else
                            element = element.NextSibling as IUIElementBase;
                        break;
                    case UINavigate.Left:
                        if (element.Parent != null)
                        {
                            element = element.Parent as IUIElementBase;
                        }
                        else
                            element = null;
                        break;
                    case UINavigate.Right:
                        element.Event<Aggregator.Events.UI.DoAction>(Container).Invoke();

                        if (element.FirstChild != null)
                            element = element.FirstChild as IUIElementBase;

                        break;
                }
            }

            Event<Aggregator.Events.UI.UIController.OnNavigateEvent>(Container).Invoke(direction);
            SelectedElement.Value = element as UIElementBase;

            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            KeyPreset.AddAction(new KeyPresets.UIKeyActionNavigateToLeft(this));
            KeyPreset.AddAction(new KeyPresets.UIKeyActionNavigateToRight(this));
            KeyPreset.AddAction(new KeyPresets.UIKeyActionNavigateToDown(this));
            KeyPreset.AddAction(new KeyPresets.UIKeyActionNavigateToUp(this));
            KeyPreset.AddAction(new KeyPresets.UIKeyActionClick(this));
            KeyPreset.AddAction(new KeyPresets.UIKeyActionBack(this));
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            int keybdPrio;

            Configuration.Instance.KeyBoardPriority.TryGetValue(this.GetType(), out keybdPrio);

            KeyPreset.Priority = keybdPrio;
            KeyPreset.Observable = Managers.KeyboardManager.Instance;

            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            KeyPreset.Observable = null;
            return true;
        }

    }

}