using System;
using System.Diagnostics;
using SFML.System;

namespace Ending.GameLogic.DungeonTools
{
    public class DungeonGenerator
    {
        public int Seed;

        private Vector2i _size;

        private Vector2i _minRoomSize;
        private Vector2i _maxRoomSize;

        private readonly int _minCorridorLength;
        private readonly int _maxCorridorLength;

        private readonly int _maxFeatures;

        private readonly int _chanceRoom;

        public DungeonGenerator()
        {
            _minRoomSize = new Vector2i(4, 4);
            _maxRoomSize = new Vector2i(8, 6);
            _minCorridorLength = 2;
            _maxCorridorLength = 6;
            _maxFeatures = 200;
            _chanceRoom = 75;
        }

        public Dungeon Generate(IDungeonStyle dungeonStyle, int x, int y)
        {

            Seed = (int)DateTime.Now.Ticks / (int)TimeSpan.TicksPerMillisecond;

            var rand = new Random(Seed);

            _size = new Vector2i(x, y);

            var dungeon = new Dungeon(dungeonStyle, _size);

            MakeDungeon(dungeon, rand);

            return dungeon;
        }

        public Dungeon Generate(IDungeonStyle dungeonStyle, int x, int y, int seed)
        {
            var rand = new Random(seed);

            _size = new Vector2i(x, y);

            var dungeon = new Dungeon(dungeonStyle, _size);

            MakeDungeon(dungeon, rand);

            return dungeon;
        }

        private bool IsValidArea(Dungeon dungeon, int xStart, int yStart, int xEnd, int yEnd)
        {
            if (!dungeon.IsXInBounds(xStart)
                || !dungeon.IsXInBounds(xEnd)
                || !dungeon.IsYInBounds(yStart)
                || !dungeon.IsYInBounds(yEnd))
                return false;

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

            var length = rand.Next(_minCorridorLength, _maxCorridorLength);

            var xStart = x;
            var yStart = y;

            var xEnd = x;
            var yEnd = y;

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

            dungeon.AddFloor(xStart, yStart, xEnd, yEnd);

            dungeon.AddFloor(doorX, doorY);

            return true;
        }

        private bool MakeRoom(Dungeon dungeon,
                Random rand,
                int x,
                int y,
                Direction direction)
        {

            var xLength = rand.Next(_minRoomSize.X, _maxRoomSize.X);
            var yLength = rand.Next(_minRoomSize.Y, _maxRoomSize.Y);

            var xStart = x;
            var yStart = y;

            var xEnd = x;
            var yEnd = y;

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

            dungeon.AddRoof(xStart, yStart, xEnd, yEnd);
            dungeon.AddFloor(xStart + 1, yStart + 1, xEnd - 1, yEnd - 1);

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

            var chance = rand.Next(0, 100);

            if (chance > _chanceRoom)
                return MakeCorridor(dungeon, rand, x + xmod, y + ymod, x, y, direction);

            if (!MakeRoom(dungeon, rand, x + xmod, y + ymod, direction)) return false;

            dungeon.AddFloor(x, y);
            dungeon.AddFloor(x + xmod, y + ymod);
            return true;
        }

        private void MakeWalls(Dungeon dungeon)
        {

            // iterate across all floor tiles
            // if tile Is adjacent to a void or unused tile:
            // Push the appropriate door tile to it
            for (var y = 1; y < _size.Y - 1; y++)
            {
                for (var x = 1; x < _size.X - 1; x++)
                {
                    if (!dungeon.HoldsFloor(x, y) || !dungeon.IsAdjacentToRoof(x, y)) continue;

                    // wall on north
                    if (dungeon.IsRoof(x, y - 1))
                    {
                        dungeon.AddWall(x, y - 1, Direction.North, true);
                    }
                    // wall on east
                    if (dungeon.IsRoof(x + 1, y))
                    {
                        dungeon.AddWall(x + 1, y, Direction.East, false);
                    }
                    // wall on west
                    if (dungeon.IsRoof(x - 1, y))
                    {
                        dungeon.AddWall(x - 1, y, Direction.West, false);
                    }
                    if (dungeon.IsRoof(x, y + 1))
                    {
                        dungeon.PushDetail(x, y, dungeon.Style.GetWallTileType(Direction.South));
                    }
                }
            }

            // Make sure north tiles are bounded on left and right by other walls
            for (var y = 1; y < _size.Y - 1; y++)
            {
                for (var x = 1; x < _size.X - 1; x++)
                {
                    if (!dungeon.HoldsWall(x, y, Direction.North)) continue;

                    if (dungeon.IsRoof(x - 1, y))
                    {
                        dungeon.AddWall(x - 1, y, Direction.West, false);
                    }
                    if (dungeon.IsRoof(x + 1, y))
                    {
                        dungeon.AddWall(x + 1, y, Direction.East, false);
                    }
                }
            }
        }

        private bool MakeFeature(Dungeon dungeon, Random rand)
        {
            var style = dungeon.Style;

            var voidType = style.GetRoofTileType();
            var floorType = style.GetFloorTileType();

            const int maxTries = 1000;

            for (var tries = 0; tries < maxTries; tries++)
            {
                // Pick a random wall or corridor tile.
                // Make sure it has no adjacent doors (looks weird to have doors next to each other).
                // Find a direction from which it's reachable.
                // Attempt to Make a feature (room or corridor) starting at thIs point.

                var x = rand.Next(1, _size.X - 2);
                var y = rand.Next(1, _size.Y - 2);

                var cellXy = dungeon.TileData[x, y].Type;
                var cellXPlusOneY = dungeon.TileData[x + 1, y].Type;
                var cellXMinusOneY = dungeon.TileData[x - 1, y].Type;
                var cellXyPlusOne = dungeon.TileData[x, y + 1].Type;
                var cellXyMinusOne = dungeon.TileData[x, y - 1].Type;

                if (!(cellXy == voidType || cellXy == floorType))
                {
                    continue;
                }

                if (cellXyPlusOne == floorType)
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
                else if (cellXyMinusOne == floorType)
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

        private void MakeDungeon(Dungeon dungeon, Random rand)
        {
            var watch = new Stopwatch();

            watch.Start();

            for (var y = 0; y < _size.Y; y++)
            {
                for (var x = 0; x < _size.X; x++)
                {
                    if (y == 0 || y == _size.Y - 1 || x == 0 || x == _size.X - 1)
                    {
                        dungeon.AddRoof(x, y);
                    }
                }
            }

            // Make one room in the middle to start things off.
            MakeRoom(dungeon, rand, _size.X / 2, _size.Y / 2, Utils.RandDirection(rand));

            for (var features = 1; features < _maxFeatures; features++)
            {
                if (!MakeFeature(dungeon, rand))
                {
                    break;
                }
            }

            for (var y = 0; y < _size.Y; y++)
            {
                for (var x = 0; x < _size.X; x++)
                {
                    if (dungeon.IsTileEmpty(x, y))
                    {
                        dungeon.AddRoof(x, y);
                    }
                }
            }

            MakeWalls(dungeon);

            var time = watch.Elapsed;

            Console.WriteLine("Dungeon generated in " + time.Milliseconds / 1000f + " seconds with "
                    + "seed " + Seed);
        }

    }
}