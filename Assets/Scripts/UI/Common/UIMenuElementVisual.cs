using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Events;
using Main.Events.KeyCodePresets;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.UI;

namespace Main.Aggregator.Events.UI
{
    public sealed class DoAction : EventDataBase
    {

    }

    public sealed class SelectedProperty : SharedPropertyEvent<bool>
    {
    }

    public sealed class VisibilityProperty : SharedPropertyEvent<bool>
    {
    }

    public sealed class PageVisibilityProperty : SharedPropertyEvent<bool>
    {
    }
}

namespace Main.Aggregator.Properties.UI
{
    public sealed class SelectedProperty : SharedProperty<bool, Aggregator.Events.UI.SelectedProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "Selected";
    }


    public sealed class VisibilityProperty : SharedProperty<bool, Aggregator.Events.UI.VisibilityProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "ViewVisibility";
    }

    public sealed class PageVisibilityProperty : SharedProperty<bool, Aggregator.Events.UI.PageVisibilityProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "PageVisibility";
    }
}

namespace Main.UI
{


    public abstract class UIMenuElementVisual : UIElementBehaviourBase, IUIElementBehaviourBase
    {
        [SharedProperty]
        public Aggregator.Properties.UI.SelectedProperty Selected { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.UI.VisibilityProperty Visibility { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.UI.PageVisibilityProperty PageVisibility { get; protected set; }

        [SharedPropertyViewer(typeof(Aggregator.Properties.UI.PageVisibilityProperty))]
        public void PageVisibilityPropertyViewer(Aggregator.Events.UI.PageVisibilityProperty eventData)
        {
            IUINavigation element = ElementBase.FirstChild;

            while (element != null)
            {
                (element as IUIElementBase).SharedProperty<Aggregator.Properties.UI.VisibilityProperty>().Value = eventData.PropertyValue;
                element = element.NextSibling;
            }

        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.UI.SelectedProperty))]
        public void SelectedPropertyViewer(Aggregator.Events.UI.SelectedProperty eventData)
        {
            UpdateUI();
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.UI.VisibilityProperty))]
        public void VisibilityPropertyViewer(Aggregator.Events.UI.VisibilityProperty eventData)
        {
            UpdateUI();
        }

        protected abstract void UpdateUI();
    }
}
