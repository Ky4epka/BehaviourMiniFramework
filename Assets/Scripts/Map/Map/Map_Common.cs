using UnityEngine;
using Main.Managers;
using Main.Events;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using Main;


namespace Main.Aggregator.Events.Map
{
    public sealed class SizeProperty : SharedPropertyEvent<Vector2Int>
    {
        public SizeProperty() : base()
        {
        }
    }

    public sealed class PivotProperty : SharedPropertyEvent<Vector2>
    {
        public PivotProperty() : base()
        {
        }
    }

    public sealed class CellWorldSizeProperty : SharedPropertyEvent<Vector2>
    {
        public CellWorldSizeProperty() : base()
        {
        }
    }

    public sealed class CellWorldSpacingProperty : SharedPropertyEvent<Vector2>
    {
        public CellWorldSpacingProperty() : base()
        {
        }
    }

    public sealed class CacheRecalcNotifyEvent : EventDataBase
    {
        public CacheRecalcNotifyEvent() : base()
        {
        }
    }

    public sealed class CellPrefabProperty : SharedPropertyEvent<Main.MapCell>
    {
        public CellPrefabProperty() : base()
        {
        }
    }

    public sealed class CellRootProperty : SharedPropertyEvent<Transform>
    {
        public CellRootProperty() : base()
        {
        }
    }
}

namespace Main.Aggregator.Properties.Map
{
    public sealed class SizeProperty : SharedProperty<Vector2Int, Main.Aggregator.Events.Map.SizeProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "Size";
    }

    public sealed class PivotProperty : SharedProperty<Vector2, Main.Aggregator.Events.Map.PivotProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "Pivot";
    }

    public sealed class CellWorldSizeProperty : SharedProperty<Vector2, Main.Aggregator.Events.Map.CellWorldSizeProperty>
    {
        public override string GroupTag => "Common";

        public override string SharedName => "CellWorldSize";
    }

    public sealed class CellWorldSpacingProperty : SharedProperty<Vector2, Main.Aggregator.Events.Map.CellWorldSpacingProperty>
    {

        public override string GroupTag => "Common";

        public override string SharedName => "CellWorldSpacing";
    }


    public sealed class CellPrefabProperty : SharedPropertyReference<Main.MapCell, Main.Aggregator.Events.Map.CellPrefabProperty>
    {


        public override string GroupTag => "Common";

        public override string SharedName => "CellPrefab";
    }

    public sealed class CellRootProperty : SharedPropertyReference<Transform, Events.Map.CellRootProperty>
    {


        public override string GroupTag => "Common";

        public override string SharedName => "CellRoot";
    }
}

namespace Main.Aggregator.Events.Map
{
    public class WorldBoundsProperty : SharedPropertyEvent<Rect>
    {
    }
}
namespace Main.Aggregator.Properties.Map
{
    public class WorldBoundsProperty : SharedProperty<Rect, Main.Aggregator.Events.Map.WorldBoundsProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "WorldBounds";
    }
}


namespace Main.Aggregator.Events.Map
{
    public class LocalCenterToWorldProperty : SharedPropertyEvent<Vector3>
    {
    }
}
namespace Main.Aggregator.Properties.Map
{
    public class LocalCenterToWorldProperty : SharedProperty<Vector3, Main.Aggregator.Events.Map.LocalCenterToWorldProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "LocalCenterToWorld";
    }
}


namespace Main.Aggregator.Events.Map
{
    public class CachedDataProperty : SharedPropertyEvent<Map_Common_CachedData>
    {
    }
}
namespace Main.Aggregator.Properties.Map
{
    public class CachedDataProperty : SharedPropertyReference<Map_Common_CachedData, Main.Aggregator.Events.Map.CachedDataProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "CachedData";
    }
}


