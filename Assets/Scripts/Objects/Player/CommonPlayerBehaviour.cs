using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Main.Events.KeyCodePresets;
using System.Xml.Serialization;
using Main.Events;
using Main.Objects.Behaviours.Attributes;
using Main.Objects;
using Main;
using Main.Player;
using Main.Objects.Behaviours;


namespace Main.Aggregator.Events.Player
{

    public sealed class IsPlayingProperty: SharedPropertyEvent<bool>
    {

    }

    public sealed class DoClearSelection: EventDataBase
    {

    }

    public sealed class OnClearSelection : EventDataBase
    {

    }

    public class DoSelectObject: EventDataBase
    {
        public IBehaviourContainer SelectTarget { get; private set; }
        public bool SelectState { get; private set; }

        public void Invoke(IBehaviourContainer selectTarget, bool selectState)
        {
            SelectTarget = selectTarget;
            SelectState = selectState;
            base.Invoke();
        }
    }

    public sealed class OnSelectObject: DoSelectObject
    {

    }


    public sealed class OwnedObjectsProperty : SharedPropertyEvent<PlayerObjectsRoot>
    {

    }

    public sealed class SelectedObjectsProperty : SharedPropertyEvent<PlayerObjectsRoot>
    {

    }

    public sealed class DisplayNameProperty : SharedPropertyEvent<string>
    {

    }

    public sealed class PlayerTypeProperty : SharedPropertyEvent<PlayerType>
    {

    }
}

namespace Main.Aggregator.Properties.Player
{
    public sealed class IsPlayingProperty : SharedProperty<bool, Events.Player.IsPlayingProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "IsPlaying";
    }


    public sealed class OwnedObjectsProperty : SharedPropertyReference<PlayerObjectsRoot, Aggregator.Events.Player.OwnedObjectsProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "OwnedObjects";
        //public override bool IsReadOnly => true;
    }

    public sealed class SelectedObjectsProperty : SharedPropertyReference<PlayerObjectsRoot, Events.Player.SelectedObjectsProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "SelectedObjects";
        //public override bool IsReadOnly => true;
    }

    public sealed class DisplayNameProperty : SharedPropertyReference<string, Events.Player.DisplayNameProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "DisplayName";
    }

    public sealed class PlayerTypeProperty : SharedEnumProperty<PlayerType, Events.Player.PlayerTypeProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "PlayerType";
    }
}


namespace Main.Aggregator.Events.Player
{
    public class AlliedPlayerListProperty : SharedPropertyEvent<List<PlayerBase>>
    {
    }
}
namespace Main.Aggregator.Properties.Player
{
    public class AlliedPlayerListProperty : SharedPropertyReference<List<PlayerBase>, Main.Aggregator.Events.Player.AlliedPlayerListProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "AlliedPlayerList";
    }
}

namespace Main.Player
{
    /*
     * Player data struct
     *   Key actions preset (default actions takes from default storage)
     *   Harvested food counter <LimitedValueSharedProperty> (Max value takes from global food counter)
     *   Lives counter <LimitedValueSharedProperty> (default max value takes from default storage)
     */

    [Serializable]
    [Unique]
    public class CommonPlayerBehaviour: PlayerBehaviourBase
    {
        [SharedProperty]
        public Aggregator.Properties.Player.IsPlayingProperty IsPlaying { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Player.DisplayNameProperty DisplayName { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Player.OwnedObjectsProperty OwnedObjects { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Player.SelectedObjectsProperty SelectedObjects { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Player.PlayerTypeProperty PlayerType { get; protected set; }
        [SharedProperty(DefaultConstructorIfDefaultReferenceValue = true)]
        public Aggregator.Properties.Player.AlliedPlayerListProperty AlliedPlayerList { get; protected set; }

        [SerializeField]
        protected BehaviourContainer[] Initial_Selected = new BehaviourContainer[0];
        protected bool iInitialized = false;

        protected PlayerObjectsRoot pOwningObjects => (iOwningObjects == null) ? iOwningObjects = new PlayerObjectsRoot(this) : iOwningObjects;
        protected PlayerObjectsRoot pSelectedObjects => (iSelectedObjects == null) ? iSelectedObjects = new PlayerObjectsRoot(this) : iSelectedObjects;

        protected PlayerObjectsRoot iOwningObjects = null;
        protected PlayerObjectsRoot iSelectedObjects = null;

        public bool IsOwningObject(BehaviourContainer obj)
        {
            return OwnedObjects.Value.Contains(obj);
        }

        public bool IsAlliedPlayer(PlayerBase player)
        {
            if (!player)
                return false;

            return (player?.Equals(Container) ?? false) || (AlliedPlayerList.Value.Contains(player));
        }

        public bool IsAlliedObject(BehaviourContainer obj)
        {
            return IsAlliedPlayer(obj?.SharedProperty<Aggregator.Properties.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty>().Value);
        }

        [EnabledStateEvent]
        public void PlayerOwnershipEvent(Aggregator.Events.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty eventData)
        {
            if (eventData.PrevValue?.Equals(PlayerBase) ?? false)
            {
                pOwningObjects.Remove(eventData.Sender as IBehaviourContainer);
            } 
            
            if (eventData.PropertyValue?.Equals(PlayerBase) ?? false)
            {
                pOwningObjects.Add(eventData.Sender as IBehaviourContainer);
            }
        }

        [EnabledStateEvent]
        public void PlayerObjectSelectionEvent(Aggregator.Events.Behaviours.Common.PlayerSelectableBehaviour.OnSelectByPlayerEvent eventData)
        {
            IBehaviourContainer obj = eventData.Sender as IBehaviourContainer;

            if (eventData.SelectState)
            {
                if (!IsOwningObject(obj.SharedProperty<Aggregator.Properties.Behaviours.Common.PlayerOwnershipBehaviour.OwningPlayerProperty>().Value))
                    ClearSelection();

                if (!pSelectedObjects.Contains(obj))
                    pSelectedObjects.Add(obj);
                else
                    // Prevents callback event if obj is already selected
                    return;
            }
            else
            {
                if (!pSelectedObjects.Remove(obj))
                // Prevents callback event if obj is already unselected
                    return;
            }

            Event<Aggregator.Events.Player.OnSelectObject>(Container).Invoke(obj, eventData.SelectState);
        }

        [EnabledStateEvent]
        public void DoSelectObject(Aggregator.Events.Player.DoSelectObject eventData)
        {
            eventData.SelectTarget.Event<Aggregator.Events.Behaviours.Common.PlayerSelectableBehaviour.DoSelectByPlayerEvent>(Container).Invoke(PlayerBase as PlayerBase, eventData.SelectState);
        }

        public void ClearSelection()
        {
            if (pSelectedObjects.Count == 0)
                return;

            foreach (BehaviourContainer selected in pSelectedObjects)
            {
                selected.Event<Aggregator.Events.Behaviours.Common.PlayerSelectableBehaviour.DoSelectByPlayerEvent>(iContainer).Invoke(PlayerBase as PlayerBase, false);
            }

            pSelectedObjects.Clear();

            Event<Aggregator.Events.Player.OnClearSelection>(Container).Invoke();
        }

        protected override void Awake()
        {
            base.Awake();
            OwnedObjects.ReadonlyValueProvider = () => { return pOwningObjects; };
            SelectedObjects.ReadonlyValueProvider = () => { return pSelectedObjects; };
        }

        protected void Update()
        {
            if (!iInitialized)
            {
                iInitialized = true;

                foreach (var selectable in Initial_Selected)
                    Event<Aggregator.Events.Player.DoSelectObject>(Container).Invoke(selectable, true);
            }
        }
    }
}
