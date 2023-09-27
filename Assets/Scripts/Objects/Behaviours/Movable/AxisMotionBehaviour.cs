using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main.Events;

namespace Main.Aggregator.Enum.Behaviours.Movable.AxisMotionBehaviour
{
    public enum AxisDirectionMove
    {
        NoMove = 0,
        MoveLeft,
        MoveTop,
        MoveRight,
        MoveBottom,
    }
}


namespace Main.Aggregator.Events.Behaviours.Movable.AxisMotionBehaviour
{
    public class AxisMovingDirectionProperty : SharedPropertyEvent<Main.Aggregator.Enum.Behaviours.Movable.AxisMotionBehaviour.AxisDirectionMove>
    {
    }
}
namespace Main.Aggregator.Properties.Behaviours.Movable.AxisMotionBehaviour
{
    public class AxisMovingDirectionProperty : SharedEnumProperty<Main.Aggregator.Enum.Behaviours.Movable.AxisMotionBehaviour.AxisDirectionMove, Main.Aggregator.Events.Behaviours.Movable.AxisMotionBehaviour.AxisMovingDirectionProperty>
    {
        public override string GroupTag => "Movable";
        public override string SharedName => "AxisMovingDirection";
    }
}


namespace Main.Objects.Behaviours.Movable
{
    using Main.Aggregator.Enum.Behaviours.Movable.AxisMotionBehaviour;

    public class AxisMotionBehaviour : LinearMovingBehaviour
    {
        public static Dictionary<AxisDirectionMove, Vector2> DirectionMapping = new Dictionary<AxisDirectionMove, Vector2>() {
            { AxisDirectionMove.MoveLeft , new Vector2(-1f, 0f)},
            { AxisDirectionMove.MoveTop , new Vector2(0f, 1f)},
            { AxisDirectionMove.MoveRight , new Vector2(1f, 0f)},
            { AxisDirectionMove.MoveBottom , new Vector2(0f, -1f)},
            { AxisDirectionMove.NoMove, new Vector2(0f, 0f)},
        };


        [SharedProperty]
        public Main.Aggregator.Properties.Behaviours.Movable.AxisMotionBehaviour.AxisMovingDirectionProperty AxisMovingDirection { get; protected set; }

        [SharedPropertyViewer(typeof(Main.Aggregator.Properties.Behaviours.Movable.AxisMotionBehaviour.AxisMovingDirectionProperty))]
        public void AxisMovingDirectionPropertyViewer(Main.Aggregator.Events.Behaviours.Movable.AxisMotionBehaviour.AxisMovingDirectionProperty eventData)
        {
            MovingDirection.Value = DirectionMapping[eventData.PropertyValue];
        }

    }
}
