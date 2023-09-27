using UnityEngine;
using System.Text;
using System.Diagnostics;

namespace Main
{
    [System.Serializable]
    public class MapData : System.Collections.Generic.ValueMap.ValueMapBase<MapCell>
    {
        public static StringBuilder iStringBuffer = new StringBuilder(100);

        public Map Owner { get; set; } = null;
        protected bool iCollectingChilds = false;

        public void CollectUnattachedTransformChilds()
        {
            Aggregator.Properties.Map.CellRootProperty cellsRoot = Owner.SharedProperty<Aggregator.Properties.Map.CellRootProperty>();
            for (int i=0; i<cellsRoot.Value.childCount; i++)
            {
                MapCell cell = cellsRoot.Value.GetChild(i).GetComponent<MapCell>();

                if (cell == null)
                    continue;

                Aggregator.Properties.MapCell.IndexesProperty cellIndexes = cell.SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>();

                if (IsValidPoint(cellIndexes.Value))
                    SetCell(cellIndexes.Value.x, cellIndexes.Value.y, cell);
                else
                    GLog.LogWarning("", $"Could not collect cell for a reason: invalid indexes ({cellIndexes.Value}) ", Owner);
            }
        }

        public override MapCell AllocCell()
        {
            if (iCollectingChilds)
                return null;

            return GameObject.Instantiate(Owner.Common.CellPrefab.Value, Owner.Common.CellRoot.Value);
        }

        public override void SetSize(int col_count, int row_count)
        {
            if ((col_count == fSize.x) &&
                (row_count == fSize.y))
                return;

            Stopwatch measure = new Stopwatch();
            measure.Start();

            try
            {

                iCollectingChilds = true;
                try
                {
                    base.SetSize(col_count, row_count);
                }
                catch (System.Exception e)
                {
                    GLog.LogException(e);
                }
                finally
                {
                    iCollectingChilds = false;
                }

                CollectUnattachedTransformChilds();
                ReallocCells((cell) => { return cell == null; }, true);
            }
            finally
            {
                measure.Stop();
                UnityEngine.Debug.Log($"Set size measured time: {measure.ElapsedMilliseconds}");
            }
        }

        public override void ReleaseCell(ref MapCell cell)
        {
            if (!cell)
                return;

            cell.Release();
            cell.RemoveEventListener(Owner);
#if UNITY_EDITOR
            GameObject.DestroyImmediate(cell.gameObject);
#else
            GameObject.Destroy(cell.gameObject);
#endif
            cell = null;
        }

        public override void RecalcCellPosition(ref MapCell cell, int x, int y)
        {
            if (cell == null)
                return;

         //   if (cell.transform.parent != Owner.Common.CellRoot.Value)
                cell.transform.parent = Owner.Common.CellRoot.Value;

            cell.SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value = new Vector2Int(x, y);
            cell.UpdateCellData();
            cell.Event<Aggregator.Events.MapCell.InvalidatePositionEvent>(Owner).Invoke();
            iStringBuffer.Clear();
            iStringBuffer.Append("cell_x");
            iStringBuffer.Append(x);
            iStringBuffer.Append("_y");
            iStringBuffer.Append(y);
            cell.name = iStringBuffer.ToString();
        }

        public override void AssignCell(MapCell source, ref MapCell destination)
        {
            destination.Assign(source);
        }

        public override void InitCell(MapCell cell)
        {
            if (cell == null)
                return;

            cell.gameObject.SetActive(true);
            cell.AssignSharedProperties(Owner.Common.CellPrefab.Value);
            cell.SharedProperty<Aggregator.Properties.MapCell.OwnerProperty>().Value = Owner;
            cell.Alloc();
            cell.AddEventListener(Owner);
        }
    }

}
