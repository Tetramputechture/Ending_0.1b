using System.Collections.Generic;
using Ending.GameLogic.DungeonTools;
using Ending.Lighting;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public class Map
    {
        public readonly MapCell[,] Layer0Cells, Layer1Cells, Layer2Cells;

        private readonly List<Entity> _entities;

        private readonly List<DynamicLight> _lights;

        private Vector3f _ambientLight;

        public Vector2i Size { get; }

        public int CellSize { get; }

        public Vector2f Center;

        public LightMap LightMap { get; }

        public Map(Vector2i size, int cellSize = 32)
        {
            Size = size;

            CellSize = cellSize;

            Center = new Vector2f();

            Layer0Cells = new MapCell[size.X, size.Y];
            Layer1Cells = new MapCell[size.X, size.Y];
            Layer2Cells = new MapCell[size.X, size.Y];

            for (var x = 0; x < Size.X; x++)
            {
                for (var y = 0; y < Size.Y; y++)
                {
                    Layer0Cells[x, y] = new MapCell();
                    Layer1Cells[x, y] = new MapCell();
                    Layer2Cells[x, y] = new MapCell();
                }
            }

            _entities = new List<Entity>();

            _lights = new List<DynamicLight>();

            _ambientLight = new Vector3f(1, 1, 1);

            LightMap = new LightMap(size, cellSize);
        }

        public Map(int x, int y, int cellSize = 32) : this(new Vector2i(x, y), cellSize)
        {
        }

        public void AddTile(Tile tile, int x, int y, uint layer)
        {
            tile.SetPosition(new Vector2f(x * CellSize, y * CellSize));

            if (layer > 2) layer = 2;

            switch (layer)
            {
                case 0:
                    Layer0Cells[x, y].AddTile(tile);
                    break;
                case 1:
                    Layer1Cells[x, y].AddTile(tile);
                    break;
                default:
                    Layer2Cells[x, y].AddTile(tile);
                    break;
            }
        }

        public void AddEntity(Entity e, int x, int y)
        {
            e.Position = new Vector2f(x * CellSize, y * CellSize);

            _entities.Add(e);
        }

        public void AddLight(DynamicLight l, int x, int y)
        {
            l.Position = new Vector2f(x * CellSize, y * CellSize);

            _lights.Add(l);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var view = target.GetView();
            view.Center = Center;
            target.SetView(view);

            // get dimensions of viewing window
            var topLeft = new Vector2i((int)(view.Center.X - view.Size.X / 2f), (int)(view.Center.Y - view.Size.Y / 2f));
            var bottomRight = new Vector2i((int)(view.Center.X + view.Size.X / 2f), (int)(view.Center.Y + view.Size.Y / 2f));

            topLeft /= CellSize;
            bottomRight /= CellSize;

            // pad / restrict bounds
            if (topLeft.X >= 2) topLeft.X -= 2;
            if (topLeft.X < 0) topLeft.X = 0;

            if (topLeft.Y >= 2) topLeft.Y -= 2;
            if (topLeft.Y < 0) topLeft.Y = 0;

            if (bottomRight.X < Size.X) bottomRight.X += 2;
            if (bottomRight.X > Size.X) bottomRight.X = Size.X;

            if (bottomRight.Y < Size.Y) bottomRight.Y += 2;
            if (bottomRight.Y > Size.Y) bottomRight.Y = Size.Y;

            LightMap.UpdateRegion(this, topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

            for (var x = topLeft.X; x < bottomRight.X; x++)
            {
                for (var y = topLeft.Y; y < bottomRight.Y; y++)
                {
                    var color = LightMap.GetLightValue(x, y);

                    // draw layer 0 tiles (floors, etc)
                    Layer0Cells[x, y].LightPass(color);
                    Layer0Cells[x, y].Draw(target, states);

                    // draw layer 1 tiles (walls, collidables)
                    Layer1Cells[x, y].LightPass(color);
                    Layer1Cells[x, y].Draw(target, states);
                }
            }

            // draw entities
            foreach (var e in _entities)
                e.Update(target, this);

            // draw layer 2 tiles (non - collidables)
            for (var x = topLeft.X; x < bottomRight.X; x++)
            {
                for (var y = topLeft.Y; y < bottomRight.Y; y++)
                {
                    var color = LightMap.GetLightValue(x, y);
                    Layer2Cells[x, y].LightPass(color);
                    Layer2Cells[x, y].Draw(target, states);
                }
            }
        }

    }
}
