using System;
using System.Collections;
using System.Collections.Generic;
using Main;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;
using UnityEngine;

namespace Main.MapExt
{

    [RequireComponent(typeof(Map))]
    [RequireComponent(typeof(Map_Common))]
    [RequireEnabledBehaviour(typeof(Map_Common))]
    public class Map_PathFind: ObjectBehavioursBase
    {
        protected Map icachedMap = null;
        protected MapCell[] iSiblings = new MapCell[Map_Common.CELL_SIBLINGS_COUNT];

        public static Vector2Int[] PathBuffer { get => iPathBuffer; }
        public static int PathBufferLength { get => iPathBufferLength; }

        protected static Path_Cell[,] iPathMapBuffer = null;
        //protected static List<Vector2Int> iOpenCellsBuffer = null;
        protected static Vector2Int[] iPathBuffer = null;
        protected static int iPathBufferLength = 0;

        protected static LinkedListEx<Path_Cell2> iExploredCellsBuffer = new LinkedListEx<Path_Cell2>(CellComparer);

        public struct Path_Cell
        {
            public Vector2Int CellPos;
            public int cost;
            public float dist_to_start;
            public float sum_dist;
            public int write_cnt;

            public Path_Cell(Vector2Int cellPos, int ACost, float ADist_to_start, float ASum_dist, int AWrite_Cnt)
            {
                CellPos = cellPos;
                cost = ACost;
                dist_to_start = ADist_to_start;
                sum_dist = ASum_dist;
                write_cnt = AWrite_Cnt;
            }
        }


        public class Path_Cell2
        {
            public Vector2Int CellPos;
            public Vector2Int CameFromPos;
            public float CostToStart;
            public float HeurCost;
            public float EstimatedCost{ get => CostToStart + HeurCost; }

            public Path_Cell2(Vector2Int cellPos, Vector2Int cameFrom, float costToStart = 0, float heurCost = 0)
            {
                CellPos = cellPos;
                CameFromPos = cameFrom;
                CostToStart = costToStart;
                HeurCost= heurCost;
            }

            public override int GetHashCode()
            {
                return CellPos.x ^ CellPos.y;
            }
        }


        public class CellPathData
        {
            public MapCell cellPointer = null;
            public CellPathData CameFrom = null;
            public Vector2Int cellMapIndexes = Vector2Int.zero;
            public int PathAtStart = -1;
            public int HeuristicCost = -1;

            public int OpenCellSessionTicket = 0;
            public int ClosedCellSessionTicket = 0;

            public CellPathData(CellPathData cameFrom, Vector2Int cell_map_indexes, int pathAtStart, int heuristic_cost)
            {
                CameFrom = cameFrom;
                //cellPointer = cell_pointer;
                cellMapIndexes = cell_map_indexes;
                PathAtStart = pathAtStart;
                HeuristicCost = heuristic_cost;
            }

            public int EstimatedFullPath => (PathAtStart + HeuristicCost);
        }

        private const int PATH_MAP_EMPTY_CELL = -3;
        private const int PATH_MAP_START_CELL = -2;
        private const int PATH_MAP_END_CELL = -1;
        private const int PATH_MAP_BUSY_CELL = 0;

        protected static int CellComparer(Path_Cell2 val1, Path_Cell2 val2)
        {
            return (val2.HeurCost - val1.HeurCost >= 0f) ? 1 : -1;
        }

        public bool IsMovableCell(MapCell cell)
        {
            return (cell != null);// && (cell.SharedProperty<Aggregator.Properties.MapCell.CollisionFlagsProperty>().Value.HasFlag(Aggregator.Enum.MapCell.CollisionFlags.Ground));
        }

        public int HeurValue(Vector2Int at, Vector2Int to)
        {
            return (Mathf.Abs(to.x - at.x) + Mathf.Abs(to.y - at.y));
        }

        protected static int CompareCellPathData(CellPathData cell1, CellPathData cell2)
        {
            return (cell1.EstimatedFullPath - cell2.EstimatedFullPath);
        }

