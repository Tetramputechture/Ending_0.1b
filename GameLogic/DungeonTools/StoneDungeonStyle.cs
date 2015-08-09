using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.DungeonTools
{
    public class StoneDungeonStyle : DungeonStyle
    {
        public TileType GetRoofTileType()
        {
            return TileType.STONEROOF;
        }

        public TileType GetWallTileType(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return TileType.STONEWALL_NORTH;
                case Direction.East:
                    return TileType.STONEWALL_EAST;
                case Direction.West:
                    return TileType.STONEWALL_WEST;
                case Direction.South:
                    return TileType.STONEWALL_SOUTH;
                default:
                    return TileType.STONEWALL_NORTH;
            }
        }

        public TileType GetFloorTileType()
        {
            return TileType.STONEFLOOR;
        }
    }
}