using System;
using System.Collections.Generic;
using System.Linq;
using Ending.GameLogic;
using SFML.Graphics;
using SFML.System;

namespace Ending.Lighting
{
    public class LightMap
    {
        private readonly Vector3f[,] _staticLights;
        private readonly Vector3f[,] _dynamicLightValues;
        private readonly Vector3f[,] _finalLights;

        private readonly List<DynamicLight> _dynamicLights;

        public Vector3f Ambient;

        private Vector2i _size;

        private readonly int _cellSize;

        public LightMap(Vector2i size, int cellSize)
        {
            _staticLights = new Vector3f[size.X, size.Y];
            _dynamicLightValues = new Vector3f[size.X, size.Y];
            _finalLights = new Vector3f[size.X, size.Y];

            _dynamicLights = new List<DynamicLight>();

            _size = size;
            _cellSize = cellSize;

            ClearStatic();

            Ambient = new Vector3f(1, 1, 1);
        }

        public void Resize(Vector2i size)
        {
            _size = size;
            ClearStatic();
            ClearDynamic();
        }

        public void ClearStatic()
        {
            for (var x = 0; x < _size.X; x++)
            {
                for (var y = 0; y < _size.Y; y++)
                {
                    _staticLights[x, y] = new Vector3f();
                }
            }
        }

        public void ClearDynamic()
        {
            //_dynamicLights.Clear();
            for (var x = 0; x < _size.X; x++)
            {
                for (var y = 0; y < _size.Y; y++)
                {
                    _dynamicLightValues[x, y] = new Vector3f();
                }
            }
        }

        public void UpdateRegion(Map map, int startX, int startY, int endX, int endY)
        {
            for (var x = startX; x < endX; x++)
            {
                for (var y = startY; y < endY; y++)
                {
                    // calculate coord of center of point
                    var cx = x * _cellSize + _cellSize / 2f;
                    var cy = y * _cellSize + _cellSize / 2f;

                    var staticLight = _staticLights[x, y];
                    var light = new Vector3f();

                    // iterate dynamic lights
                    foreach (var l in _dynamicLights)
                    {
                        var dx = cx - l.Position.X;
                        var dy = cy - l.Position.Y;

                        if (Math.Abs(dx) > l.Radius || Math.Abs(dy) > l.Radius || !CheckLine(map, l.Position.X, l.Position.Y, cx, cy)) continue;

                        // light is in range of point, and not blocked
                        var distance = (float)Math.Sqrt(dx * dx + dy * dy);
                        var intensity = (l.Radius - distance) / l.Radius;
                        intensity = Math.Max(0, Math.Min(1f, intensity)); // clamp

                        light.X += l.Color.X * intensity;
                        light.Y += l.Color.Y * intensity;
                        light.Z += l.Color.Z * intensity;
                    }

                    _finalLights[x, y] = new Vector3f(Ambient.X + staticLight.X + light.X, Ambient.Y + staticLight.Y + light.Y, Ambient.Z + staticLight.Z + light.Z);

                }

            }
        }

        private bool CheckLine(Map map, float x0, float y0, float x1, float y1)
        {
            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);

            var x = (int)Math.Floor(x0);
            var y = (int)Math.Floor(y0);

            var n = 1;
            int xInc, yInc;

            float error;

            if (dx == 0)
            {
                xInc = 0;
                error = float.PositiveInfinity;
            }
            else if (x1 > x0)
            {
                xInc = 1;
                n += (int)Math.Floor(x1) - x;
                error = (float)(Math.Floor(x0) + 1 - x0) * dy;
            }
            else
            {
                xInc = -1;
                n += x - (int)Math.Floor(x1);
                error = (float)(x0 - Math.Floor(x0)) * dy;
            }

            if (dy == 0)
            {
                yInc = 0;
                error -= float.PositiveInfinity;
            }
            else if (y1 > y0)
            {
                yInc = 1;
                n += (int)Math.Floor(y1) - y;
                error -= (float)(Math.Floor(y0) + 1 - y0) * dx;
            }
            else
            {
                yInc = -1;
                n += y - (int)Math.Floor(y1);
                error -= (float)(y0 - Math.Floor(y0)) * dx;
            }

            for (; n > 0; n--)
            {
                var cellX = x / _cellSize;
                var cellY = y / _cellSize;
                if (!map.Layer1Cells[cellX, cellY].IsEmpty())
                    return false;

                if (error > 0)
                {
                    y += yInc;
                    error -= dx;
                }
                else
                {
                    x += xInc;
                    error += dy;
                }
            }

            return true;
        }

