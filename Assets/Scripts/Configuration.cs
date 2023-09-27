using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Main;
using Main.Events;
using Main.Events.KeyCodePresets;
using Main.Objects;
using Main.Managers;
using Main.Managers.Attributes;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;


namespace Main.Aggregator.Events.Configuration
{
    public class CellTypeToMotionTypeCollisionMapProperty : SharedPropertyEvent<Main.Aggregator.Helpers.MapCell.CellCollisionTypeToMotionTypeCollisionMap>
    {
    }
}
namespace Main.Aggregator.Properties.Configuration
{
    public class CellTypeToMotionTypeCollisionMapProperty : SharedPropertyReference<Main.Aggregator.Helpers.MapCell.CellCollisionTypeToMotionTypeCollisionMap, Main.Aggregator.Events.Configuration.CellTypeToMotionTypeCollisionMapProperty>
    {
        public override string GroupTag => "Game";
        public override string SharedName => "CellTypeToMotionTypeCollisionMap";
    }
}

namespace Main.Aggregator.Events.Configuration.ArmourTypeMap
{
    public class ArmourTypeMapProperty : SharedPropertyEvent<Helpers.Behaviours.Common.Hitpoints.ArmourToDamageMap>
    {
    }
}
namespace Main.Aggregator.Properties.Configuration.ArmourTypeMap
{
    public class ArmourTypeMapProperty : SharedPropertyReference<Helpers.Behaviours.Common.Hitpoints.ArmourToDamageMap, Main.Aggregator.Events.Configuration.ArmourTypeMap.ArmourTypeMapProperty>
    {
        public override string GroupTag => "Game";
        public override string SharedName => "ArmourTypeMap";
    }
}

namespace Main
{

    /// <summary>
    /// Global collector of global shared managers and data also performs the role of global event bus
    /// </summary>
    [DefaultExecutionOrder(KnownExecutionOrder.ConfigurationOrder)]
    public sealed class Configuration : ObjectBehavioursBase
    {
        /* CONSTANTS BLOCK */
        public const int INVALID_ID = 0;
        public const int INVALID_INDEX = -1;
        public static Vector2Int INVALID_MAP_INDEXES { get; } = new Vector2Int(
                                                                            INVALID_INDEX,
                                                                            INVALID_INDEX);

        public const int MAP_CELL_COLLISION_POOL_SIZE = 10;

        public const int OBJECT_MANAGER_START_STORAGE_SIZE = 100;
        public const int OBJECT_MANAGER_STORAGE_SIZE_GROW = 10;

        public const float TARGET_OBJECT_PATH_FIND_CONTROL_INTERVAL = 0.2f;

        public const float MOTION_IDLE_TIME = 1f;
        public const int MOTION_CELL_CHECK_FRAME_SKIP = 5;
        /// <summary>
        /// In angle units
        /// </summary>
        public const float ROTATION_EQUAL_ANGLE_THRESHOLD = 1;


        [SharedProperty]
        public Main.Aggregator.Properties.Configuration.ArmourTypeMap.ArmourTypeMapProperty ArmourTypeMap { get; private set; }

        [SharedProperty]
        public Main.Aggregator.Properties.Configuration.CellTypeToMotionTypeCollisionMapProperty CellTypeToMotionTypeCollisionMapProperty { get; private set; }


        /* KEYBOARD OBSERVERS PRIORITY */
        public Dictionary<System.Type, int> KeyBoardPriority => new Dictionary<System.Type, int>()
        {
            {typeof(Main.Player.InputBehaviour), 0 },
            {typeof(Main.UI.UIControllerBehaviour), 1 },
        };

        private static Configuration fInstance = null;

        public static Configuration GetInstance()
        {
            if (fInstance == null)
            {
                fInstance = GameObject.FindObjectOfType<Configuration>();
                Random.InitState((int)System.DateTime.Now.Ticks);

                if (fInstance == null)
                    throw new System.NullReferenceException($"Could not find a {nameof(Configuration)}");
            }

            if (UnityEngine.Application.isPlaying)
                DontDestroyOnLoad(fInstance);

            return fInstance;
        }

        public static Configuration Instance => GetInstance();

        public static bool IsPaused
        {
            get
            {
                return !Application.isPlaying;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            GetInstance();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (ArmourTypeMap.Value == null)
                ArmourTypeMap.Value = new Aggregator.Helpers.Behaviours.Common.Hitpoints.ArmourToDamageMap();

            if (CellTypeToMotionTypeCollisionMapProperty.Value == null)
                CellTypeToMotionTypeCollisionMapProperty.Value = new Aggregator.Helpers.MapCell.CellCollisionTypeToMotionTypeCollisionMap();

            try
            {
                ArmourTypeMap.Value.AddColumnsRange((Aggregator.Enum.Behaviours.Common.Hitpoints.DamageType[])System.Enum.GetValues(typeof(Aggregator.Enum.Behaviours.Common.Hitpoints.DamageType)));
                ArmourTypeMap.Value.AddRowsRange((Aggregator.Enum.Behaviours.Common.Hitpoints.ArmourType[])System.Enum.GetValues(typeof(Aggregator.Enum.Behaviours.Common.Hitpoints.ArmourType)));
            }
            catch
            {

            }

            try
            {
                CellTypeToMotionTypeCollisionMapProperty.Value.AddColumnsRange((Aggregator.Enum.Behaviours.Movable.MotionType[])System.Enum.GetValues(typeof(Aggregator.Enum.Behaviours.Movable.MotionType)));
                CellTypeToMotionTypeCollisionMapProperty.Value.AddRowsRange((Aggregator.Enum.MapCell.CollisionFlags[])System.Enum.GetValues(typeof(Aggregator.Enum.MapCell.CollisionFlags)));
            }
            catch
            {

            }
        }

    }

}