        protected LinkedListEx<CellPathData> iOpenCellsPrioList = new LinkedListEx<CellPathData>(CompareCellPathData);
        protected CellPathData[,] iClosedCellsBuffer = new CellPathData[0, 0];
        protected CellPathData[,] iOpenCellsBuffer = new CellPathData[0, 0];
        protected int iClosedCellSessionTicket = 0;
        protected int iOpenCellSessionTicket = 0;

        protected void PrepareFindPath()
        {
            iClosedCellSessionTicket++;

            if (iClosedCellSessionTicket == int.MaxValue)
                iClosedCellSessionTicket = 0;

            iOpenCellSessionTicket++;

            if (iOpenCellSessionTicket == int.MaxValue)
                iOpenCellSessionTicket = 0;

            if (iClosedCellsBuffer.Length < icachedMap.Common.Size.Value.x * icachedMap.Common.Size.Value.y)
                iClosedCellsBuffer = new CellPathData[icachedMap.Common.Size.Value.x, icachedMap.Common.Size.Value.y];

            if (iOpenCellsBuffer.Length < icachedMap.Common.Size.Value.x * icachedMap.Common.Size.Value.y)
                iOpenCellsBuffer = new CellPathData[icachedMap.Common.Size.Value.x, icachedMap.Common.Size.Value.y];

            iOpenCellsPrioList.Clear();
        }


        public bool FindPathAlgorithmAStar(Vector2Int start, Vector2Int end, List<Vector2Int> path, Func<MapCell, bool> passableCondition, bool pathAsDirections)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (passableCondition == null)
                throw new ArgumentNullException("passableCondition");

            PrepareFindPath();

            CellPathData current = new CellPathData(null, start, 0, 0);
            iOpenCellsPrioList.AddSorted(current);

            bool finished = false;
            CellPathData finishedNode = null;
            CellPathData nearestNodeToGoal = null;

            while (iOpenCellsPrioList.Count > 0)
            {
                current = iOpenCellsPrioList.First.Value;
                current.ClosedCellSessionTicket = iClosedCellSessionTicket;
                iClosedCellsBuffer[current.cellMapIndexes.x, current.cellMapIndexes.y] = current;
                iOpenCellsPrioList.RemoveFirst();

                if (current.cellMapIndexes == end)
                {
                    finished = true;
                    finishedNode = current;
                    break;
                }

                if (current.EstimatedFullPath < (nearestNodeToGoal?.EstimatedFullPath ?? current.EstimatedFullPath + 1))
                    nearestNodeToGoal = current;

                // Добавление соседей
                foreach (Vector2Int sibling_offset in Map_Common.CELL_SIBLINGS_OFFSETS)
                {
                    Vector2Int next_map_indexes = current.cellMapIndexes + sibling_offset;
                    MapCell cell = icachedMap.Common.GetCellByMapIndexes(next_map_indexes);

                    if (icachedMap.Common.IsValidMapIndexes(next_map_indexes) &&
                        passableCondition(cell))
                    {
                        if ((iClosedCellsBuffer[next_map_indexes.x, next_map_indexes.y]?.ClosedCellSessionTicket ?? iClosedCellSessionTicket + 1) == iClosedCellSessionTicket)
                            continue;

                        CellPathData openMatchNode = null;

                        if ((iOpenCellsBuffer[next_map_indexes.x, next_map_indexes.y]?.OpenCellSessionTicket ?? iOpenCellSessionTicket + 1) == iOpenCellSessionTicket)
                            openMatchNode = iOpenCellsBuffer[next_map_indexes.x, next_map_indexes.y];

                        CellPathData neighbourNode = new CellPathData(current, next_map_indexes, current.PathAtStart + cell.SharedProperty<Aggregator.Properties.MapCell.PathCost>().Value, HeurValue(next_map_indexes, end));
                        neighbourNode.OpenCellSessionTicket = iOpenCellSessionTicket;

                        if ((openMatchNode == null))
                        {
                            iOpenCellsBuffer[next_map_indexes.x, next_map_indexes.y] = neighbourNode;
                            iOpenCellsPrioList.AddSorted(neighbourNode);
                        }
                        else
                        // Корректировка более выгодного пути
                        {
                            if (neighbourNode.EstimatedFullPath < openMatchNode.EstimatedFullPath)
                            {
                                openMatchNode.CameFrom = current;
                                openMatchNode.PathAtStart = neighbourNode.PathAtStart;
                                openMatchNode.HeuristicCost = neighbourNode.HeuristicCost;
                            }
                        }
                    }
                }

            }

