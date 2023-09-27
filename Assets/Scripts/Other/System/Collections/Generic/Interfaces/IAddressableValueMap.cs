using System.Runtime.InteropServices;

namespace System.Collections.Generic.ValueMap
{
    public interface IAddressableValueMap<ColKey, RowKey, T> : IDisposable, IAssignable
    {
        T this[RowKey row, ColKey col] { get; set; }

        void AddNewColumn(ColKey to);
        void AddColumnsRange(IEnumerable<ColKey> columns);
        void AddNewRow(RowKey to);
        void AddRowsRange(IEnumerable<RowKey> rows);
        void DeleteColumn(ColKey from);
        void DeleteRow(RowKey from);
        void Clear();
        void CopyMapFrom(IAddressableValueMap<ColKey, RowKey, T> from, IEnumerable<ColKey> colKeys=null, IEnumerable<RowKey> rowKeys=null);
        void CopyColumnFrom(ColKey column, IAddressableValueMap<ColKey, RowKey, T> from, IEnumerable<RowKey> rowKeys = null);
        void CopyRowFrom(RowKey row, IAddressableValueMap<ColKey, RowKey, T> from, IEnumerable<ColKey> colKeys=null);

        int GetColumnCount();
        int GetRowCount();

        IEnumerable<ColKey> ColumnKeys();
        IEnumerable<RowKey> RowKeys();

        bool IsValidColumn(ColKey column);
        bool IsValidRow(RowKey row);
        bool IsValidPoint(ColKey column, RowKey row);

        T GetCell(ColKey column, RowKey row);
        void SetCell(ColKey col, RowKey row, T value);
        T AllocCell();
        void InitCell(T cell);
        void ReleaseCell(ref T cell);
        void CellAddressChanged(ref T cell, ColKey x, RowKey y);
        void ReallocCells(Predicate<T> conditionToRealloc, bool skipExists);
        void InvalidateCellAddresses();
        void AssignCell(T source, ref T destination);
    }
}