using Ending.Lighting;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.DungeonTools
{
    public class Dungeon : Drawable
    {
        public DungeonStyle style { get; }

        public Vector2i size { get; }

        public Vector2f center;

        public Tile[,] tileData { get; }

        private Tile[,] detailData;

        private Entity[,] entityData;

        private const int cellSize = 32;

        public LightMap lightMap { get; }

        public Dungeon(DungeonStyle dungeonStyle, Vector2i size)
        {
            style = dungeonStyle;
            this.size = size;
            center = new Vector2f();

            tileData = new Tile[size.X, size.Y];
            detailData = new Tile[size.X, size.Y];

            for (var x = 0; x < size.X; x++)
            {
                for (var y = 0; y < size.Y; y++)
                {
                    tileData[x, y] = new Tile(TileType.NULL);
                    detailData[x, y] = new Tile(TileType.NULL);
                }
            }

            entityData = new Entity[size.X, size.Y];

            lightMap = new LightMap(size, 32);
        }

        public void AddFloor(int x, int y)
        {
            Tile t = new Tile(style.GetFloorTileType());

            t.SetPosition(new Vector2f(cellSize * x, cellSize * y));

            tileData[x, y] = t;
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

            Tile t = new Tile(style.GetWallTileType(direction));
            t.SetPosition(new Vector2f(cellSize * x, cellSize * y));

            if (replace)
            {
                tileData[x, y] = t;
            }
            else
            {
                tileData[x, y].children.Push(t);
            }
        }

        public void AddRoof(int x, int y)
        {
            if (!IsXInBounds(x) || !(IsYInBounds(y)))
            {
                return;
            }

            Tile t = new Tile(style.GetRoofTileType());
            t.SetPosition(new Vector2f(cellSize * x, cellSize * y));

            tileData[x, y] = t;
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
            Tile tile = new Tile(tileType);
            tile.SetPosition(new Vector2f(cellSize * x, cellSize * y));

            if (detailData[x, y].type == TileType.NULL)
            {
                detailData[x, y] = tile;
            }
            else
            {
                detailData[x, y].children.Push(tile);
            }
        }

        public bool IsTileEmpty(int x, int y)
        {
            return tileData[x, y].type == TileType.NULL;
        }

        public bool IsFloor(int x, int y)
        {
            return tileData[x, y].type == style.GetFloorTileType();
        }

        public bool IsRoof(int x, int y)
        {
            return tileData[x, y].type == style.GetRoofTileType();
        }

        public bool IsWall(int x, int y, Direction direction)
        {
            return tileData[x, y].type == style.GetWallTileType(direction);
        }

        public bool HoldsFloor(int x, int y)
        {
            TileType floor = style.GetFloorTileType();

            if (tileData[x, y].type == floor)
            {
                return true;
            }
            else
            {
                foreach (Tile t in tileData[x, y].children)
                {
                    if (t.type == floor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HoldsWall(int x, int y, Direction direction)
        {
            TileType wall = style.GetWallTileType(direction);

            if (tileData[x, y].type == wall)
            {
                return true;
            }
            else
            {
                foreach (Tile t in tileData[x, y].children)
                {
                    if (t.type == wall)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HoldsRoof(int x, int y)
        {
            TileType roof = style.GetRoofTileType();

            if (tileData[x, y].type == roof)
            {
                return true;
            }
            else
            {
                foreach (Tile t in tileData[x, y].children)
                {
                    if (t.type == roof)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsAdjacentToFloor(int x, int y)
        {
            Tile top = tileData[x, y + 1];
            Tile right = tileData[x + 1, y];
            Tile down = tileData[x, y - 1];
            Tile left = tileData[x - 1, y];

            TileType floor = style.GetFloorTileType();

            return top.Contains(floor) || right.Contains(floor) || down.Contains(floor) || left.Contains(floor);
        }

        public bool IsAdjacentToRoof(int x, int y)
        {
            Tile top = tileData[x, y + 1];
            Tile right = tileData[x + 1, y];
            Tile down = tileData[x, y - 1];
            Tile left = tileData[x - 1, y];

            TileType roof = style.GetRoofTileType();

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
            return x >= 0 && x < tileData.GetLength(0);
        }

        public bool IsYInBounds(int y)
        {
            return y >= 0 && y < tileData.GetLength(1);
        }

        public bool IsAreaUnused(int xStart, int yStart, int xEnd, int yEnd)
        {
            return ContainsOnly(xStart, yStart, xEnd, yEnd, TileType.NULL);
        }

        public bool ContainsOnly(int xStart, int yStart, int xEnd, int yEnd, TileType tileType)
        {
            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    if (tileData[x, y] == null || tileData[x, y].type != tileType)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void AddEntity(int x, int y, Entity a)
        {
            a.Position = new Vector2f(cellSize * x, cellSize * y);

            entityData[x, y] = a;
        }

        private void DrawBaseTiles(RenderTarget rt, RenderStates states, int startX, int startY, int endX, int endY)
        {
            for (var y = startY; y < endY; y++)
            {
                for (var x = startX; x < endX; x++)
                {
                    if (IsXInBounds(x) && IsYInBounds(y))
                    {
                        Tile t = tileData[x, y];
                        t.LightPass(lightMap.GetLightValue(x, y));
                        t.Draw(rt, states);
                    }
                }
            }
        }

        private void DrawDetailTiles(RenderTarget rt, RenderStates states, int startX, int startY, int endX, int endY)
        {
            for (var y = startY; y < endY; y++)
            {
                for (var x = startX; x < endX; x++)
                {
                    if (IsXInBounds(x) && IsYInBounds(y))
                    {
                        Tile t = detailData[x, y];
                        t.LightPass(lightMap.GetLightValue(x, y));
                        t.Draw(rt, states);
                    }
                }
            }
        }

        private void UpdateEntities(RenderTarget rt)
        {
            for (var y = 0; y < size.Y; y++)
            {
                for (var x = 0; x < size.X; x++)
                {
                    if (IsXInBounds(x) && IsYInBounds(y))
                    {
                        if (entityData[x, y] != null)
                        {
                            entityData[x, y].Update(rt, this);
                        }
                    }
                }
            }
        }

        public void Draw(RenderTarget rt, RenderStates states)
        {
            View view = rt.GetView();
            view.Center = center;
            rt.SetView(view);

            // get dimensions of viewing window
            Vector2i topLeft = new Vector2i((int)(view.Center.X - view.Size.X / 2f), (int)(view.Center.Y - view.Size.Y / 2f));
            Vector2i bottomRight = new Vector2i((int)(view.Center.X + view.Size.X / 2f), (int)(view.Center.Y + view.Size.Y / 2f));

            topLeft = new Vector2i((int)Math.Floor((float) topLeft.X / cellSize), (int) Math.Floor((float)topLeft.Y / cellSize));
            topLeft -= new Vector2i(2, 2);

            bottomRight /= cellSize;
            bottomRight += new Vector2i(2, 2);

            Vector2i dimensions = topLeft - bottomRight;

            lightMap.UpdateRegion(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

            DrawBaseTiles(rt, states, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
            UpdateEntities(rt);
            DrawDetailTiles(rt, states, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }
    }
}
