using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public readonly List<Entity> _entities;

        public readonly List<PointLight> _lights;

        public Color AmbientLightColor;

        public Vector2i Size { get; }

        public int CellSize { get; }

        public Vector2f Center;

        private readonly RenderTexture _lightTexture;

        private readonly Sprite _lightSprite;

        public Map(string name, Vector2i size, int cellSize = 32)
        {
            Name = name;

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

            _lights = new List<PointLight>();

            AmbientLightColor = Color.White;

            _lightTexture = new RenderTexture((uint) (size.X * cellSize), (uint) (size.Y * cellSize));

            _lightSprite = new Sprite(_lightTexture.Texture);
        }

        public Map(string name, int x, int y, int cellSize = 32) : this(name, new Vector2i(x, y), cellSize)
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
                    foreach (var l in _lights)
                    {
                        l.VisMap.AddRectangleOccluder(new FloatRect(x * 32, y * 32, 32, 32));
                    }
                    break;
                default:
                    Layer2Cells[x, y].AddTile(type);
                    Layer2Cells[x, y].SetPosition(x * CellSize, y * CellSize);
                    break;
            }
        }

        public void RemoveTile(int x, int y, uint layer)
        {
            if (layer > 2) layer = 2;

            switch (layer)
            {
                case 0:
                    Layer0Cells[x, y].Clear();
                    break;
                case 1:
                    Layer1Cells[x, y].Clear();
                    foreach (var l in _lights)
                    {
                        l.VisMap.AddRectangleOccluder(new FloatRect(x * 32, y * 32, 32, 32));
                    }
                    break;
                default:
                    Layer2Cells[x, y].Clear();
                    break;
            }
        }

        public void AddEntity(Entity e) => _entities.Add(e);

        public void AddLight(PointLight l) => _lights.Add(l);

        public void Draw(RenderTarget target, RenderStates states)
        {
            var view = target.GetView();

            // get dimensions of viewing window
            var topLeft = new Vector2i((int) (view.Center.X - view.Size.X / 2f),
                (int) (view.Center.Y - view.Size.Y / 2f));
            var bottomRight = new Vector2i((int) (view.Center.X + view.Size.X / 2f),
                (int) (view.Center.Y + view.Size.Y / 2f));

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

            // draw world

            for (var x = topLeft.X; x < bottomRight.X; x++)
            {
                for (var y = topLeft.Y; y < bottomRight.Y; y++)
                {
                    // draw layer 0 tiles (floors, etc)
                    Layer0Cells[x, y].Draw(target, states);

                    // draw layer 1 tiles (walls, collidables)
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
                    if (Layer2Cells[x, y].IsEmpty()) continue;

                    Layer2Cells[x, y].Draw(target, states);
                }
            }

            // draw lights to light texture buffer
            _lightTexture.Clear(AmbientLightColor);

            foreach (var light in _lights)
                _lightTexture.Draw(light);

            _lightTexture.Display();

            // multiply the light texture with the screen
            _lightSprite.Draw(target, new RenderStates(BlendMode.Multiply));
        }

        public void Save(string filename)
        {
            var bw = new BinaryWriter(
                File.Open(filename, FileMode.Create));

            bw.Write(MapVersion);
            bw.Write(Name);

            bw.Write(Size.X);
            bw.Write(Size.Y);

            bw.Write(CellSize);

            bw.Write(_entities.Count);

            bw.Write(_lights.Count);

            for (var i = 0; i < Size.X; i++)
            {
                for (var j = 0; j < Size.Y; j++)
                {
                    Layer0Cells[i, j].Write(bw);
                    Layer1Cells[i, j].Write(bw);
                    Layer2Cells[i, j].Write(bw);
                }
            }

            foreach (var e in _entities)
                e.Write(bw);

            bw.Write(AmbientLightColor.R);
            bw.Write(AmbientLightColor.G);
            bw.Write(AmbientLightColor.B);

            foreach (var l in _lights)
                l.Write(bw);

            bw.Close();
        }

        public static Map Load(string filename)
        {
            var br = new BinaryReader(
                File.Open(filename, FileMode.Open));

            var version = br.ReadInt16();

            var name = br.ReadString();

            var size = new Vector2i(br.ReadInt32(), br.ReadInt32());

            var cellSize = br.ReadInt32();

            var entityCount = br.ReadInt32();
            var lightCount = br.ReadInt32();

            var map = new Map(name, size, cellSize);

            for (var i = 0; i < size.X; i++)
            {
                for (var j = 0; j < size.Y; j++)
                {
                    map.Layer0Cells[i, j] = MapCell.Read(br);
                    map.Layer0Cells[i, j].SetPosition(i * cellSize, j * cellSize);

                    map.Layer1Cells[i, j] = MapCell.Read(br);
                    map.Layer1Cells[i, j].SetPosition(i * cellSize, j * cellSize);

                    map.Layer2Cells[i, j] = MapCell.Read(br);
                    map.Layer2Cells[i, j].SetPosition(i * cellSize, j * cellSize);
                }
            }

            for (var i = 0; i < entityCount; i++)
                map.AddEntity(Entity.Read(br));

            map.AmbientLightColor = new Color(br.ReadByte(), br.ReadByte(), br.ReadByte());

            for (var i = 0; i < lightCount; i++)
                map.AddLight(PointLight.Read(br));

            br.Close();

            var sb = new StringBuilder();
            sb.AppendLine("Loaded map " + name);
            sb.AppendLine("\tVersion: " + version);
            sb.AppendLine("\tName: " + name);
            sb.AppendLine("\tSize: (" + size.X + ", " + size.Y + ")");
            sb.AppendLine("\tCell Size: " + cellSize);
            sb.AppendLine("\tEntity Count: " + entityCount);
            sb.AppendLine("\tLight Count: " + lightCount);

            Console.WriteLine(sb);

            return map;
        }
    }
}
