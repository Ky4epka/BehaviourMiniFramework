using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Main.Objects;
using Main.Objects.Behaviours;
using Main.Objects.Behaviours.Attributes;

namespace Main
{


    public struct Map_Cell_ModificatorsMask
    {
        public const int MASK_SIZE = 3;

        public Map_Cell_Modificators.Enum[,] Mask;

        public Map_Cell_ModificatorsMask(Map_Cell_Modificators.Enum[,] mask_source)
        {
            Mask = new Map_Cell_Modificators.Enum[MASK_SIZE, MASK_SIZE];
            Assign(mask_source);
        }

        public Map_Cell_ModificatorsMask(Map_Cell_Modificators.Enum initial_value)
        {
            Mask = new Map_Cell_Modificators.Enum[MASK_SIZE, MASK_SIZE];
            Fill(initial_value);
        }

        public void Fill(Map_Cell_Modificators.Enum value)
        {
            for (int i = 0; i < MASK_SIZE; i++)
            {
                for (int j = 0; j < MASK_SIZE; j++)
                {
                    Mask[i, j] = value;
                }
            }
        }

        public void Assign(Map_Cell_Modificators.Enum[,] source)
        {
            for (int i = 0; i < MASK_SIZE; i++)
            {
                for (int j = 0; j < MASK_SIZE; j++)
                {
                    Mask[i, j] = source[i, j];
                }
            }
        }

        public void Set(int column, int row, Map_Cell_Modificators.Enum value)
        {
            Mask[row, column] = value;
        }
                
        public bool CompareMask(Map_Cell_Modificators.Enum[,] array)
        {
            for (int i = 0; i < MASK_SIZE; i++)
            {
                for (int j = 0; j < MASK_SIZE; j++)
                {
                    if (array[i, j] != Mask[i, j] && (Mask[i,j] != Map_Cell_Modificators.Enum.Ignore))
                        return false;
                }
            }

            return true;
        }

        public bool CompareMask(Map_Cell_ModificatorsMask mask)
        {
            return CompareMask(mask.Mask);
        }

        public Map_Cell_ModificatorsMask ExtractMask(Map_Cell_Modificators.Enum value)
        {
            Map_Cell_ModificatorsMask result = new Map_Cell_ModificatorsMask(0);

            for (int i = 0; i < MASK_SIZE; i++)
            {
                for (int j = 0; j < MASK_SIZE; j++)
                {
                    if ((Mask[i, j] & value) == value)
                        result.Set(j, i, value);
                }
            }

            return result;
        }

    }

    public struct Map_Cell_ModificatorsData
    {
        public string SpriteName;
        public Map_Cell_ModificatorsMask IdentMask;

        public Map_Cell_ModificatorsData(string sprite_name, Map_Cell_ModificatorsMask ident_mask)
        {
            SpriteName = sprite_name;
            IdentMask = ident_mask;
        }

    }

    public class Map_Cell_ModificatorsDataManager
    {
        public List<Map_Cell_ModificatorsData> Data = new List<Map_Cell_ModificatorsData>();
        public Map_Cell_ModificatorsData DefaultData = new Map_Cell_ModificatorsData();

        protected static Map_Cell_ModificatorsDataManager iInstance = null;

        public Map_Cell_ModificatorsDataManager()
        {
            Fill();
        }

        public static Map_Cell_ModificatorsDataManager Instance
        {
            get
            {
                if (iInstance == null) iInstance = new Map_Cell_ModificatorsDataManager();

                return iInstance;
            }
        }

        public Map_Cell_ModificatorsData DataByMask(Map_Cell_ModificatorsMask mask, Map_Cell_ModificatorsData default_data)
        {
            for (int i= 0; i<Data.Count; i++)
            {
                if (Data[i].IdentMask.CompareMask(mask))
                    return Data[i];
            }

            return default_data;
        }
        
        public Map_Cell_ModificatorsData DataByMask(Map_Cell_ModificatorsMask mask)
        {
            return DataByMask(mask, DefaultData);
        }

        public void SetDefaultData(Map_Cell_ModificatorsData data)
        {
            Instance.DefaultData = data;
        }

