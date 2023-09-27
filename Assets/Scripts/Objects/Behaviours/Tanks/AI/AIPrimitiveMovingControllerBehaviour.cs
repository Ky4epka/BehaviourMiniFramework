using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;


namespace Main.Aggregator.Events.Behaviours.Tanks.AI
{
    public class ObstacleReactionProperty : SharedPropertyEvent<float>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Tanks.AI
{
    public class ObstacleReactionProperty : SharedProperty<float, Main.Aggregator.Events.Behaviours.Tanks.AI.ObstacleReactionProperty>
    {
        public override string GroupTag => "Tanks.AI";
        public override string SharedName => "ObstacleReaction";
    }
}

namespace Main.Objects.Behaviours.Tanks.AI
{

    [RequireComponent(typeof(Main.Objects.Behaviours.Movable.LinearMovingBehaviour))]
    public class AIPrimitiveMovingControllerBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Movable.LinearMovingBehaviour.MovingDirectionProperty MovingDirection { get; protected set; }
        [SharedProperty]
        public Aggregator.Properties.Behaviours.Tanks.AI.ObstacleReactionProperty ObstacleReaction { get; protected set; }

        protected Coroutine iObstacleTimer = null;
        protected Vector2 iPositionTracker = Vector2.zero;
        protected float iElapsedTime = 0f;

        [SharedPropertyViewer(typeof(Aggregator.Properties.Behaviours.Movable.PositionProperty))]
        public void PositionPropertyViewer(Aggregator.Events.Behaviours.Movable.PositionProperty eventData)
        {
            if (!MathKit.Vectors2DEquals(eventData.PropertyValue, iPositionTracker))
            {
                iElapsedTime = 0f;
                iPositionTracker = eventData.PropertyValue;
            }
        }

        protected IEnumerator ObstacleTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);

                iElapsedTime += 0.1f;

                if (iElapsedTime >= ObstacleReaction.Value)
                    TryOtherDirection();
            }
        }

        protected void TryOtherDirection()
        {
            MovingDirection.Value = new Vector2(Random.Range((int)-1, (int)1), Random.Range((int)-1, (int)1));
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            if (iObstacleTimer != null)
                StopCoroutine(iObstacleTimer);

            iObstacleTimer = StartCoroutine(ObstacleTimer());
            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;

            if (iObstacleTimer != null)
                StopCoroutine(iObstacleTimer);

            return true;
        }
    }

}