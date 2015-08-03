using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.DungeonTools
{
    public class DungeonGenerator
    {
        public int seed { get; set; }

        private Vector2i size;

        private Vector2i MinRoomSize;
        private Vector2i MaxRoomSize;

        private int MinCorridorLength;
        private int MaxCorridorLength;

        private int MaxFeatures;

        private int chanceRoom;

        public DungeonGenerator()
        {
            MinRoomSize = new Vector2i(4, 4);
            MaxRoomSize = new Vector2i(8, 6);
            MinCorridorLength = 2;
            MaxCorridorLength = 6;
            MaxFeatures = 200;
            chanceRoom = 75;
        }

        public Dungeon Generate(DungeonStyle dungeonStyle, int x, int y)
        {

            seed = (int)DateTime.Now.Ticks / (int)TimeSpan.TicksPerMillisecond;

            Random rand = new Random(seed);
            
            size = new Vector2i(x, y);

            Dungeon dungeon = new Dungeon(dungeonStyle, size);

            MakeDungeon(dungeon, rand);

            return dungeon;
        }

        public Dungeon Generate(DungeonStyle dungeonStyle, int x, int y, int seed)
        {
            Random rand = new Random(seed);

            size = new Vector2i(x, y);

            Dungeon dungeon = new Dungeon(dungeonStyle, size);

            MakeDungeon(dungeon, rand);

            return dungeon;
        }

        private bool IsValidArea(Dungeon dungeon, int xStart, int yStart, int xEnd, int yEnd)
        {
            if (!dungeon.IsXInBounds(xStart)
                    || !dungeon.IsXInBounds(xEnd)
                    || !dungeon.IsYInBounds(yStart)
                    || !dungeon.IsYInBounds(yEnd))
            {
                return false;
            }

            return dungeon.IsAreaUnused(xStart, yStart, xEnd, yEnd);
        }

        private bool MakeCorridor(Dungeon dungeon,
                Random rand,
                int x,
                int y,
                int doorX,
                int doorY,
                Direction direction)
        {

            int length = rand.Next(MinCorridorLength, MaxCorridorLength);

            int xStart = x;
            int yStart = y;

            int xEnd = x;
            int yEnd = y;

            switch (direction)
            {
                case Direction.North:
                    yStart = y - length;
                    break;
                case Direction.East:
                    xEnd = x + length;
                    break;
                case Direction.South:
                    yEnd = y + length;
                    break;
                case Direction.West:
                    xStart = x - length;
                    break;
            }

            if (!IsValidArea(dungeon, xStart, yStart, xEnd, yEnd))
            {
                return false;
            }

            TileType floorType = dungeon.dungeonStyle.GetFloorTileType();

            dungeon.AddTiles(xStart, yStart, xEnd, yEnd, floorType);

            dungeon.AddTile(doorX, doorY, floorType);

            return true;
        }

        private bool MakeRoom(Dungeon dungeon,
                Random rand,
                int x,
                int y,
                Direction direction)
        {

            int xLength = rand.Next(MinRoomSize.X, MaxRoomSize.X);
            int yLength = rand.Next(MinRoomSize.Y, MaxRoomSize.Y);

            int xStart = x;
            int yStart = y;

            int xEnd = x;
            int yEnd = y;

            switch (direction)
            {
                case Direction.North:
                    yStart = y - yLength;
                    xStart = x - xLength / 2;
                    xEnd = x + (xLength + 1) / 2;
                    break;
                case Direction.East:
                    yStart = y - yLength / 2;
                    yEnd = y + (yLength + 1) / 2;
                    xEnd = x + xLength;
                    break;
                case Direction.South:
                    yEnd = y + yLength;
                    xStart = x - xLength / 2;
                    xEnd = x + (xLength + 1) / 2;
                    break;
                case Direction.West:
                    yStart = y - yLength / 2;
                    yEnd = y + (yLength + 1) / 2;
                    xStart = x - xLength;
                    break;
            }

            if (!IsValidArea(dungeon, xStart, yStart, xEnd, yEnd))
            {
                return false;
            }

            DungeonStyle style = dungeon.dungeonStyle;

            dungeon.AddTiles(xStart, yStart, xEnd, yEnd, style.GetVoidTileType());
            dungeon.AddTiles(xStart + 1, yStart + 1, xEnd - 1, yEnd - 1, style.GetFloorTileType());

            return true;
        }

        private bool MakeFeature(Dungeon dungeon,
                Random rand,
                int x,
                int y,
                int xmod,
                int ymod,
                Direction direction)
        {

            int chance = rand.Next(0, 100);

            if (chance <= chanceRoom)
            {
                if (MakeRoom(dungeon, rand, x + xmod, y + ymod, direction))
                {
                    TileType floorType = dungeon.dungeonStyle.GetFloorTileType();
                    dungeon.AddTile(x, y, floorType);
                    dungeon.AddTile(x + xmod, y + ymod, floorType);
                    return true;
                }
                return false;
            }
            else
            {
                return MakeCorridor(dungeon, rand, x + xmod, y + ymod, x, y, direction);
            }
        }

        private void MakeWalls(Dungeon dungeon)
        {

            DungeonStyle style = dungeon.dungeonStyle;

            TileType floorType = style.GetFloorTileType();
            TileType voidType = style.GetVoidTileType();
            TileType northWall = style.GetWallTileType(Direction.North);
            TileType eastWall = style.GetWallTileType(Direction.East);
            TileType westWall = style.GetWallTileType(Direction.West);
            TileType southWall = style.GetWallTileType(Direction.South);


            // iterate across all floor tiles
            // if tile Is adjacent to a void or unused tile:
            // Push the appropriate door tile to it
            for (int y = 1; y < size.Y - 1; y++)
            {
                for (int x = 1; x < size.X - 1; x++)
                {
                    if (dungeon.holdsTileType(x, y, floorType) && dungeon.IsAdjacent(x, y, voidType))
                    {
                        // wall on north
                        if (dungeon.GetTileTypeEquals(x, y - 1, voidType))
                        {
                            dungeon.AddTile(x, y - 1, northWall);
                        }
                        // wall on east
                        if (dungeon.GetTileTypeEquals(x + 1, y, voidType))
                        {
                            dungeon.PushTile(x + 1, y, eastWall);
                        }
                        // wall on west
                        if (dungeon.GetTileTypeEquals(x - 1, y, voidType))
                        {
                            dungeon.PushTile(x - 1, y, westWall);
                        }
                        if (dungeon.GetTileTypeEquals(x, y + 1, voidType))
                        {
                            dungeon.PushDetail(x, y, southWall);
                        }
                    }
                }
            }

            // Make sure north tiles are bounded on left and right by other walls
            for (int y = 1; y < size.Y - 1; y++)
            {
                for (int x = 1; x < size.X - 1; x++)
                {
                    if (dungeon.holdsTileType(x, y, northWall) && dungeon.IsAdjacent(x, y, voidType))
                    {
                        if (dungeon.GetTileTypeEquals(x - 1, y, voidType))
                        {
                            dungeon.PushTile(x - 1, y, westWall);
                        }
                        if (dungeon.GetTileTypeEquals(x + 1, y, voidType))
                        {
                            dungeon.PushTile(x + 1, y, eastWall);
                        }
                    }
                }
            }
        }

        private bool MakeFeature(Dungeon dungeon, Random rand)
        {

            DungeonStyle style = dungeon.dungeonStyle;

            TileType voidType = style.GetVoidTileType();
            TileType floorType = style.GetFloorTileType();

            int MaxTries = 1000;

            for (int tries = 0; tries < MaxTries; tries++)
            {
                // Pick a random wall or corridor tile.
                // Make sure it has no adjacent doors (looks weird to have doors next to each other).
                // Find a direction from which it's reachable.
                // Attempt to Make a feature (room or corridor) starting at thIs point.

                int x = rand.Next(1, size.X - 2);
                int y = rand.Next(1, size.Y - 2);

                TileType cellXY = dungeon.tileData[x, y].type;
                TileType cellXPlusOneY = dungeon.tileData[x + 1, y].type;
                TileType cellXMinusOneY = dungeon.tileData[x - 1, y].type;
                TileType cellXYPlusOne = dungeon.tileData[x, y + 1].type;
                TileType cellXYMinusOne = dungeon.tileData[x, y - 1].type;

                if (!(cellXY == voidType || cellXY == floorType))
                {
                    continue;
                }

                if (cellXYPlusOne == floorType)
                {
                    if (MakeFeature(dungeon, rand, x, y, 0, -1, Direction.North))
                    {
                        return true;
                    }
                }
                else if (cellXMinusOneY == floorType)
                {
                    if (MakeFeature(dungeon, rand, x, y, 1, 0, Direction.East))
                    {
                        return true;
                    }
                }
                else if (cellXYMinusOne == floorType)
                {
                    if (MakeFeature(dungeon, rand, x, y, 0, 1, Direction.South))
                    {
                        return true;
                    }
                }
                else if (cellXPlusOneY == floorType)
                {
                    if (MakeFeature(dungeon, rand, x, y, -1, 0, Direction.West))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool MakeStairs(Dungeon dungeon, Random rand, TileType tileType)
        {

            DungeonStyle style = dungeon.dungeonStyle;

            int MaxTries = 10000;

            for (int tries = 0; tries < MaxTries; tries++)
            {
                int x = rand.Next(1, size.X - 2);
                int y = rand.Next(1, size.Y - 2);

                if (!(dungeon.IsAdjacent(x, y, style.GetFloorTileType())
                        && dungeon.IsAdjacent(x, y, style.GetVoidTileType())))
                {
                    continue;
                }

                dungeon.AddTile(x, y, tileType);

                return true;
            }
            return false;
        }

        private bool MakeDungeon(Dungeon dungeon, Random rand)
        {

            DungeonStyle style = dungeon.dungeonStyle;

            Stopwatch watch = new Stopwatch();

            watch.Start();

            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    if (y == 0 || y == size.Y - 1 || x == 0 || x == size.X - 1)
                    {
                        dungeon.AddTile(x, y, style.GetVoidTileType());
                    }
                }
            }

            // Make one room in the middle to start things off.
            MakeRoom(dungeon, rand, size.X / 2, size.Y / 2, Utils.RandDirection(rand));

            for (int features = 1; features < MaxFeatures; features++)
            {
                if (!MakeFeature(dungeon, rand))
                {
                    break;
                }
            }

            if (!MakeStairs(dungeon, rand, style.GetUpStairsTileType()))
            {
                Console.WriteLine("Unable to place up stairs.");
            }

            if (!MakeStairs(dungeon, rand, style.GetDownStairsTileType()))
            {
                Console.WriteLine("Unable to place down stairs");
            }

            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    if (dungeon.GetTileTypeEquals(x, y, style.GetUnusedTileType()))
                    {
                        dungeon.AddTile(x, y, style.GetVoidTileType());
                    }
                }
            }

            MakeWalls(dungeon);

            TimeSpan time = watch.Elapsed;

            Console.WriteLine("Dungeon generated in " + time.Milliseconds / 1000f + " seconds with "
                    + "seed " + seed);

            return true;
        }

    }
}
