using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic.ValueMap
{
    public interface ISerializableAddressableValueMap: UnityEngine.ISerializationCallbackReceiver
    {
        
    }

    [System.Serializable]
    public abstract class AddressableValueMap<ColKey, RowKey, Value> : IAddressableValueMap<ColKey, RowKey, Value>, ISerializableAddressableValueMap
    {
        protected HashSet<ColKey> ColumnList { get => (iColumnList == null) ? iColumnList = new HashSet<ColKey>() : iColumnList; }
        protected Dictionary<RowKey, Dictionary<ColKey, Value>> Map { get => (iMap == null) ? iMap = new Dictionary<RowKey, Dictionary<ColKey, Value>>() : iMap; }

        protected HashSet<ColKey> iColumnList = null;
        protected Dictionary<RowKey, Dictionary<ColKey, Value>> iMap = null;

        [UnityEngine.SerializeField]
        public ColKey[] SerializedColumnKeys = new ColKey[0];
        [UnityEngine.SerializeField]
        public RowKey[] SerializedRowKeys = new RowKey[0];

        [UnityEngine.SerializeField]
        public Value[] SerializedCellsLinear = new Value[0];

        public Value this [RowKey row, ColKey col] 
        {
            get => GetCell(col, row);
            set => SetCell(col, row, value);
        }

        public void Assign(IAssignable source)
        {
            if (!(source is IAddressableValueMap<ColKey, RowKey, Value>))
            {
                throw new InvalidOperationException($"Could not assign {source.GetType().FullName} as {nameof(IAddressableValueMap<ColKey, RowKey, Value>)}");
            }

            var sourceMap = source as IAddressableValueMap<ColKey, RowKey, Value>;
            Clear();
            CopyMapFrom(sourceMap, sourceMap.ColumnKeys(), sourceMap.RowKeys());
        }

        public void AddNewColumn(ColKey to)
        {
            if (ColumnList.Contains(to))
                throw new InvalidOperationException($"Column {to} already exists.");

            ColumnList.Add(to);

            foreach (KeyValuePair<RowKey, Dictionary<ColKey, Value>> keyvalue in Map)
            {
                Value val = AllocCell();
                InitCell(val);
                CellAddressChanged(ref val, to, keyvalue.Key);
                keyvalue.Value.Add(to, val);
            }
        }

        public void AddColumnsRange(IEnumerable<ColKey> columns)
        {
            foreach (var column in columns)
                AddNewColumn(column);
        }

        public void AddNewRow(RowKey to)
        {
            Dictionary<ColKey, Value> Columns;

            if (!Map.TryGetValue(to, out Columns))
            {
                Columns = new Dictionary<ColKey, Value>();
                Map.Add(to, Columns);
            }
            else
                throw new InvalidOperationException($"Unable to add a row {to} for a reason: row already exists");

            foreach (ColKey col in ColumnList)
            {
                Value val = AllocCell();
                InitCell(val);
                CellAddressChanged(ref val, col, to);
                Columns.Add(col, val);
            }
        }
        public void AddRowsRange(IEnumerable<RowKey> rows)
        {
            foreach (var row in rows)
                AddNewRow(row);
        }

        public void Clear()
        {
            var keys = new List<RowKey>(Map.Keys);

            foreach (var keyval in keys)
            {
                DeleteRow(keyval);
            }

            Map.Clear();
            ColumnList.Clear();
        }


        public void CopyColumnFrom(ColKey column, IAddressableValueMap<ColKey, RowKey, Value> from, IEnumerable<RowKey> rowKeys = null)
        {
            if (rowKeys == null)
                rowKeys = Map.Keys;

            if (!IsValidColumn(column))
                AddNewColumn(column);

            foreach (var row in rowKeys)
            {
                if (!IsValidRow(row))
                    AddNewRow(row);

                Value val = Map[row][column];
                AssignCell(from[row, column], ref val);
                Map[row][column] = val;
            }
        }

        public void CopyMapFrom(IAddressableValueMap<ColKey, RowKey, Value> from, IEnumerable<ColKey> colKeys = null, IEnumerable<RowKey> rowKeys = null)
        {
            foreach (var col in colKeys)
                if (!IsValidColumn(col))
                    AddNewColumn(col);

            foreach (var row in rowKeys)
            {
                if (!IsValidRow(row))
                    AddNewRow(row);

                foreach (var col in colKeys)
                {
                    Value val = Map[row][col];
                    AssignCell(from[row, col], ref val);
                    Map[row][col] = val;
                }
            }
        }

        public void CopyRowFrom(RowKey row, IAddressableValueMap<ColKey, RowKey, Value> from, IEnumerable<ColKey> colKeys = null)
        {
            if (colKeys == null)
                colKeys = ColumnList;

            if (!IsValidRow(row))
                AddNewRow(row);

            foreach (var col in colKeys)
            {
                if (!IsValidColumn(col))
                    AddNewColumn(col);

                Value val = Map[row][col];
                AssignCell(from[row, col], ref val);
                Map[row][col] = val;
            }
        }

        public void DeleteColumn(ColKey from)
        {
            if (ColumnList.Contains(from))
            {
                foreach (var keyvalue in Map)
                {
                    Value val = keyvalue.Value[from];
                    ReleaseCell(ref val);
                    keyvalue.Value.Remove(from);
                }

                ColumnList.Remove(from);
            }
        }

        public void DeleteRow(RowKey from)
        {
            Dictionary<ColKey, Value> row;

            if (Map.TryGetValue(from, out row))
            {
                foreach (var keyvalue in row)
                {
                    Value val = keyvalue.Value;
                    ReleaseCell(ref val);
                }

                row.Clear();
                Map.Remove(from);
            }
        }

        public void Dispose()
        {
            Clear();
        }

        public Value GetCell(ColKey column, RowKey row)
        {
            return Map[row][column];
        }

        public void SetCell(ColKey col, RowKey row, Value val)
        {
            Map[row][col] = val;
        }

        public int GetColumnCount()
        {
            return ColumnList.Count;
        }

        public int GetRowCount()
        {
            return Map.Count;
        }

        public IEnumerable<ColKey> ColumnKeys()
        {
            return ColumnList;
        }

        public IEnumerable<RowKey> RowKeys()
        {
            return Map.Keys;
        }

        public void InvalidateCellAddresses()
        {
            foreach (var row in Map)
                foreach (var col in row.Value)
                {
                    Value val = col.Value;
                    CellAddressChanged(ref val, col.Key, row.Key);
                    row.Value[col.Key] = val;
                }
        }

        public bool IsValidColumn(ColKey column)
        {
            return ColumnList.Contains(column);
        }

        public bool IsValidPoint(ColKey column, RowKey row)
        {
            return IsValidColumn(column) && IsValidRow(row);
        }

        public bool IsValidRow(RowKey row)
        {
            return Map.ContainsKey(row);
        }

        public void ReallocCells(Predicate<Value> conditionToRealloc, bool skipExists)
        {
            foreach (var row in Map)
                foreach (var col in row.Value)
                {
                    Value val = col.Value;

                    if (!conditionToRealloc(val))
                        continue;

                    ReleaseCell(ref val);
                    val = AllocCell();
                    InitCell(val);
                    CellAddressChanged(ref val, col.Key, row.Key);
                    row.Value[col.Key] = val;
                }
        }


        public abstract Value AllocCell();
        public abstract void AssignCell(Value source, ref Value destination);
        public abstract void CellAddressChanged(ref Value cell, ColKey x, RowKey y);
        public abstract void InitCell(Value cell);
        public abstract void ReleaseCell(ref Value cell);

        public void OnBeforeSerialize()
        {
            SerializedColumnKeys = new ColKey[ColumnList.Count];
            SerializedRowKeys = new RowKey[Map.Count];
            SerializedCellsLinear = new Value[ColumnList.Count * Map.Count];

            int colcount = 0,
                rowcount = 0,
                cellcount = 0;

            foreach (var rowkeyvalue in Map)
            {
                SerializedRowKeys[rowcount++] = rowkeyvalue.Key;

                foreach (var colkeyvalue in rowkeyvalue.Value)
                {
                    if (colcount < ColumnList.Count)
                        SerializedColumnKeys[colcount++] = colkeyvalue.Key;

                    SerializedCellsLinear[cellcount++] = colkeyvalue.Value;
                }
            }
        }

        public void OnAfterDeserialize()
        {
            int cellcount = 0;

            Clear();

            AddColumnsRange(SerializedColumnKeys);
            AddRowsRange(SerializedRowKeys);

            foreach (var rowkeyvalue in SerializedRowKeys)
            {
                foreach (var colkeyvalue in SerializedColumnKeys)
                {
                    Map[rowkeyvalue][colkeyvalue] = SerializedCellsLinear[cellcount++];
                }
            }

            return;
        }
    }
}
