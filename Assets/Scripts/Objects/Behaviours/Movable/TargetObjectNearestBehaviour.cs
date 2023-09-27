using Main;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine;
using System.Collections.Generic;


namespace Main.Aggregator.Events.Behaviours.Movable
{
    public class TargetObjectForNearestPathProperty : SharedPropertyEvent<BehaviourContainer>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable
{
    public class TargetObjectForNearestPathProperty : SharedPropertyReference<BehaviourContainer, Main.Aggregator.Events.Behaviours.Movable.TargetObjectForNearestPathProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "TargetObjectForNearestPath";
    }
}


namespace Main.Objects.Behaviours.Movable
{
    public class TargetObjectNearestBehaviour : TargetPointNearestPathMotionBehaviour
    {

        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.TargetObjectForNearestPathProperty TargetObjectForNearestPath { get; protected set; }

        protected Vector2Int iTargetPrevPosition = Vector2Int.zero;
        protected Coroutine iTargetController = null;

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Movable.TargetObjectForNearestPathProperty))]
        public void TargetObjectForNearestPathPropertyViewer(Main.Aggregator.Events.Behaviours.Movable.TargetObjectForNearestPathProperty eventData)
        {
            iTargetPrevPosition = new Vector2Int(-1, -1);
        }


        public override void OnDoSetTargetForMotion(Main.Aggregator.Events.Behaviours.Movable.DoSetTargetForMotion eventData)
        {
            if (eventData.PropertyValue is BehaviourContainer)
            {
                TargetObjectForNearestPath.Value = eventData.PropertyValue as BehaviourContainer;
                return;
            }

            base.OnDoSetTargetForMotion(eventData);
        }

        protected System.Collections.IEnumerator TargetController()
        {
            while (true)
            {
                yield return new WaitForSeconds(Configuration.TARGET_OBJECT_PATH_FIND_CONTROL_INTERVAL);

                if (TargetObjectForNearestPath.Value != null)
                {
                    Vector2Int curTargetPos = TargetObjectForNearestPath.Value.SharedProperty<Aggregator.Properties.Behaviours.Movable.MapPositionProperty>().Value;
                    if (curTargetPos != iTargetPrevPosition)
                    {
                        if (TargetPointForNearestPath.Value == curTargetPos)
                            TargetPointForNearestPath.DirtyValue();
                        else
                            TargetPointForNearestPath.Value = curTargetPos;

                        iTargetPrevPosition = curTargetPos;
                    }
                }
            }
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            iTargetController = StartCoroutine(TargetController());
            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            StopCoroutine(iTargetController);
            return true;
        }

    }
}