        public void UpdateRegionNew(int startX, int startY, int endX, int endY)
        {
            Func<float, float, bool> lightInRange = (x, y) => (x >= startX && x < endX && y >= startY && y < endY);

            ClearDynamic();

            foreach (var light in _dynamicLights.Where(light => lightInRange((float)Math.Floor(light.Position.X / _cellSize), (float)Math.Floor(light.Position.Y / _cellSize))))
            {
                var lightTileX = (int)light.Position.X / _cellSize;
                var lightTileY = (int)light.Position.Y / _cellSize;

                var lightTileRadius = (int)Math.Floor(light.Radius / _cellSize);

                for (var i = 0; i < 2; i++)
                {
                    DrawCircle((x, y) =>
                    {
                        BresenLine((bX, bY) =>
                        {
                            _dynamicLightValues[bX, bY] += new Vector3f(0.1f, 0.1f, 0.1f);
                        }, lightTileX, lightTileY, x, y);
                    }, lightTileX, lightTileY, lightTileRadius - i);
                }
            }

            for (var x = startX; x < endX; x++)
            {
                for (var y = startY; y < endY; y++)
                {
                    var staticLight = _staticLights[x, y];
                    var dynamicLight = _dynamicLightValues[x, y];

                    _finalLights[x, y] = new Vector3f(Ambient.X + staticLight.X + dynamicLight.X, Ambient.Y + staticLight.Y + dynamicLight.Y, Ambient.Z + staticLight.Z + dynamicLight.Z);
                }

            }
        }

        public void DrawCircle(Action<int, int> drawPixel, int x0, int y0, int radius)
        {
            var x = radius;
            var y = 0;
            var decisionOver2 = 1 - x;   // Decision criterion divided by 2 evaluated at x=r, y=0

            while (x >= y)
            {
                drawPixel(x + x0, y + y0);
                drawPixel(y + x0, x + y0);
                drawPixel(-x + x0, y + y0);
                drawPixel(-y + x0, x + y0);
                drawPixel(-x + x0, -y + y0);
                drawPixel(-y + x0, -x + y0);
                drawPixel(x + x0, -y + y0);
                drawPixel(y + x0, -x + y0);
                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2 * y + 1;   // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;   // Change for y -> y+1, x -> x-1
                }
            }
        }

        private static void BresenLine(Action<int, int> onLine, int x0, int y0, int x1, int y1)
        {
            var dx = Math.Abs(x1 - x0);
            var sx = x0 < x1 ? 1 : -1;

            var dy = Math.Abs(y1 - y0);
            var sy = y0 < y1 ? 1 : -1;

            var err = (dx > dy ? dx : -dy) / 2;

            for (;;)
            {
                onLine(x0, y0);

                if (x0 == x1 && y0 == y1)
                    break;

                var e2 = err;
                if (e2 > -dx)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dy)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        public DynamicLight RequestLight()
        {
            _dynamicLights.Add(new DynamicLight());
            return _dynamicLights.LastOrDefault();
        }

        public void ReleaseLight(DynamicLight light) => _dynamicLights.Remove(light);

        public Color GetLightValue(int x, int y)
        {
            if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y)
            {
                return new Color(0, 0, 0, 255);
            }

            var color = _finalLights[x, y];

            color.X = Math.Max(0, Math.Min(1f, color.X));
            color.Y = Math.Max(0, Math.Min(1f, color.Y));
            color.Z = Math.Max(0, Math.Min(1f, color.Z));

            return new Color((byte)(color.X * 255f), (byte)(color.Y * 255f), (byte)(color.Z * 255f));
        }

        public void AddStaticLightCell(int x, int y, Color c)
        {
            if (x < 0 || x >= _size.X || y < 0 || y >= _size.Y)
                return;

            _staticLights[x, y] = new Vector3f(c.R, c.G, c.B);
        }

        public void AddStaticLightRadius(float x, float y, Color c, float radius)
        {
            var minX = (int)((x - radius) / (_cellSize)) - 1;
            var minY = (int)((y - radius) / (_cellSize)) - 1;
            var maxX = (int)((x + radius) / (_cellSize)) + 1;
            var maxY = (int)((y + radius) / (_cellSize)) + 1;

            for (var cx = minX; cx <= maxX; cx++)
            {
                for (var cy = minY; cy <= maxY; cy++)
                {
                    if (cx < 0 || cx >= _size.X || cy < 0 || cy >= _size.Y) continue;

                    var st = new Vector3f();

                    var cellx = cx * _cellSize + _cellSize / 2f;
                    var celly = cy * _cellSize + _cellSize / 2f;

                    var dx = cellx - x;
                    var dy = celly - y;

                    var distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    var intensity = (radius - distance) / radius;

                    intensity = Math.Max(0, Math.Min(1f, intensity)); // clamp

                    st.X += c.R * intensity;
                    st.Y += c.G * intensity;
                    st.Z += c.B * intensity;

                    _staticLights[cx, cy] = st;
                }
            }
        }
    }
}
