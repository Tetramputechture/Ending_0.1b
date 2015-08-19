using System;
using System.Linq;
using Ending.Lighting;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic.DungeonTools
{
    public class Dungeon : Drawable
    {
        public IDungeonStyle Style { get; }

        public Vector2i Size { get; }

        public Vector2f Center;

        public Tile[,] TileData { get; }

        private readonly Tile[,] _detailData;

        private readonly Entity[,] _entityData;

        private const int CellSize = 32;

        public LightMap LightMap { get; }

        public Dungeon(IDungeonStyle dungeonStyle, Vector2i size)
        {
            Style = dungeonStyle;
            Size = size;
            Center = new Vector2f();

            TileData = new Tile[size.X, size.Y];
            _detailData = new Tile[size.X, size.Y];

            for (var x = 0; x < size.X; x++)
            {
                for (var y = 0; y < size.Y; y++)
                {
                    TileData[x, y] = new Tile(TileType.Null);
                    _detailData[x, y] = new Tile(TileType.Null);
                }
            }

            _entityData = new Entity[size.X, size.Y];

            LightMap = new LightMap(size, 32);
        }

        public void AddFloor(int x, int y)
        {
            var t = new Tile(Style.GetFloorTileType());

            t.SetPosition(new Vector2f(CellSize * x, CellSize * y));

            TileData[x, y] = t;
        }

        public void AddFloor(int startX, int startY, int endX, int endY)
        {
            if (!IsRangeInBounds(startX, startY, endX, endY))
            {
                return;
            }

            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    AddFloor(x, y);
                }
            }
        }

        public void AddWall(int x, int y, Direction direction, bool replace)
        {
            if (!IsXInBounds(x) || !(IsYInBounds(y)))
            {
                return;
            }

            var t = new Tile(Style.GetWallTileType(direction));
            t.SetPosition(new Vector2f(CellSize * x, CellSize * y));

            if (replace)
            {
                TileData[x, y] = t;
            }
            else
            {
                TileData[x, y].Children.Push(t);
            }
        }

        public void AddRoof(int x, int y)
        {
            if (!IsXInBounds(x) || !(IsYInBounds(y)))
            {
                return;
            }

            var t = new Tile(Style.GetRoofTileType());
            t.SetPosition(new Vector2f(CellSize * x, CellSize * y));

            TileData[x, y] = t;
        }

        public void AddRoof(int startX, int startY, int endX, int endY)
        {
            if (!IsRangeInBounds(startX, startY, endX, endY))
            {
                return;
            }

            for (var x = startX; x <= endX; x++)
            {
                for (var y = startY; y <= endY; y++)
                {
                    AddRoof(x, y);
                }
            }
        }

        public void PushDetail(int x, int y, TileType tileType)
        {
            var tile = new Tile(tileType);
            tile.SetPosition(new Vector2f(CellSize * x, CellSize * y));

            if (_detailData[x, y].Type == TileType.Null)
            {
                _detailData[x, y] = tile;
            }
            else
            {
                _detailData[x, y].Children.Push(tile);
            }
        }

        public bool IsTileEmpty(int x, int y)
        {
            return TileData[x, y].Type == TileType.Null;
        }

        public bool IsFloor(int x, int y)
        {
            return TileData[x, y].Type == Style.GetFloorTileType();
        }

        public bool IsRoof(int x, int y)
        {
            return TileData[x, y].Type == Style.GetRoofTileType();
        }

        public bool IsWall(int x, int y, Direction direction)
        {
            return TileData[x, y].Type == Style.GetWallTileType(direction);
        }

        public bool HoldsFloor(int x, int y)
        {
            var floor = Style.GetFloorTileType();

            return TileData[x, y].Type == floor || TileData[x, y].Children.Any(t => t.Type == floor);
        }

        public bool HoldsWall(int x, int y, Direction direction)
        {
            var wall = Style.GetWallTileType(direction);

            return TileData[x, y].Type == wall || TileData[x, y].Children.Any(t => t.Type == wall);
        }

        public bool HoldsRoof(int x, int y)
        {
            var roof = Style.GetRoofTileType();

            return TileData[x, y].Type == roof || TileData[x, y].Children.Any(t => t.Type == roof);
        }

        public bool IsAdjacentToFloor(int x, int y)
        {
            var top = TileData[x, y + 1];
            var right = TileData[x + 1, y];
            var down = TileData[x, y - 1];
            var left = TileData[x - 1, y];

            var floor = Style.GetFloorTileType();

            return top.Contains(floor) || right.Contains(floor) || down.Contains(floor) || left.Contains(floor);
        }

        public bool IsAdjacentToRoof(int x, int y)
        {
            var top = TileData[x, y + 1];
            var right = TileData[x + 1, y];
            var down = TileData[x, y - 1];
            var left = TileData[x - 1, y];

            var roof = Style.GetRoofTileType();

            return top.Contains(roof) || right.Contains(roof) || down.Contains(roof) || left.Contains(roof);
        }

        private bool IsRangeInBounds(int xStart, int yStart, int xEnd, int yEnd)
        {
            return IsXInBounds(xStart) &&
                    IsXInBounds(xEnd) &&
                    IsYInBounds(yStart) &&
                    IsYInBounds(yEnd) &&
                    xStart <= xEnd &&
                    yStart <= yEnd;
        }

        public bool IsXInBounds(int x)
        {
            return x >= 0 && x < TileData.GetLength(0);
        }

        public bool IsYInBounds(int y)
        {
            return y >= 0 && y < TileData.GetLength(1);
        }

        public bool IsAreaUnused(int xStart, int yStart, int xEnd, int yEnd)
        {
            return ContainsOnly(xStart, yStart, xEnd, yEnd, TileType.Null);
        }

        public bool ContainsOnly(int xStart, int yStart, int xEnd, int yEnd, TileType tileType)
        {
            for (var y = yStart; y <= yEnd; y++)
            {
                for (var x = xStart; x <= xEnd; x++)
                {
                    if (TileData[x, y] == null || TileData[x, y].Type != tileType)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void AddEntity(int x, int y, Entity a)
        {
            a.Position = new Vector2f(CellSize * x, CellSize * y);

            _entityData[x, y] = a;
        }

        private void DrawBaseTiles(RenderTarget rt, RenderStates states, int startX, int startY, int endX, int endY)
        {
            for (var y = startY; y < endY; y++)
            {
                for (var x = startX; x < endX; x++)
                {
                    if (!IsXInBounds(x) || !IsYInBounds(y)) continue;

                    var t = TileData[x, y];
                    t.LightPass(LightMap.GetLightValue(x, y));
                    t.Draw(rt, states);
                }
            }
        }

        private void DrawDetailTiles(RenderTarget rt, RenderStates states, int startX, int startY, int endX, int endY)
        {
            for (var y = startY; y < endY; y++)
            {
                for (var x = startX; x < endX; x++)
                {
                    if (!IsXInBounds(x) || !IsYInBounds(y)) continue;

                    var t = _detailData[x, y];
                    t.LightPass(LightMap.GetLightValue(x, y));
                    t.Draw(rt, states);
                }
            }
        }

        public void Draw(RenderTarget rt, RenderStates states)
        {
            var view = rt.GetView();
            view.Center = Center;
            rt.SetView(view);

            // get dimensions of viewing window
            var topLeft = new Vector2i((int)(view.Center.X - view.Size.X / 2f), (int)(view.Center.Y - view.Size.Y / 2f));
            var bottomRight = new Vector2i((int)(view.Center.X + view.Size.X / 2f), (int)(view.Center.Y + view.Size.Y / 2f));

            topLeft = new Vector2i((int)Math.Floor((float) topLeft.X / CellSize), (int) Math.Floor((float)topLeft.Y / CellSize));
            topLeft -= new Vector2i(2, 2);

            bottomRight /= CellSize;
            bottomRight += new Vector2i(2, 2);

            DrawBaseTiles(rt, states, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
            DrawDetailTiles(rt, states, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }
    }
}
