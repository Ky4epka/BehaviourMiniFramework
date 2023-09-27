namespace Main
{
    public class Map_Cell_Modificators
	{
		public const int MOD_UNKNOWN = 0;
		public const int MOD_MAP_BORDER_LEFT = 0x01;
		public const int MOD_MAP_BORDER_RIGHT = 0x02;
		public const int MOD_MAP_BORDER_TOP = 0x04;
		public const int MOD_MAP_BORDER_BOTTOM = 0x08;
		public const int MOD_MAP_OBSTACLE_LEFT = 0x10;
		public const int MOD_MAP_OBSTACLE_RIGHT = 0x20;
		public const int MOD_MAP_OBSTACLE_TOP = 0x40;
		public const int MOD_MAP_OBSTACLE_BOTTOM = 0x80;
		public const int MOD_MAP_OBSTACLE_SINGLE = 0x100;
		public const int MOD_MAP_BORDER_LEFT_TRANSIT_TO_OBSTACLE = 0x200;
		public const int MOD_MAP_BORDER_RIGHT_TRANSIT_TO_OBSTACLE = 0x400;
		public const int MOD_MAP_BORDER_TOP_TRANSIT_TO_OBSTACLE = 0x800;
		public const int MOD_MAP_BORDER_BOTTOM_TRANSIT_TO_OBSTACLE = 0x1000;
		public const int MOD_GHOSTS_ENCLOSURE_LEFT = 0x2000;
		public const int MOD_GHOSTS_ENCLOSURE_RIGHT = 0x4000;
		public const int MOD_GHOSTS_ENCLOSURE_TOP = 0x8000;
		public const int MOD_GHOSTS_ENCLOSURE_BOTTOM = 0x10000;
		public const int MOD_GHOSTS_ENCLOSURE_DOOR_LEFT = 0x20000;
		public const int MOD_GHOSTS_ENCLOSURE_DOOR_RIGHT = 0x40000;
		public const int MOD_GHOSTS_ENCLOSURE_DOOR_TOP = 0x80000;
		public const int MOD_GHOSTS_ENCLOSURE_DOOR_BOTTOM = 0x100000;
		public const int MOD_IGNORE = 0x200000;
		public const int MOD_GHOSTS_CAGE_AREA = 0x400000;

		public enum Enum
		{
			Unknown = MOD_UNKNOWN,
			MapBorderLeft = MOD_MAP_BORDER_LEFT,
			MapBorderRight = MOD_MAP_BORDER_RIGHT,
			MapBorderTop = MOD_MAP_BORDER_TOP,
			MapBorderBottom = MOD_MAP_BORDER_BOTTOM,
			MapObstacleLeft = MOD_MAP_OBSTACLE_LEFT,
			MapObstacleRight = MOD_MAP_OBSTACLE_RIGHT,
			MapObstacleTop = MOD_MAP_OBSTACLE_TOP,
			MapObstacleBottom = MOD_MAP_OBSTACLE_BOTTOM,
			MapObstacleSingle = MOD_MAP_OBSTACLE_SINGLE,
			MapBorderLeftTransitToObstacle = MOD_MAP_BORDER_LEFT_TRANSIT_TO_OBSTACLE,
			MapBorderRightTransitToObstacle = MOD_MAP_BORDER_RIGHT_TRANSIT_TO_OBSTACLE,
			MapBorderTopTransitToObstacle = MOD_MAP_BORDER_TOP_TRANSIT_TO_OBSTACLE,
			MapBorderBottomTransitToObstacle = MOD_MAP_BORDER_BOTTOM_TRANSIT_TO_OBSTACLE,
			GhostsEnclosureLeft = MOD_GHOSTS_ENCLOSURE_LEFT,
			GhostsEnclosureRight = MOD_GHOSTS_ENCLOSURE_RIGHT,
			GhostsEnclosureTop = MOD_GHOSTS_ENCLOSURE_TOP,
			GhostsEnclosureBottom = MOD_GHOSTS_ENCLOSURE_BOTTOM,
			GhostsEnclosureDoorLeft = MOD_GHOSTS_ENCLOSURE_DOOR_LEFT,
			GhostsEnclosureDoorRight = MOD_GHOSTS_ENCLOSURE_DOOR_RIGHT,
			GhostsEnclosureDoorTop = MOD_GHOSTS_ENCLOSURE_DOOR_TOP,
			GhostsEnclosureDoorBottom = MOD_GHOSTS_ENCLOSURE_DOOR_BOTTOM,
			GhostsCageArea = MOD_GHOSTS_CAGE_AREA,
			Ignore = MOD_IGNORE
		}


	}
}