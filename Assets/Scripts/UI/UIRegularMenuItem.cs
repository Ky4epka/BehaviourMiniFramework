using UnityEngine;
using Main.Events;
using Main.Events.KeyCodePresets;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.UI;
using TMPro;

namespace Main.Aggregator.Events.UI
{
    public abstract class ActionPivotEvent: SharedPropertyEvent<EventDataBase>
    {
    }
}

namespace Main.Aggregator.Events.UI.MainMenu
{
}

namespace Main.Aggregator.Events.UI.UIRegularMenuItem
{

    public sealed class TextLabelProperty: SharedPropertyEvent<TMPro.TextMeshProUGUI>
    {
    }

    public sealed class LabelSelectedColorProperty : SharedPropertyEvent<Color>
    {
    }

    public sealed class LabelNormalColorProperty : SharedPropertyEvent<Color>
    {
    }

    public sealed class ActionEventProperty : SharedPropertyEvent<Aggregator.Events.UI.ActionPivotEvent>
    {
    }

    public sealed class StyleSourceProperty: SharedPropertyEvent<BehaviourContainer>
    {
    }
}

namespace Main.Aggregator.Properties.UI.UIRegularMenuItem
{
    public sealed class TextLabelProperty : SharedPropertyReference<TMPro.TextMeshProUGUI, Aggregator.Events.UI.UIRegularMenuItem.TextLabelProperty>
    {
        public override string GroupTag => "Binding";
        public override string SharedName => "BindedLabel";
    }
    public sealed class LabelSelectedColorProperty : SharedProperty<Color, Aggregator.Events.UI.UIRegularMenuItem.LabelSelectedColorProperty>
    {
        public override string GroupTag => "Visual";
        public override string SharedName => "SelectedColor";
    }

    public sealed class LabelNormalColorProperty : SharedProperty<Color, Aggregator.Events.UI.UIRegularMenuItem.LabelNormalColorProperty>
    {
        public override string GroupTag => "Visual";
        public override string SharedName => "NormalColor";
    }

    public sealed class ActionEventProperty : SharedPropertyReference<Aggregator.Events.UI.ActionPivotEvent, Aggregator.Events.UI.UIRegularMenuItem.ActionEventProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "OutputActionEvent";
    }

    public sealed class StyleSourceProperty : SharedPropertyReference<BehaviourContainer, Aggregator.Events.UI.UIRegularMenuItem.StyleSourceProperty>
    {
        public override string GroupTag => "Style";

        public override string SharedName => "StyleSource";
    }
}

namespace Main.UI
{
    public class UIRegularMenuItem : UIMenuElementVisual
    {
        [SharedProperty]
        public Aggregator.Properties.UI.UIRegularMenuItem.TextLabelProperty TextLabel { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.UI.UIRegularMenuItem.LabelSelectedColorProperty LabelSelectedColor { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.UI.UIRegularMenuItem.LabelNormalColorProperty LabelNormalColor { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.UI.UIRegularMenuItem.ActionEventProperty OutputActionEvent { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.UI.UIRegularMenuItem.StyleSourceProperty StyleSource { get; protected set; }

        [EnabledStateEvent]
        public void DoActionEvent(Aggregator.Events.UI.DoAction eventData)
        {
            ActionHandler();
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.UI.UIRegularMenuItem.StyleSourceProperty))]
        public void StyleSourcePropertyViewer(Aggregator.Events.UI.UIRegularMenuItem.StyleSourceProperty eventData)
        {
            if (eventData.PropertyValue)
            {
                Container.SharedPropertyContainer.MergeSharedProperties(eventData.PropertyValue);
            }
        }

        protected virtual void ActionHandler()
        {
            Debug.Log(gameObject.name);
            if (OutputActionEvent.Value != null)
                Event(OutputActionEvent.Value.GetType(), Container);
        }

        protected override void UpdateUI()
        {
           if (TextLabel.Value)
            {
                TextLabel.Value.color = (Selected.Value) ? LabelSelectedColor.Value : LabelNormalColor.Value;
                TextLabel.Value.enabled = Visibility.Value && enabled;
            }

        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            UpdateUI();
            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            UpdateUI();
            return true;
        }
    }
}