using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Events;
using Main.Objects;
using Main.Player;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;


namespace Main.Aggregator.Events.Behaviours.Common.PlayerSelectableBehaviour
{

    public class DoSelectByPlayerEvent: EventDataBase
    {
        public PlayerBase Selector { get; private set; }
        public bool SelectState { get; private set; }

        public void Invoke(PlayerBase selector, bool selectState)
        {
            Selector = selector;
            SelectState = selectState;
            base.Invoke();
        }
    }

    public class OnSelectByPlayerEvent: DoSelectByPlayerEvent
    {

    }
}

namespace Main.Objects.Behaviours.Common
{
    [RequireComponent(typeof(AliveBehaviour))]
    [RequireComponent(typeof(PlayerOwnershipBehaviour))]
    [RequireEnabledBehaviour(typeof(AliveBehaviour))]
    [Unique]
    [EnableOnAlive]
    [DisableOnDie]
    public class PlayerSelectableBehaviour : ObjectBehavioursBase
    {
        protected HashSet<PlayerBase> iSelectors = new HashSet<PlayerBase>();

        [EnabledStateEvent]
        public void SelectByPlayerEvent(Aggregator.Events.Behaviours.Common.PlayerSelectableBehaviour.DoSelectByPlayerEvent eventData)
        {
            if (!eventData.Selector)
                GLog.LogError("Null event param", $"Event param {nameof(eventData.Selector)} is null", this);

            if (eventData.SelectState == IsPlayerSelected(eventData.Selector))
                return;

            DoSelectPlayer(eventData.Selector, eventData.SelectState);
        }

        public bool IsPlayerSelected(PlayerBase player)
        {
            return iSelectors.Contains(player);
        }

        public void ClearPlayerSelection()
        {
            HashSet<PlayerBase> store = new HashSet<PlayerBase>(iSelectors);

            foreach (PlayerBase player in store)
            {
                if (!player)
                    continue;

                DoSelectPlayer(player, false); 
            }

            iSelectors.Clear();
        }

        protected void DoSelectPlayer(PlayerBase selector, bool selectionState)
        {
            if (selectionState)
            {
                iSelectors.Add(selector);
            }
            else
            {
                iSelectors.Remove(selector);
            }

            Event<Aggregator.
                Events.
                Behaviours.
                Common.
                PlayerSelectableBehaviour.
                OnSelectByPlayerEvent>(Container).
                Invoke(selector, selectionState);
            selector.
                Event<
                    Aggregator.
                    Events.
                    Behaviours.
                    Common.
                    PlayerSelectableBehaviour.
                    OnSelectByPlayerEvent>(Container).
                        Invoke(selector, selectionState);
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            ClearPlayerSelection();
            return true;
        }

    }
}