            if (finished || (nearestNodeToGoal != null))
            {
                if (!finished)
                    finishedNode = nearestNodeToGoal;

                path.Clear();
                CellPathData node = null;

                // Восстановление пути
                node = finishedNode;
                Vector2Int pathCellPos = Vector2Int.zero;
                Vector2Int prevPathCellPos = node.cellMapIndexes;

                while (node.CameFrom != null)
                {
                    if (pathAsDirections)
                        path.Add(node.cellMapIndexes - (node.CameFrom?.cellMapIndexes ?? node.cellMapIndexes));
                    else
                        path.Add(node.cellMapIndexes);

                    prevPathCellPos = node.cellMapIndexes;
                    node = node.CameFrom;
                }

                path.Reverse();

                return true;
            }

            return false;
        }



        public bool FindPathAlgorithmLi(Vector2Int start, Vector2Int end, Vector2Int[] findedPathBuffer, ref int findedPathLength)
        {
            findedPathLength = 0;

            if (icachedMap.Common.Size.Value == Vector2.zero)
                return false;

            if (!icachedMap.Common.IsValidMapIndexes(start))
                return false;

            end = icachedMap.Common.ValidateMapIndexes(end);

            if (start == end)
                return true;

            bool pathExists = false;
            try
            {

            }
            catch (Exception e)
            {
                Debug.Log($"Raised exception while path find process: {e}");
            }

            return pathExists;
        }

