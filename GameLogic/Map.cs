using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ending.GameLogic.DungeonTools;
using Ending.Input;
using Ending.Lighting;
using Ending.SpriteTools;
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

        private readonly Vector3f[,] _finalLightColors;

        public Vector3f AmbientLightColor;

        public Vector2i Size { get; }

        public int CellSize { get; }

        public Vector2f Center;

        private readonly List<Tuple<Vector2f, Vector2f>> _segments;

        private bool _segmentsNeedUpdate;

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

            _finalLightColors = new Vector3f[size.X, size.Y];

            AmbientLightColor = new Vector3f(1, 1, 1);

            _segments = new List<Tuple<Vector2f, Vector2f>>();

            _segmentsNeedUpdate = true;
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
                    _segmentsNeedUpdate = true;
                    break;
                default:
                    Layer2Cells[x, y].AddTile(type);
                    Layer2Cells[x, y].SetPosition(x * CellSize, y * CellSize);
                    break;
            }
        }

        public void AddEntity(Entity e) => _entities.Add(e);

        public void AddLight(PointLight l) => _lights.Add(l);

        public void Draw(RenderTarget target, RenderStates states)
        {
            var view = target.GetView();
            view.Center = Center;
            target.SetView(view);

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

            if (_segmentsNeedUpdate)
            {
                UpdateSegments();
                _segmentsNeedUpdate = false;
            }

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
                    if (Layer2Cells[x, y].IsEmpty()) continue;

                    var color = GetLightColor(x, y);

                    Layer2Cells[x, y].LightPass(color);
                    Layer2Cells[x, y].Draw(target, states);
                }
            }

            //// get all unique endpoints
            //var points = new HashSet<Vector2f>();
            //foreach (var v in _segments)
            //{
            //    points.Add(v.Item1);
            //    points.Add(v.Item2);
            //}

            //// get all angles
            //var angles = new List<float>();
            //foreach (
            //    var fAngle in
            //        points.Select(p => Math.Atan2(p.Y - Center.Y, p.X - Center.X)).Select(angle => (float)angle))
            //{
            //    angles.Add(fAngle - 0.0001f);
            //    angles.Add(fAngle);
            //    angles.Add(fAngle + 0.0001f);
            //}

            //// rays in all directions
            //var intersections = new List<Ray>();
            //foreach (var angle in angles)
            //{
            //    var dx = Math.Cos(angle);
            //    var dy = Math.Sin(angle);

            //    var rayStart = Center;
            //    var rayEnd = new Vector2f(Center.X + (float)dx, Center.Y + (float)dy);

            //    // find closest intersection
            //    Ray closestIntersect = null;

            //    foreach (var s in _segments)
            //    {
            //        var intersect = GetIntersection(rayStart, rayEnd, s.Item1, s.Item2);
            //        if (intersect == null) continue;

            //        if (closestIntersect == null || intersect.Length < closestIntersect.Length)
            //            closestIntersect = intersect;
            //    }

            //    if (closestIntersect == null) continue;

            //    closestIntersect.Angle = angle;

            //    intersections.Add(closestIntersect);
            //}

            //// sort intersects by angle
            //intersections.Sort((a, b) => a.Angle.CompareTo(b.Angle));

            //// make visibility mesh
            //var mesh = new VertexArray(PrimitiveType.TrianglesFan);

            //var lightColor = new Color(217, 217, 217, 50);

            //var radius = 128;

            //mesh.Append(new Vertex(Center, lightColor));

            //foreach (var hit in intersections)
            //{
            //    mesh.Append(new Vertex(hit.Position, lightColor));
            //    //TraceLine(target, Center.X, Center.Y, hit.Position.X, hit.Position.Y);
            //}

            //mesh.Append(new Vertex(intersections[0].Position, lightColor));

            //target.Draw(mesh);

            foreach (var light in _lights)
                target.Draw(light);
        }

        // find intersection of ray and segment
        // returns <x, y, T1>
        private static Ray GetIntersection(Vector2f rayStart, Vector2f rayEnd,
            Vector2f segmentStart, Vector2f segmentEnd)
        {
            // ray in parametric form: Point + Direction * T1
            var rPx = rayStart.X;
            var rPy = rayStart.Y;
            var rDx = rayEnd.X - rPx;
            var rDy = rayEnd.Y - rPy;

            // segment in parametric form: Point * Direction * T2
            var sPx = segmentStart.X;
            var sPy = segmentStart.Y;
            var sDx = segmentEnd.X - sPx;
            var sDy = segmentEnd.Y - sPy;

            // if lines are parallel, no intersection
            var rMag = Math.Sqrt(rDx * rDx + rDy * rDy);
            var sMag = Math.Sqrt(sDx * sDx + sDy * sDy);

            const float tolerance = 0.00001f;

            if (Math.Abs(rDx / rMag - sDx / sMag) < tolerance && Math.Abs(rDy / rMag - sDy / sMag) < tolerance)
                // directions are the same
                return null;

            // solve for T1 and T2
            var t2 = (rDx * (sPy - rPy) + rDy * (rPx - sPx)) / (sDx * rDy - sDy * rDx);
            var t1 = (sPx + sDx * t2 - rPx) / rDx;

            // must be within parametric bounds
            if (t1 < 0 || t2 < 0 || t2 > 1)
                return null;

            // return the point of intersection
            return new Ray()
            {
                Position = new Vector2f(rPx + rDx * t1, rPy + rDy * t1),
                Length = t1,
                Angle = 0
            };
        }

        public void TraceLine(RenderTarget rt, float x0, float y0, float x1, float y1)
        {
            var line = new[]
            {
                new Vertex(new Vector2f(x0, y0), Color.Red),
                new Vertex(new Vector2f(x1, y1), Color.Red)
            };

            rt.Draw(line, PrimitiveType.Lines);

            var endPoint = new CircleShape(2)
            {
                Origin = new Vector2f(1, 1),
                Position = new Vector2f(x1, y1),
                FillColor = Color.Red
            };

            rt.Draw(endPoint);
        }

        private Color GetLightColor(int x, int y)
        {
            var color = _finalLightColors[x, y];

            color.X = Math.Max(0, Math.Min(1f, color.X));
            color.Y = Math.Max(0, Math.Min(1f, color.Y));
            color.Z = Math.Max(0, Math.Min(1f, color.Z));

            return new Color((byte) (AmbientLightColor.X * 255f), (byte) (AmbientLightColor.X * 255f), (byte) (AmbientLightColor.X * 255f));
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

            bw.Write(AmbientLightColor.X);
            bw.Write(AmbientLightColor.Y);
            bw.Write(AmbientLightColor.Z);

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

            map.AmbientLightColor = new Vector3f(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

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

        private void UpdateSegments()
        {
            _segments.Clear();

            // add map bounds
            var mapWidth = Size.X * CellSize;
            var mapHeight = Size.Y * CellSize;

            // top
            AddSegment(new Vector2f(), new Vector2f(mapWidth, 0));

            // right
            AddSegment(new Vector2f(mapWidth, 0), new Vector2f(mapWidth, mapHeight));

            // bottom
            AddSegment(new Vector2f(mapWidth, mapHeight), new Vector2f(0, mapHeight));

            // left
            AddSegment(new Vector2f(0, mapHeight), new Vector2f());

            for (var i = 0; i < Size.X; i++)
            {
                for (var j = 0; j < Size.Y; j++)
                {
                    if (Layer1Cells[i, j].IsEmpty()) continue;

                    // add each vector of the cell's bounds
                    var rect = new IntRect(0, 0, 32, 32)
                    {
                        Left = i * CellSize,
                        Top = j * CellSize
                    };

                    // top
                    AddSegment(
                        new Vector2f(rect.Left, rect.Top),
                        new Vector2f(rect.Left + rect.Width, rect.Top));

                    // right
                    AddSegment(
                        new Vector2f(rect.Left + rect.Width, rect.Top),
                        new Vector2f(rect.Left + rect.Width, rect.Top + rect.Height));

                    // bottom
                    AddSegment(
                        new Vector2f(rect.Left + rect.Width, rect.Top + rect.Height),
                        new Vector2f(rect.Left, rect.Top + rect.Height));

                    // left
                    AddSegment(
                        new Vector2f(rect.Left, rect.Top + rect.Height),
                        new Vector2f(rect.Left, rect.Top));
                }
            }
        }

        private void AddSegment(Vector2f start, Vector2f end)
            => _segments.Add(new Tuple<Vector2f, Vector2f>(start, end));
    }
}
