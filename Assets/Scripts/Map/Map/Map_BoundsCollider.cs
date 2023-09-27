using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine.EventSystems;

namespace Main
{
    [RequireComponent(typeof(Map))]
    public class Map_BoundsCollider: ObjectBehavioursBase
    {
        protected List<BoxCollider2D> iColliderList = new List<BoxCollider2D>();

        [EnabledStateEvent]
        public void RecalcNotifyEvent (Aggregator.Events.Map.CacheRecalcNotifyEvent eventData)
        {
            Map map = (Map)Container;
            Vector2 map_center = map.Common.CachedData.Value.cache_localCenter;
            Rect map_bounds = map.Common.CachedData.Value.cache_localBounds;
            Vector2 map_size = map.Common.CachedData.Value.cache_WorldSize;
            Vector2 half_map_size = map.Common.CachedData.Value.cache_HalfWorldSize;
            Vector2 cell_size = map.Common.CellWorldSize.Value;
            Vector2 half_cell_size = map.Common.CachedData.Value.cache_CellHalfWorldSize;
            
            iColliderList[0].offset = new Vector2(map_bounds.xMin - half_cell_size.x, map_center.y);
            iColliderList[0].size = new Vector2(cell_size.x, map_size.y + cell_size.y * 2f);
            iColliderList[1].offset = new Vector2(map_bounds.xMax + half_cell_size.x, map_center.y);
            iColliderList[1].size = new Vector2(cell_size.x, map_size.y + cell_size.y * 2f);
            iColliderList[2].offset = new Vector2(map_center.x, map_bounds.yMax + half_cell_size.y);
            iColliderList[2].size = new Vector2(map_size.x, cell_size.y);
            iColliderList[3].offset = new Vector2(map_center.x, map_bounds.yMin - half_cell_size.y);
            iColliderList[3].size = new Vector2(map_size.x, cell_size.y);
        }

        protected override void Start()
        {
            base.Start();
            RecalcNotifyEvent(null);
        }

        protected override bool DoEnable()
        {
            if (!base.DoEnable())
                return false;

            var colliders = gameObject.GetComponents<BoxCollider2D>();

            for (int i = 0; i < 4; i++)
            {
                BoxCollider2D collider = null;

                if (i >= colliders.Length)
                {
                    collider = gameObject.AddComponent<BoxCollider2D>();
                    iColliderList.Add(collider);
                }
                else
                {
                    collider = colliders[i];
                    iColliderList.Add(collider);
                }

                collider.enabled = true;
            }


            return true;
        }

        protected override bool DoDisable()
        {
            if (!base.DoDisable())
                return false;
            
            foreach (BoxCollider2D collider in iColliderList)
                collider.enabled = false;

            return true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (BoxCollider2D collider in iColliderList)
            {
                // Detects indirect destroying state of game object
                // This solutions needs for avoid exception: Destroying object multiple times. Don't use DestroyImmediate on the same object in OnDisable or OnDestroy.
                if (gameObject.activeInHierarchy)
#if UNITY_EDITOR
                GameObject.DestroyImmediate(collider);
#else
                GameObject.Destroy(collider);
#endif
            }

            iColliderList.Clear();
        }
    }

}