namespace Main
{
    public static class Map_CollisionFlagsExt
	{
		public static MapCell[] CellsByCollisionFlags(this Map map, Aggregator.Enum.MapCell.CollisionFlags flags)
		{
			return map.Common.MapData.Value.FilterCells(
				(MapCell cell) =>
				{
					return (cell.SharedProperty<Aggregator.Properties.MapCell.CollisionFlagsProperty>().Value.HasFlag(flags));
				}
			);
		}
	}
}