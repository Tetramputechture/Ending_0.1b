using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Ending.GameLogic.DungeonTools;
using Ending.Lighting;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public class Map
    {
        private const short MapVersion = 1;

        public string Name;

        public readonly MapCell[,] Layer0Cells, Layer1Cells, Layer2Cells;

        private readonly List<Entity> _entities;

        private readonly List<DynamicLight> _lights;

        private readonly Vector3f[,] _finalLightColors;

        public Vector3f AmbientLightColor;

        public Vector2i Size { get; }

        public int CellSize { get; }

        public Vector2f Center;

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

            _finalLightColors = new Vector3f[size.X, size.Y];

            AmbientLightColor = new Vector3f(1, 1, 1);
        }

        public Map(int x, int y, int cellSize = 32) : this(new Vector2i(x, y), cellSize)
        {
        }

        public void AddTile(TileType type, int x, int y, uint layer)
        {
            if (layer > 2) layer = 2;

            switch (layer)
            {
                case 0:
                    Layer0Cells[x, y].AddTile(type);
                    Layer0Cells[x, y].SetPosition(x * CellSize, y * CellSize);
                    break;
                case 1:
                    Layer1Cells[x, y].AddTile(type);
                    Layer1Cells[x, y].SetPosition(x * CellSize, y * CellSize);
                    break;
                default:
                    Layer2Cells[x, y].AddTile(type);
                    Layer2Cells[x, y].SetPosition(x * CellSize, y * CellSize);
                    break;
            }
        }

        public void AddEntity(Entity e) => _entities.Add(e);

        public void AddLight(DynamicLight l) => _lights.Add(l);

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

            UpdateLightmapRegion(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);

            for (var x = topLeft.X; x < bottomRight.X; x++)
            {
                for (var y = topLeft.Y; y < bottomRight.Y; y++)
                {
                    var color = GetLightColor(x, y);

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
                    var color = GetLightColor(x, y);

                    Layer2Cells[x, y].LightPass(color);
                    Layer2Cells[x, y].Draw(target, states);
                }
            }
        }

        private void UpdateLightmapRegion(int x0, int y0, int x1, int y1)
        {
            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    // calculate coord of center of point
                    var cx = x * CellSize + CellSize / 2f;
                    var cy = y * CellSize + CellSize / 2f;

                    var light = new Vector3f();

                    // iterate dynamic lights
                    foreach (var l in _lights)
                    {
                        var dx = cx - l.Position.X;
                        var dy = cy - l.Position.Y;

                        if (Math.Abs(dx) > l.Radius || Math.Abs(dy) > l.Radius || !CheckLine((int)l.Position.X, (int)l.Position.Y, (int)cx, (int)cy)) continue;

                        // light is in range of point, and not blocked
                        var distance = (float)Math.Sqrt(dx * dx + dy * dy);
                        var intensity = (l.Radius - distance) / l.Radius;
                        intensity = Math.Max(0, Math.Min(1f, intensity)); // clamp

                        light.X += l.Color.X * intensity;
                        light.Y += l.Color.Y * intensity;
                        light.Z += l.Color.Z * intensity;
                    }

                    _finalLightColors[x, y] = AmbientLightColor + light;

                }

            }
        }

        private bool CheckLine(int x0, int y0, int x1, int y1)
        {
            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);

            var x = x0;
            var y = y0;

            var n = 1 + dx + dy;

            var xInc = (x1 > x0) ? 1 : -1;
            var yInc = (y1 > y0) ? 1 : -1;

            var error = dx - dy;

            dx *= 2;
            dy *= 2;

            for (; n > 0; n--)
            {
                var cellX = x / CellSize;
                var cellY = y / CellSize;
                if (!Layer1Cells[cellX, cellY].IsEmpty())
                    return false;

                if (error > 0)
                {
                    x += xInc;
                    error -= dy;
                }
                else
                {
                    y += yInc;
                    error += dx;
                }
            }

            return true;
        }

        private Color GetLightColor(int x, int y)
        {
            var color = _finalLightColors[x, y];

            color.X = Math.Max(0, Math.Min(1f, color.X));
            color.Y = Math.Max(0, Math.Min(1f, color.Y));
            color.Z = Math.Max(0, Math.Min(1f, color.Z));

            return new Color((byte)(color.X * 255f), (byte)(color.Y * 255f), (byte)(color.Z * 255f));
        }

        public void Save(string filename)
        {
            var bw = new BinaryWriter(
                File.Open(filename, FileMode.Create));

            bw.Write(MapVersion);
            bw.Write(Name);

        }

    }
}
