using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.DungeonTools
{
    public interface DungeonStyle
    {
        TileType GetRoofTileType();

        TileType GetWallTileType(Direction direction);

        TileType GetFloorTileType();
    }
}