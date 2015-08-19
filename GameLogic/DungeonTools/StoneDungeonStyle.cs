namespace Ending.GameLogic.DungeonTools
{
    public class StoneDungeonStyle : IDungeonStyle
    {
        public TileType GetRoofTileType()
        {
            return TileType.Stoneroof;
        }

        public TileType GetWallTileType(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return TileType.StonewallNorth;
                case Direction.East:
                    return TileType.StonewallEast;
                case Direction.West:
                    return TileType.StonewallWest;
                case Direction.South:
                    return TileType.StonewallSouth;
                default:
                    return TileType.StonewallNorth;
            }
        }

        public TileType GetFloorTileType()
        {
            return TileType.Stonefloor;
        }
    }
}