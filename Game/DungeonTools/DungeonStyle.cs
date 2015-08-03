using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.DungeonTools
{
    public interface DungeonStyle
    {
        TileType GetUnusedTileType();

        TileType GetVoidTileType();

        TileType GetWallTileType(Direction direction);

        TileType GetFloorTileType();

        TileType GetUpStairsTileType();

        TileType GetDownStairsTileType();
    }
}
