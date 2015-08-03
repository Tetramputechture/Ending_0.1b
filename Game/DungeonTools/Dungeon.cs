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
    public class Dungeon
    {
        public DungeonStyle dungeonStyle { get; }

        public Vector2i size { get; }

        public Tile[,] tileData { get; }

        private Tile[,] detailData;

        private Entity[,] entityData;

        private bool compiled;

        public Dungeon(DungeonStyle dungeonStyle, Vector2i size)
        {
            this.dungeonStyle = dungeonStyle;
            this.size = size;
            tileData = new Tile[size.X, size.Y];
            detailData = new Tile[size.X, size.Y];

            TileType unused = dungeonStyle.GetUnusedTileType();
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    AddTile(x, y, unused);
                }
            }
            entityData = new Entity[size.X, size.Y];
        }

        public void AddTile(int x, int y, TileType tileType)
        {
            Tile tile = new Tile(tileType);

            tileData[x, y] = tile;
        }

        /**
         * Sets a cell to a Detail Tile (Drawn over entities)
         *
         * @param x the x coordinate of the cell to be set.
         * @param y the y coordinate of the cell to be set.
         * @param tileType the Tile to set thIs cell to.
         */
        public void AddDetail(int x, int y, TileType tileType)
        {
            detailData[x, y] = new Tile(tileType);
        }

        public void PushTile(int x, int y, TileType tileType)
        {
            Tile tile = new Tile(tileType);

            if (tileData[x, y] == null)
            {
                tileData[x, y] = tile;
            }
            else
            {
                tileData[x, y].children.Push(tile);
            }
        }

        public void PushDetail(int x, int y, TileType tileType)
        {
            Tile tile = new Tile(tileType);

            if (detailData[x, y] == null)
            {
                detailData[x, y] = tile;
            }
            else
            {
                detailData[x, y].children.Push(tile);
            }
        }

        /**
         * Sets a range of cells to a tile.
         *
         * @param xStart the starting x value of the range.
         * @param yStart the starting y value of the range.
         * @param xEnd the ending x value of the range.
         * @param yEnd the ending y value of the range.
         * @param tileType the tile type to set the range of cells to.
         */
        public void AddTiles(int xStart, int yStart, int xEnd, int yEnd, TileType tileType)
        {
            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    AddTile(x, y, tileType);
                }
            }
        }

        public bool GetTileTypeEquals(int x, int y, TileType tileType)
        {
            return tileData[x, y].type == tileType;
        }

        public bool holdsTileType(int x, int y, TileType tileType)
        {
            if (tileData[x, y].type == tileType)
            {
                return true;
            }
            else
            {
                foreach (Tile t in tileData[x, y].children)
                {
                    if (t.type == tileType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool holdsDetailType(int x, int y, TileType tileType)
        {
            if (detailData[x, y] == null)
            {
                return false;
            }

            if (detailData[x, y].type == tileType)
            {
                return true;
            }
            else
            {
                foreach (Tile t in detailData[x, y].children)
                {
                    if (t.type == tileType)
                    {
                        return true;
                    }
                }
            }
            return false;
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

        /**
         * Returns if an integer Is within thIs Dungeon's number of columns.
         *
         * @param x the x coordinate to be checked.
         * @return <code>true</code> if 0 &lt= x &lt= Dungeon Columns,
         * <code>false</code> otherwIse.
         */
        public bool IsXInBounds(int x)
        {
            return x >= 0 && x < tileData.GetLength(0);
        }

        /**
         * Returns if an integer Is within thIs Dungeon's number of rows.
         *
         * @param y the y coordinate to be checked.
         * @return true if 0 &lt= x &lt= Dungeon Rows, false otherwIse.
         */
        public bool IsYInBounds(int y)
        {
            return y >= 0 && y < tileData.GetLength(1);
        }

        /**
         * Returns if a range of cells Is of <code>Tile.UNUSED.</code>
         *
         * @param xStart the starting x value of the range.
         * @param yStart the starting y value of the range.
         * @param xEnd the ending x value of the range.
         * @param yEnd the ending y value of the range.
         * @return <code>true</code> if each cell within the range Is of type
         * <code>Tile.UNUSED</code>, <code>false</code> otherwIse.
         */
        public bool IsAreaUnused(int xStart, int yStart, int xEnd, int yEnd)
        {
            return containsOnly(xStart, yStart, xEnd, yEnd, dungeonStyle.GetUnusedTileType());
        }

        /**
         * If a range of cells if of a specified tile type.
         * @param xStart the starting x value of the range.
         * @param yStart the starting y value of the range.
         * @param xEnd the ending x value of the range.
         * @param yEnd the ending y value of the range.
         * @param tileType the TileType to check for.
         * @return <code>true</code> if each cell within the range Is of type
         * <code>tileType</code>, <code>false</code> otherwIse.
         */
        public bool containsOnly(int xStart, int yStart, int xEnd, int yEnd, TileType tileType)
        {
            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    if (tileData[x, y].type != tileType)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /**
         * Returns if a cell Is adjacent to a cell of a certain tile.
         *
         * @param x the x coordinate of the cell.
         * @param y the y coordinate of the cell.
         * @param tileType the tile  to be checked if the cell Is adjacent to.
         * @return <code>true</code> if the cell has another cell of type
         * <code>tileType</code> adjacent (to the first cell north, east, south, or
         * west) to it, <code>false</code> otherwIse
         */
        public bool IsAdjacent(int x, int y, TileType tileType)
        {
            return holdsTileType(x - 1, y, tileType) || holdsTileType(x + 1, y, tileType)
                    || holdsTileType(x, y - 1, tileType) || holdsTileType(x, y + 1, tileType);
        }

        public void AddEntity(int x, int y, Entity a)
        {
            entityData[x, y] = a;
        }

        private void compile()
        {
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    Vector2f pos = new Vector2f(x * Tile.TILE_WIDTH, y * Tile.TILE_HEIGHT);

                    tileData[x, y].SetPosition(pos);

                    Entity e = entityData[x, y];
                    if (e != null)
                    {
                        e.Position = pos;
                    }

                    Tile d = detailData[x, y];
                    if (d != null)
                    {
                        d.SetPosition(pos);
                    }
                }
            }
            compiled = true;
        }

        private void DrawBaseTiles(RenderTarget rt, RenderStates states)
        {
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    if (tileData[x, y] != null)
                    {
                        rt.Draw(tileData[x, y]);
                    }
                }
            }
        }

        private void DrawDetailTiles(RenderTarget rt, RenderStates states)
        {
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    if (detailData[x, y] != null)
                    {
                        rt.Draw(detailData[x, y]);
                    }
                }
            }
        }

        private void UpdateEntities(Time deltaTime, RenderTarget rt)
        {
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    if (entityData[x, y] != null)
                    {
                        entityData[x, y].Update(rt, this, deltaTime);
                    }
                }
            }
        }

        public void Update(RenderTarget rt, RenderStates states, Time deltaTime)
        {
            if (!compiled)
            {
                compile();
            }
            DrawBaseTiles(rt, states);
            UpdateEntities(deltaTime, rt);
            DrawDetailTiles(rt, states);
        }
    }
}