        public bool FindPath(Vector2Int start, Vector2Int end, Path_Cell[,] path_map_buffer, List<Vector2Int> open_cells_buffer, ref Vector2Int[] path, out int out_length, Vector2Int[] excludedCells = null)
        {
            out_length = 0;
            end = icachedMap.Common.ValidateMapIndexes(end);

            if ((start == end) ||
                !icachedMap.Common.IsValidMapIndexes(start))
                return true;

            // Zero path map
            for (int i = 0; i < icachedMap.Common.Size.Value.y; i++)
            {
                for (int j = 0; j < icachedMap.Common.Size.Value.x; j++)
                {
                    path_map_buffer[i, j].cost = PATH_MAP_EMPTY_CELL;
                    path_map_buffer[i, j].sum_dist = -1;
                    path_map_buffer[i, j].dist_to_start = -1;
                    path_map_buffer[i, j].write_cnt = 0;
                }
            }

            if (excludedCells != null)
            {
                for (int i = 0; i < excludedCells.Length; i++)
                    if (icachedMap.Common.IsValidMapIndexes(excludedCells[i]))
                        path_map_buffer[excludedCells[i].y, excludedCells[i].x].cost = PATH_MAP_BUSY_CELL;
            }

            // Wave propagation

            Vector2Int pos = start;
            Vector2Int pos2;
            Vector2Int nearest_point = start;
            float nearest_point_dist = Vector3.Distance(icachedMap.Common.MapIndexesToWorld(start),
                                                        icachedMap.Common.MapIndexesToWorld(end));
            float f_buffer = 0f;
            bool target_achieved = false;

            open_cells_buffer.Clear();
            path_map_buffer[start.y, start.x].cost = PATH_MAP_START_CELL;
            path_map_buffer[end.y, end.x].cost = PATH_MAP_END_CELL;
            open_cells_buffer.Add(pos);

            while (true)
            {
                int c = open_cells_buffer.Count;
                int p = 0;

                // Enum open cells
                for (int i = 0; i < c; i++)
                {
                    pos = open_cells_buffer[i];
                    MapCell current = icachedMap.Common.GetCellByMapIndexes(pos);
                    p++;

                    // Enum neighbourhood cells
                    for (int k = 0; k < Map_Common.CELL_MANH_SIBLINGS_COUNT; k++)
                    {
                        pos2 = pos + Map_Common.CELL_MANH_SIBLINGS_OFFSETS[k];

                        if (!icachedMap.Common.IsValidMapIndexes(pos2))
                            continue;

                        switch (path_map_buffer[pos2.y, pos2.x].cost)
                        {
                            case PATH_MAP_EMPTY_CELL:
                                MapCell cell = icachedMap.Common.GetCellByMapIndexes(pos2);

                                if (IsMovableCell(cell))
                                {
                                    int pcost = path_map_buffer[pos.y, pos.x].cost;

                                    if (pcost == PATH_MAP_START_CELL)
                                    {
                                        pcost = current.SharedProperty<Aggregator.Properties.MapCell.PathCost>().Value;
                                    }

                                    path_map_buffer[pos2.y, pos2.x].cost = pcost + cell.SharedProperty<Aggregator.Properties.MapCell.PathCost>().Value;
                                    open_cells_buffer.Add(pos2);
                                    f_buffer = Vector3.Distance(icachedMap.Common.MapIndexesToWorld(end), cell.transform.position);

                                    if (f_buffer < nearest_point_dist)
                                    {
                                        nearest_point_dist = f_buffer;
                                        nearest_point = pos2;
                                    }
                                }
                                else
                                    path_map_buffer[pos2.y, pos2.x].cost = PATH_MAP_BUSY_CELL;

                                break;
                            case PATH_MAP_START_CELL:
                                break;
                            case PATH_MAP_END_CELL:
                                target_achieved = true;
                                break;
                            case PATH_MAP_BUSY_CELL:
                                break;
                        }

                        if (target_achieved)
                            break;
                    }

                    if (target_achieved)
                        break;
                }

                open_cells_buffer.RemoveRange(0, p);

                if (target_achieved)
                    break;

                if (open_cells_buffer.Count == 0)
                {
                    break;
                }
            }

            //        MapCell_Script ncell = GetMapCell(nearest_point);
            Vector2Int buffer_end = end;

            if ((!target_achieved))
            {
                if (start == nearest_point)
                    return true;
                else
                    end = nearest_point;
            }

            pos = end;
            // Path calculation
            Vector2Int best = pos;
            int best_val = System.Int32.MaxValue;
            float best_dist = System.Single.MaxValue;
            bool path_found = false;
            int it_count = 0;

            if (path != null)
            {
                path[out_length] = end;
                out_length++;
            }

            while (true)
            {
                if (it_count >= (icachedMap.Common.Size.Value.x * icachedMap.Common.Size.Value.y))
                {
                    Debug.LogError("Alllowable number of iterations is exhausted!");
                    return false;
                }

                for (int k = 0; k < Map_Common.CELL_MANH_SIBLINGS_COUNT; k++)
                {
                    it_count++;
                    pos2 = pos + Map_Common.CELL_MANH_SIBLINGS_OFFSETS[k];

                    if (!icachedMap.Common.IsValidMapIndexes(pos2))
                        continue;

                    int path_val = path_map_buffer[pos2.y, pos2.x].cost;
                    float dist = Vector2Int.Distance(start, pos2);

                    switch (path_val)
                    {
                        case PATH_MAP_EMPTY_CELL:
                            break;
                        case PATH_MAP_BUSY_CELL:
                            break;
                        case PATH_MAP_START_CELL:
                            path_found = true;
                            break;
                        case PATH_MAP_END_CELL:
                            break;

                        default:
                            if ((path_val < best_val) || (path_val <= best_val) && (dist < best_dist))
                            {
                                best_val = path_val;
                                best_dist = dist;
                                best = pos2;
                            }

                            break;
                    }
                }

                if (path_found)
                {
                    return buffer_end == end;
                }
                else
                {
                    if (path != null)
                    {
                        path[out_length] = best;
                        out_length++;
                    }
                    pos = best;
                }
            }

        }
        /*
        public bool FindPath(Vector2Int from, Vector2Int to, Vector2Int[] excludedCells = null)
        {
            BuffersNeeded();
            return FindPath(from, to, iPathMapBuffer, iOpenCellsBuffer, ref iPathBuffer, out iPathBufferLength, excludedCells);
        }

        protected void BuffersNeeded()
        {
            Vector2Int msize = icachedMap.Common.Size.Value;
            int total_cells = msize.x * msize.y;

            if ((iPathMapBuffer == null) ||
                (iPathMapBuffer.GetUpperBound(0) != msize.y) ||
                (iPathMapBuffer.GetUpperBound(1) != msize.x))
            {
                iPathMapBuffer = new Path_Cell[msize.y, msize.x];
            }

            if ((iOpenCellsBuffer == null) ||
                (iOpenCellsBuffer.Capacity < total_cells))
            {
                if (iOpenCellsBuffer == null)
                    iOpenCellsBuffer = new List<Vector2Int>(total_cells);
                else
                    iOpenCellsBuffer.Capacity = total_cells;
            }

            if ((iPathBuffer == null))
            {
                iPathBuffer = new Vector2Int[total_cells];
            }
        }

        /// <summary>
        /// For debug only
        /// </summary>
        /// <returns></returns>
        public IEnumerator FindPath()
        {
            Path_Cell[,] path_map = new Path_Cell[icachedMap.Common.Size.Value.y, icachedMap.Common.Size.Value.x];
            List<Vector2Int> open_cells_buffer = new List<Vector2Int>(icachedMap.Common.Size.Value.y * icachedMap.Common.Size.Value.x);
            Vector2Int[] path_buffer = new Vector2Int[icachedMap.Common.Size.Value.y * icachedMap.Common.Size.Value.x];


            Vector2Int at = Vector2Int.zero;
            Vector2Int to = Vector2Int.zero;
            MapCell[] cells = null;

            if (cells.Length > 0)
                at = cells[0].SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value;
            else
            {
                GLog.Log("Start cell is not specified");
                yield break;
            }

            if (cells.Length > 0)
                to = cells[0].SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value;
            else
            {
                GLog.Log("Stop cell is not specified");
                yield break;
            }

            //FindPath(at, to, path_map, open_cells_buffer, path_buffer, out path_len);
            FindPath(at, to);

            // Zero path map
            for (int i = 0; i < icachedMap.Common.Size.Value.y; i++)
            {
                for (int j = 0; j < icachedMap.Common.Size.Value.x; j++)
                {
                    switch (path_map[i, j].cost)
                    {
                        case PATH_MAP_START_CELL:
                            break;
                        case PATH_MAP_END_CELL:
                            break;
                        case PATH_MAP_EMPTY_CELL:
                            break;
                        case PATH_MAP_BUSY_CELL:
                            break;
                    }
                }
            }

            for (int i=0; i<PathBufferLength; i++)
            {
                MapCell cell = icachedMap.Common.GetCellByMapIndexes(PathBuffer[i]);
            }

            yield break;
        }*/

