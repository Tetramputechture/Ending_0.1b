namespace Ending.GameLogic.DungeonTools
{
    public interface IDungeonStyle
    {
        TileType GetRoofTileType();

        TileType GetWallTileType(Direction direction);

        TileType GetFloorTileType();
    }
}