        public void Fill()
        {
            /*
             
            Data.Add(new Map_Cell_ModificatorsData("MAP_DEFAULT", new Map_Cell_ModificatorsMask(new long[3, 3] 
                { 
                  {Map_Cell_Modificators.MOD_UNKNOWN, Map_Cell_Modificators.MOD_UNKNOWN, Map_Cell_Modificators.MOD_UNKNOWN}, 
                  {Map_Cell_Modificators.MOD_UNKNOWN, Map_Cell_Modificators.MOD_UNKNOWN, Map_Cell_Modificators.MOD_UNKNOWN}, 
                  {Map_Cell_Modificators.MOD_UNKNOWN, Map_Cell_Modificators.MOD_UNKNOWN, Map_Cell_Modificators.MOD_UNKNOWN} 
                })));


             */

            Map_Cell_ModificatorsData default_data = (new Map_Cell_ModificatorsData("MAP_DEFAULT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3] 
                { 
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}, 
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}, 
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown} 
                })));
            Data.Add(default_data);
            DefaultData = default_data;

            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_TOPLEFT_CORNER", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderTop},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_TOPRIGHT_CORNER", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapBorderTop, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_BOTTOMLEFT_CORNER", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderBottom},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_BOTTOMRIGHT_CORNER", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapBorderBottom, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));


            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderBottom},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapBorderBottom, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderTop},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapBorderTop, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));

            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_LEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_RIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_TOP", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderTop, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_BOTTOM", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderBottom, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));



            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_LEFT_TRANSIT_TO_OBSTACLE_TOP", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
            {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.MapObstacleTop},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Ignore}
            })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_LEFT_TRANSIT_TO_OBSTACLE_BOTTOM", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
            {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.MapObstacleBottom},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderLeft, Map_Cell_Modificators.Enum.Ignore}
            })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_RIGHT_TRANSIT_TO_OBSTACLE_TOP", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
            {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.MapObstacleTop, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Ignore}
            })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_RIGHT_TRANSIT_TO_OBSTACLE_BOTTOM", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
            {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.MapObstacleBottom, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapBorderRight, Map_Cell_Modificators.Enum.Ignore}
            })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_TOP_TRANSIT_TO_OBSTACLE_LEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
            {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.MapBorderTop, Map_Cell_Modificators.Enum.MapBorderTop, Map_Cell_Modificators.Enum.MapBorderTop},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.Ignore}
            })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_TOP_TRANSIT_TO_OBSTACLE_RIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
            {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.MapBorderTop, Map_Cell_Modificators.Enum.MapBorderTop, Map_Cell_Modificators.Enum.MapBorderTop},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Ignore}
            })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_BOTTOM_TRANSIT_TO_OBSTACLE_LEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
            {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.MapBorderBottom, Map_Cell_Modificators.Enum.MapBorderBottom, Map_Cell_Modificators.Enum.MapBorderBottom},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
            })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_BOTTOM_TRANSIT_TO_OBSTACLE_RIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
            {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.MapBorderBottom, Map_Cell_Modificators.Enum.MapBorderBottom, Map_Cell_Modificators.Enum.MapBorderBottom},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
            })));



            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleTop},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleTop, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleBottom},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleBottom, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));


            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_INNER_BOTTOMRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleBottom},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_INNER_BOTTOMLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.MapObstacleBottom, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_INNER_TOPRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleTop},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_INNER_TOPLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.MapObstacleTop, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));

            // Single cell
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.MapObstacleSingle},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.MapObstacleSingle}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.MapObstacleSingle},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.MapObstacleSingle},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.MapObstacleSingle, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));

            // Vert thin line
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleTop, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.MapObstacleRight}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleTop, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.MapObstacleRight},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleBottom, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleBottom, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));


            // Horz thin line
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.MapObstacleTop},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleBottom}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_TOPRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleTop, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleBottom, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleTop},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.MapObstacleBottom},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_THIN_BOTTOMRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.MapObstacleTop, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.MapObstacleBottom, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));

            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_LEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));

            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_RIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_TOP", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleTop, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_OBSTACLE_BOTTOM", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.MapObstacleBottom, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));

            Data.Add(new Map_Cell_ModificatorsData("MAP_GHOSTS_ENCLOSURE_TOPLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureTop},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.GhostsEnclosureLeft, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_GHOSTS_ENCLOSURE_TOPRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.GhostsEnclosureTop, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureRight, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_GHOSTS_ENCLOSURE_BOTTOMLEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.GhostsEnclosureLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureBottom},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_GHOSTS_ENCLOSURE_BOTTOMRIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureRight, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.GhostsEnclosureBottom, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Unknown},
                  {Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown, Map_Cell_Modificators.Enum.Unknown}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_RIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_LEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_BOTTOM", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureTop, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_BORDER_TOP", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureBottom, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_GHOSTS_DOOR_LEFT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureDoorLeft, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_GHOSTS_DOOR_RIGHT", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureDoorRight, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_GHOSTS_DOOR_TOP", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureDoorTop, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
            Data.Add(new Map_Cell_ModificatorsData("MAP_GHOSTS_DOOR_BOTTOM", new Map_Cell_ModificatorsMask(new Map_Cell_Modificators.Enum[3, 3]
                {
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.GhostsEnclosureDoorBottom, Map_Cell_Modificators.Enum.Ignore},
                  {Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore, Map_Cell_Modificators.Enum.Ignore}
                })));
        }
    }

    [RequireComponent(typeof(MapCell))]
    [Unique]
    public class Map_Cell_ModificatorsBehaviour : ObjectBehavioursBase
    {
        [SharedProperty]
        public Aggregator.Properties.MapCell.ModificatorsProperty Modificators { get; private set; }

        protected MapCell icachedCell = null;
        protected MapCell[] EnumCellsBuffer = new MapCell[Map_Common.CELL_SIBLINGS_COUNT+1];
        protected bool iDisableRefreshSiblings = false;

        [SharedPropertyViewer(typeof(Aggregator.Properties.MapCell.ModificatorsProperty))]
        public void ModificatorsViewer(Aggregator.Events.MapCell.ModificatorsProperty eventData)
        {
            //HandleModificators(eventData.PropertyValue);
        }

        protected void HandleModificators(Map_Cell_Modificators.Enum mods)
        {
            int count = 0;
            icachedCell.SharedProperty<Aggregator.Properties.MapCell.OwnerProperty>().Value.Common.EnumCellSiblings(icachedCell.SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value, ref EnumCellsBuffer, ref count, null, null);
            EnumCellsBuffer[count++] = icachedCell;

            MapCell cell;
            Vector2Int offset;
            Map_Cell_ModificatorsMask mask = new Map_Cell_ModificatorsMask(Map_Cell_Modificators.MOD_UNKNOWN);

            for (int i = 0; i < count; i++)
            {
                cell = EnumCellsBuffer[i];
                offset = new Vector2Int(cell.SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value.x - icachedCell.SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value.x, icachedCell.SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value.y - cell.SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value.y);
                mask.Set(offset.x+1, offset.y+1, cell.SharedProperty<Aggregator.Properties.MapCell.ModificatorsProperty>().Value);
            }

            //icachedCell.ProvideSharedProperty<Aggregator.Properties.MapCell.SpriteName.Value = Map_Cell_ModificatorsDataManager.Instance.DataByMask(mask).SpriteName;

            if (!iDisableRefreshSiblings)
                RefreshSiblings();
            /*
            if ((mods & Map_Cell_Modificators.MOD_MAP_BORDER) == Map_Cell_Modificators.MOD_MAP_BORDER)
            {
                refresh_mods = true;
            }
            
            if ((mods & Map_Cell_Modificators.MOD_MAP_OBSTACLE) == Map_Cell_Modificators.MOD_MAP_OBSTACLE)
            {
                refresh_mods = true;
            }

            if ((mods & Map_Cell_Modificators.MOD_GHOSTS_ENCLOSURE) == Map_Cell_Modificators.MOD_GHOSTS_ENCLOSURE)
            {
                refresh_mods = true;
            }

            if ((mods & Map_Cell_Modificators.MOD_GHOSTS_ENCLOSURE_DOOR) == Map_Cell_Modificators.MOD_GHOSTS_ENCLOSURE_DOOR)
            {
                refresh_mods = true;
            }
            */

//            for (int i = 0; i < count; i++)
 //               EnumCellsBuffer[i].Modificators = EnumCellsBuffer[i].Modificators;
        }

        public void RefreshSiblings()
        {
            int count = 0;
            icachedCell.SharedProperty<Aggregator.Properties.MapCell.OwnerProperty>().Value.Common.EnumCellSiblings(icachedCell.SharedProperty<Aggregator.Properties.MapCell.IndexesProperty>().Value, ref EnumCellsBuffer, ref count, null, null);
            EnumCellsBuffer[count++] = icachedCell;

            for (int i = 0; i < count; i++)
            {
                Map_Cell_ModificatorsBehaviour cell = EnumCellsBuffer[i].GetComponent<Map_Cell_ModificatorsBehaviour>();
                cell.iDisableRefreshSiblings = true;
                try 
                {
                }
                finally
                {
                    cell.iDisableRefreshSiblings = false;
                }                
            }
        }

        protected override void Awake()
        {
            base.Awake();
            icachedCell = GetComponent<MapCell>();
        }

    }
}