namespace Main.Aggregator.Events.Map
{
    public class MapDataProperty : SharedPropertyEvent<MapData>
    {
    }
}
namespace Main.Aggregator.Properties.Map
{
    public class MapDataProperty : SharedPropertyReference<MapData, Main.Aggregator.Events.Map.MapDataProperty>
    {
        public override string GroupTag => "Common";
        public override string SharedName => "MapData";
    }
}

namespace Main
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    [System.Serializable]
    public class Map_Config : IConfigurable
    {
        [System.NonSerialized]
        protected Map_Common iTarget = null;

        [SerializeField]
        public int mapSize_x = 0;
        [SerializeField]
        public int mapSize_y = 0;

        [SerializeField]
        public float cellWorldSize_x = 0f;
        [SerializeField]
        public float cellWorldSize_y = 0f;

        [SerializeField]
        public float cellWorldSpacing_x = 0f;
        [SerializeField]
        public float cellWorldSpacing_y = 0f;

        [SerializeField]
        public float Pivot_x = 0f;
        [SerializeField]
        public float Pivot_y = 0f;

        public Map_Config(Map_Common target)
        {
            iTarget = target;
        }

        public void Assign(Map_Config source)
        {
            mapSize_x = source.mapSize_x;
            mapSize_y = source.mapSize_y;
            cellWorldSize_x = source.cellWorldSize_x;
            cellWorldSize_y = source.cellWorldSize_y;
            cellWorldSpacing_x = source.cellWorldSpacing_x;
            cellWorldSpacing_y = source.cellWorldSpacing_y;
            Pivot_x = source.Pivot_x;
            Pivot_y = source.Pivot_y;
        }

        public void PushConfig(System.IO.Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                Map_Config config = formatter.Deserialize(stream) as Map_Config;
                Assign(config);

                iTarget.CellWorldSize.Value = new Vector2(cellWorldSize_x, cellWorldSize_y);
                iTarget.CellWorldSpacing.Value = new Vector2(cellWorldSpacing_x, cellWorldSpacing_x);
                iTarget.Pivot.Value = new Vector2(Pivot_x, Pivot_y);
                iTarget.Size.Value = new Vector2Int(mapSize_x, mapSize_y);

                for (int i = 0; i < iTarget.MapData.Value.GetRowCount(); i++)
                {
                    for (int j = 0; j < iTarget.MapData.Value.GetColumnCount(); j++)
                    {
                        try
                        {
                            Map_Cell_Config cell_config = new Map_Cell_Config(iTarget.MapData.Value.GetCell(j, i));
                            cell_config.PushConfig(stream);
                        }
                        catch (System.Exception e)
                        {
                            GLog.LogException(e);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                GLog.LogException(e);
            }
        }

        public void LoadFromFile(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                PushConfig(file);
            }
        }

        public void DropConfig(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }

    [System.Serializable]
    public class Map_Cell_Config : IConfigurable
    {
        [System.NonSerialized]
        public MapCell TargetCell = null;
        [SerializeField]
        public Aggregator.Enum.MapCell.CollisionFlags CollisionFlags = 0;
        [SerializeField]
        public Map_Cell_Modificators.Enum Modificators = 0;
        [SerializeField]
        public string SpriteName = string.Empty;


        public Map_Cell_Config(MapCell targetCell)
        {
            TargetCell = targetCell;
        }

        public void Assign(Map_Cell_Config source)
        {
            CollisionFlags = source.CollisionFlags;
            Modificators = source.Modificators;
            SpriteName = source.SpriteName;
        }

        public void DropConfig(System.IO.Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public void PushConfig(System.IO.Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                Map_Cell_Config cell_config = formatter.Deserialize(stream) as Map_Cell_Config;
                Assign(cell_config);
                TargetCell.SharedProperty<Aggregator.Properties.MapCell.CollisionFlagsProperty>().Value = CollisionFlags;
                TargetCell.SharedProperty<Aggregator.Properties.MapCell.ModificatorsProperty>().Value = Modificators;
                TargetCell.SharedProperty<Aggregator.Properties.MapCell.SpriteProperty>().Value = SpriteManager.Instance.Sprites.ItemAt(SpriteName);
            }
            catch (System.Exception e)
            {
                GLog.LogException(e);
            }
        }
    }

    public class Map_Common_CachedData
    {
        public Vector2Int cache_HalfSize { get; set; }  = Vector2Int.zero;
        public Vector2 cache_CellHalfWorldSize { get; set; }  = Vector2.zero;
        public Vector2 cache_WorldSize { get; set; } = Vector2.zero;
        public Vector2 cache_HalfWorldSize { get; set; } = Vector2.zero;
        public Vector3 cache_localPivotPosition { get; set; } = Vector2.zero;
        public Vector2 cache_CellStep { get; set; } = Vector2.zero;
        public Vector2 cache_HalfCellStep { get; set; } = Vector2.zero;
        public Vector2 cache_localCenter { get; set; } = Vector2.zero;
        public Rect cache_localBounds { get; set; } = Rect.zero;
        public Rect cache_WorldBounds { get; set; } = Rect.zero;
    }


    [RequireComponent(typeof(Map))]
    public class Map_Common : ObjectBehavioursBase, System.IEquatable<Map_Common>
    {
        [HideInInspector]
        public float RECALC_CACHE_DELAY_S = 0.1f;

        [SharedProperty]
        public Aggregator.Properties.Map.CellPrefabProperty CellPrefab { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Map.CellRootProperty CellRoot { get; private set; }

        [SharedProperty]
        public Aggregator.Properties.Map.SizeProperty Size { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Map.PivotProperty Pivot { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Map.CellWorldSizeProperty CellWorldSize { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Map.CellWorldSpacingProperty CellWorldSpacing { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Map.WorldBoundsProperty WorldBounds { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Map.LocalCenterToWorldProperty LocalCenterToWorld { get; private set; }
        [SharedProperty(DefaultConstructorIfDefaultReferenceValue = true)]
        public Aggregator.Properties.Map.CachedDataProperty CachedData { get; private set; }
        [SharedProperty]
        public Aggregator.Properties.Map.MapDataProperty MapData { get; private set; }

        public const int CELL_MANH_SIBLINGS_COUNT = 4;
        public static readonly Vector2Int[] CELL_MANH_SIBLINGS_OFFSETS = new Vector2Int[CELL_MANH_SIBLINGS_COUNT] {
                                                                                        new Vector2Int(0, 1),  new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0)
                                                                                        };

        public const int CELL_SIBLINGS_COUNT = 8;
        public static readonly Vector2Int []CELL_SIBLINGS_OFFSETS = new Vector2Int[CELL_SIBLINGS_COUNT] { 
                                                                                        new Vector2Int(-1, 1),  new Vector2Int(0, 1),  new Vector2Int(1, 1),
                                                                                        new Vector2Int(-1, 0),                         new Vector2Int(1, 0),
                                                                                        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
                                                                                        };

        public delegate bool SiblingFilter(Map owner, MapCell match_cell, Vector2Int cell_map_indexes, object param);
        public delegate void SiblingAction(Map owner, MapCell match_cell, Vector2Int cell_map_indexes, object param);

        protected float icache_RecalcDelayElapsed = 0f;
        protected bool icache_RecalcNeeded = false;

        protected float iInvalidateCellsPositionDelayElapsed = 0f;
        protected bool iInvalidateCellsPositionNeeded = false;

        [SerializeField]
        [HideInInspector]
        protected MapData iMapData;

        [SharedPropertyViewer(typeof(Aggregator.Properties.Map.SizeProperty))]
        public void SizeViewer(Aggregator.Events.Map.SizeProperty eventData)
        {
            MapData.Value.SetSize(eventData.PropertyValue);
            RecalcCacheNeeded();
            InvalidateCellsNeeded();
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Map.PivotProperty))]
        public void PivotViewer(Aggregator.Events.Map.PivotProperty eventData)
        {
            RecalcCacheNeeded();
            InvalidateCellsNeeded();
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Map.CellWorldSizeProperty))]
        public void CellWorldSizeViewer(Aggregator.Events.Map.CellWorldSizeProperty eventData)
        {
            RecalcCacheNeeded();
            InvalidateCellsNeeded();
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Map.CellWorldSpacingProperty))]
        public void CellWorldSpacingViewer(Aggregator.Events.Map.CellWorldSpacingProperty eventData)
        {
            RecalcCacheNeeded();
            InvalidateCellsNeeded();
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Map.CellRootProperty))]
        public void CellRootViewer(Aggregator.Events.Map.CellRootProperty eventData)
        {
            RecalcCacheNeeded();
            InvalidateCellsNeeded();
        }

        [SharedPropertyViewer(typeof(Aggregator.Properties.Map.CellPrefabProperty))]
        public void CellPrefabViewer(Aggregator.Events.Map.CellPrefabProperty eventData)
        {
            RecalcCacheNeeded();
            InvalidateCellsNeeded();
        }

        public void MapIndexesToLocal(int x, int y, ref float out_x, ref float out_y)
        {
            out_x = (float)x * CachedData.Value.cache_CellStep.x + CachedData.Value.cache_localPivotPosition.x;
            out_y = (float)y * CachedData.Value.cache_CellStep.y + CachedData.Value.cache_localPivotPosition.y;
        }

        public Vector2 MapIndexesToLocal(int x, int y)
        {
            Vector2 result=Vector2.zero;
            MapIndexesToLocal(x, y, ref result.x, ref result.y);
            return result;
        }

        public Vector2 MapIndexesToLocal(Vector2Int indexes)
        {
            return MapIndexesToLocal(indexes.x, indexes.y);
        }

        public void MapIndexesToLocalCellCenter(int x, int y, ref float out_x, ref float out_y)
        {
            out_x = (float)x * CachedData.Value.cache_CellStep.x + CachedData.Value.cache_CellHalfWorldSize.x + CachedData.Value.cache_localPivotPosition.x;
            out_y = (float)y * CachedData.Value.cache_CellStep.y + CachedData.Value.cache_CellHalfWorldSize.y + CachedData.Value.cache_localPivotPosition.y;
        }

        public Vector2 MapIndexesToLocalCellCenter(Vector2Int indexes)
        {
            Vector2 result = Vector2.zero;
            MapIndexesToLocalCellCenter(indexes.x, indexes.y, ref result.x, ref result.y);
            return result;
        }

        public Vector3 MapIndexesToWorldCellCenter(Vector2Int indexes)
        {
            return LocalToWorld(MapIndexesToLocalCellCenter(indexes));
        }

        public void LocalToMapIndexes(float x, float y, ref int out_x, ref int out_y)
        {// - icache_localPivotPosition.x
         // - icache_localPivotPosition.y
            out_x = (CachedData.Value.cache_CellStep.x == 0) ? 0 : (int)((x - CachedData.Value.cache_localPivotPosition.x) / (CachedData.Value.cache_CellStep.x));
            out_y = (CachedData.Value.cache_CellStep.y == 0) ? 0 : (int)((y - CachedData.Value.cache_localPivotPosition.y) / (CachedData.Value.cache_CellStep.y));
        }

        public Vector2Int LocalToMapIndexes(float x, float y)
        {
            int result_x = 0;
            int result_y = 0;
            LocalToMapIndexes(x, y, ref result_x, ref result_y);
            return new Vector2Int(result_x, result_y);
        }

        public Vector2Int LocalToMapIndexes(Vector2 local)
        {
            return LocalToMapIndexes(local.x, local.y);
        }

        public Vector3 MapIndexesToWorld(int x, int y)
        {
            Vector3 map_world = transform.position;
            Vector3 result;
            result.x = 0;
            result.y = 0;
            result.z = 0;
            MapIndexesToLocal(x, y, ref result.x, ref result.y);
            result.x += map_world.x;
            result.y += map_world.y;
            return result;
        }

        public Vector3 MapIndexesToWorld(Vector2Int indexes)
        {
            return MapIndexesToWorld(indexes.x, indexes.y);
        }

        public Vector3 LocalToWorld(float x, float y)
        {
            Vector3 map_world = transform.position;
            Vector3 result;
            result.x = x + map_world.x;
            result.y = y + map_world.y;
            result.z = 0;
            return result;
        }

        public Vector3 LocalToWorld(Vector2 local)
        {
            return LocalToWorld(local.x, local.y);
        }

        public Vector2Int WorldToMapIndexes(Vector3 world)
        {
            return LocalToMapIndexes(world - transform.position);
        }

        public Vector2 WorldToLocal(Vector3 world)
        {
            return world - transform.position;
        }

        public Vector3 WorldToWorldCellCenter(Vector3 world)
        {
            return MapIndexesToWorldCellCenter(WorldToMapIndexes(world));
        }

        public bool IsValidMapIndexes(int x, int y)
        {
            return (x >= 0) && (x < Size.Value.x) &&
                   (y >= 0) && (y < Size.Value.y);
        }

        public bool IsValidMapIndexes(Vector2Int indexes)
        {
            return IsValidMapIndexes(indexes.x, indexes.y);
        }
        
        public Vector2Int ValidateMapIndexes(Vector2Int pos)
        {
            return MathKit.EnsureVectorRectRange(pos, new RectInt(0, 0, Size.Value.x - 1, Size.Value.y - 1));
        }

        public MapCell GetCellByMapIndexes(int x, int y)
        {
            if (IsValidMapIndexes(x, y))
                return MapData.Value.GetCell(x, y);

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns>Null if indexes is invalid</returns>
        public MapCell GetCellByMapIndexes(Vector2Int indexes)
        {
            return GetCellByMapIndexes(indexes.x, indexes.y);
        }

        public MapCell GetCellByLocal(Vector2 local)
        {
            return GetCellByMapIndexes(LocalToMapIndexes(local));
        }

        public MapCell GetCellByWorld(Vector3 world)
        {
            return GetCellByMapIndexes(WorldToMapIndexes(world));
        }
                      
        public void EnumCellSiblings(Vector2Int at_map_indexes, ref MapCell[] result, ref int count, SiblingFilter filter, object param, bool is_manhattan = false)
        {
            Vector2Int cell_indexes = Vector2Int.zero;
            MapCell match_cell = null;
            count = 0;
            Vector2Int[] offsets;

            if (is_manhattan)
                offsets = CELL_MANH_SIBLINGS_OFFSETS;
            else
                offsets = CELL_SIBLINGS_OFFSETS;

            for (int i = 0; i < offsets.Length; i++)
            {
                cell_indexes.x = at_map_indexes.x + offsets[i].x;
                cell_indexes.y = at_map_indexes.y + offsets[i].y;
                match_cell = GetCellByMapIndexes(cell_indexes);

                if (((filter != null) && (!filter((Map)Container, match_cell, cell_indexes, param)) || (!IsValidMapIndexes(cell_indexes)))) continue;

                result[count++] = match_cell;
            }
        }

        protected void RecalcCacheNeeded()
        {
            icache_RecalcDelayElapsed = 0f;
            icache_RecalcNeeded = true;
        }

        protected void InvalidateCellsNeeded()
        {
            iInvalidateCellsPositionDelayElapsed = 0f;
            iInvalidateCellsPositionNeeded = true;
        }


        protected void DoRecalcCache()
        {
            CachedData.Value.cache_HalfSize = 
                new Vector2Int(Size.Value.x / 2, Size.Value.y / 2);
            CachedData.Value.cache_CellHalfWorldSize = 
                new Vector2(
                    CellWorldSize.Value.x / 2.0f, 
                    CellWorldSize.Value.y / 2.0f);
            CachedData.Value.cache_CellStep = 
                new Vector2(
                    CellWorldSize.Value.x + CellWorldSpacing.Value.x * 1.0f, 
                    CellWorldSize.Value.y + CellWorldSpacing.Value.y * 1.0f);
            CachedData.Value.cache_HalfCellStep = 
                new Vector2(
                    CachedData.Value.cache_CellStep.x / 2f,
                    CachedData.Value.cache_CellStep.y / 2f);
            CachedData.Value.cache_WorldSize = 
                new Vector2(
                    (float)Size.Value.x * CachedData.Value.cache_CellStep.x, 
                    (float)Size.Value.y * CachedData.Value.cache_CellStep.y);
            CachedData.Value.cache_HalfWorldSize = 
                new Vector2(
                    CachedData.Value.cache_WorldSize.x / 2.0f,
                    CachedData.Value.cache_WorldSize.y / 2.0f);
            CachedData.Value.cache_localPivotPosition = 
                new Vector3(
                    Pivot.Value.x - CachedData.Value.cache_HalfWorldSize.x,
                    Pivot.Value.y - CachedData.Value.cache_HalfWorldSize.y);
            CachedData.Value.cache_localCenter = 
                CachedData.Value.cache_localPivotPosition + 
                (Vector3)CachedData.Value.cache_HalfWorldSize;
            Rect r = Rect.zero;
            r.min = CachedData.Value.cache_localPivotPosition;
            r.max = (Vector2)CachedData.Value.cache_localPivotPosition + CachedData.Value.cache_WorldSize;
            CachedData.Value.cache_localBounds = r;
            r.position = transform.position;
            CachedData.Value.cache_WorldBounds = r;
            Event<Aggregator.Events.Map.CacheRecalcNotifyEvent>(Container).Invoke();
        }


        protected override void Awake()
        {
            base.Awake();

            if (iMapData == null)
                iMapData = new MapData();//
        }

        public bool Equals(Map_Common other)
        {
            return this == other;
        }

        protected override void Start()
        {
            base.Start();
            RecalcCacheNeeded();
            InvalidateCellsNeeded();
        }

        protected void Update()
        {
            if (icache_RecalcNeeded)
            {
                icache_RecalcDelayElapsed += Time.deltaTime;

                if (icache_RecalcDelayElapsed > RECALC_CACHE_DELAY_S)
                {
                    icache_RecalcNeeded = false;
                    DoRecalcCache();
                }
            }

            if (iInvalidateCellsPositionNeeded)
            {
                iInvalidateCellsPositionDelayElapsed += Time.deltaTime;

                if (iInvalidateCellsPositionDelayElapsed > RECALC_CACHE_DELAY_S)
                {
                    iInvalidateCellsPositionNeeded = false;
                    MapData.Value.InvalidateCellPositions();
                }
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            MapData.ReadonlyValueProvider = () => iMapData;
            MapData.Value.Owner = (Map)Container;

            WorldBounds.ReadonlyValueProvider = () => {
                return CachedData.Value.cache_WorldBounds;
            };
            LocalCenterToWorld.ReadonlyValueProvider = () => {
                return (Vector3)CachedData.Value.cache_localCenter + transform.position;
            };
            //1ы1ss
            Objects.ISharedPropertyReference<Transform> root = Container.SharedProperty<Aggregator.Properties.Map.CellRootProperty>();

            if (!root.Value)
                root.Value = transform;

            Objects.ISharedPropertyReference<MapCell> prefab = Container.SharedProperty<Aggregator.Properties.Map.CellPrefabProperty>();

            if (!prefab.Value)
            {
                prefab.Value = GetComponentInChildren<MapCell>(true);
            }
        }
    }

}