        protected override void Awake()
        {
            base.Awake();
            icachedMap = (Map)Container;
        }
    }
}


/*

        public bool ExploredInLoop(PathCellList cells, PathCell target)
        {
            LinkedListNode<PathCell> node = cells.First;

            while (node != null)
            {
                for (int i = 0; i < Map.CELL_MANH_SIBLINGS_OFFSETS.Length; i++)
                {
                    Vector2Int cell_indexes = node.Value.Cell + Map.CELL_MANH_SIBLINGS_OFFSETS[i];
                    Map_Cell cell = icachedMap.Common.GetCellByMapIndexes(cell_indexes);

                    if (cell == null) continue;

                    PathCell pcell = new PathCell(cell_indexes, node.Value, cell.MoveCost, target);

                    if (pcell.HeuristicRemainCost > node.Value.HeuristicRemainCost) continue;

                    if (IsMovableCell(cell))
                    {
                        return false;
                    }
                    else if (!cell.GetComponent<Map_Cell_PathFinderTool>().GetState(Map_Cell_PathFinderTool.STATE_EXPLORED))
                    {
                        return true;
                    }
                }

                node = node.Next;
            }

            return false;
        }

        public IEnumerator FindPath(Vector2Int at, Vector2Int to, Vector2Int[] finded_path, int count, IEnumerator stepper)
        {
            finded_path = new Vector2Int[icachedMap.Common.Size.x * icachedMap.Common.Size.y + 1];
            count = 0;
            
            DeactivateState(Map_Cell_PathFinderTool.STATE_EXPLORED);
            DeactivateState(Map_Cell_PathFinderTool.STATE_REACHABLE);
            DeactivateState(Map_Cell_PathFinderTool.STATE_WAYPOINT);
            Map_Cell_PathFinderTool tool = null;

            tool = icachedMap.Common.GetCellByMapIndexes(at).GetComponent<Map_Cell_PathFinderTool>();
            tool.SetState(Map_Cell_PathFinderTool.STATE_START, true);
            tool = icachedMap.Common.GetCellByMapIndexes(to).GetComponent<Map_Cell_PathFinderTool>();
            tool.SetState(Map_Cell_PathFinderTool.STATE_STOP, true);
            yield return stepper;

            PathCell atData = new PathCell(at);
            PathCell toData = new PathCell(to);
            PathCellList ReachableCells = new PathCellList();
            PathCellList ExploredCells = new PathCellList();
            ReachableCells.AddLast(new PathCell(at));

            while (ReachableCells.Count > 0)
            {
                LinkedListNode<PathCell> currentNode = ReachableCells.First;
                PathCell current = currentNode.Value;
                ReachableCells.Remove(currentNode);
                tool = icachedMap.Common.GetCellByMapIndexes(current.Cell).GetComponent<Map_Cell_PathFinderTool>();
                tool.SetState(Map_Cell_PathFinderTool.STATE_REACHABLE, false);
                tool.SetState(Map_Cell_PathFinderTool.STATE_EXPLORED, true);
                ExploredCells.AddSorted(current, (PathCell cmp1, PathCell cmp2) => { return cmp1.HeuristicRemainCost - cmp2.HeuristicRemainCost; });

                if (MathKit.Vectors2DIntEquals(current.Cell, to))
                {
                    toData.Parent = current;
                    break;
                }

                if (ExploredInLoop(ExploredCells, toData))
                {
                    toData = ExploredCells.First.Value;
                    break;
                }

                Vector2Int cell_indexes = Vector2Int.zero;

                for (int i = 0; i < Map.CELL_MANH_SIBLINGS_OFFSETS.Length; i++)
                {
                    cell_indexes.x = current.Cell.x + Map.CELL_MANH_SIBLINGS_OFFSETS[i].x;
                    cell_indexes.y = current.Cell.y + Map.CELL_MANH_SIBLINGS_OFFSETS[i].y;
                    Map_Cell cell = icachedMap.Common.GetCellByMapIndexes(cell_indexes);

                    if (!IsMovableCell(cell) || ExploredCells.Contains(cell_indexes))
                        continue;

                    LinkedListNode<PathCell> siblingNode = ReachableCells.Find(cell_indexes);
                    PathCell siblingData = siblingNode?.Value;

                    if (siblingData != null)
                    {
                        PathCell temp_parent = siblingData.Parent;
                        int temp_cost = siblingData.TotalCost;
                        siblingData.Parent = current;

                        if (siblingData.TotalCost >= temp_cost)
                        {
                            siblingData.Parent = temp_parent;
                        }

                        tool = cell.GetComponent<Map_Cell_PathFinderTool>();
                        tool.CellCost = cell.MoveCost;
                        tool.ElapsedCost = siblingData.ElapsedCost;
                        tool.HeuristicRemainCost = siblingData.HeuristicRemainCost;
                        tool.TotalCost = siblingData.TotalCost;
                    }
                    else
                    {
                        PathCell pcell = new PathCell(cell_indexes, current, cell.MoveCost, toData);
                        ReachableCells.AddSorted(pcell,(PathCell cmp1, PathCell cmp2) => { return cmp1.TotalCost - cmp2.TotalCost; });
                        tool = cell.GetComponent<Map_Cell_PathFinderTool>();
                        tool.SetState(Map_Cell_PathFinderTool.STATE_REACHABLE, true);
                        tool.CellCost = cell.MoveCost;
                        tool.ElapsedCost = pcell.ElapsedCost;
                        tool.HeuristicRemainCost = pcell.HeuristicRemainCost;
                        tool.TotalCost = pcell.TotalCost;
                    }

                }

                yield return stepper;
            }

            PathCell cellData = toData;
            count = 0;

            while (cellData != null)
            {
                finded_path[count] = cellData.Cell;
                tool = icachedMap.Common.GetCellByMapIndexes(cellData.Cell).GetComponent<Map_Cell_PathFinderTool>();
                tool.SetState(Map_Cell_PathFinderTool.STATE_WAYPOINT, true);
                count++;
                cellData = cellData.Parent;
            }
            yield return stepper;

            yield break;
        }
 */