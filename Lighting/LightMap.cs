using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.Lighting
{
    public class LightMap
    {
        private List<Vector3f> staticLights, finalLights;

        private List<DynamicLight> dynamicLights;

        public Vector3f ambient;

        private Vector2i size;

        private int nodeSize;

        public LightMap(Vector2i size, int nodeSize)
        {
            staticLights = new List<Vector3f>();
            finalLights = new List<Vector3f>();
            dynamicLights = new List<DynamicLight>();

            this.size = size;
            this.nodeSize = nodeSize;

            ClearStatic();

            ambient = new Vector3f(1, 1, 1);
        }

        public void Resize(Vector2i size)
        {
            this.size = size;
            ClearStatic();
            ClearDynamic();
        }

        public void ClearStatic()
        {
            staticLights.Clear();
            finalLights.Clear();

            int area = size.X * size.Y;

            for (int i = 0; i < area; i++)
            {
                staticLights.Add(new Vector3f());
                finalLights.Add(new Vector3f());
            }
        }

        public void ClearDynamic()
        {
            dynamicLights.Clear();
        }

        public void UpdateRegion(int startX, int startY, int endX, int endY)
        {
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    if (x < 0 || x >= size.X || y < 0 || y >= size.Y)
                    {
                        continue;
                    }
                    // calculate coord of center of point
                    float cx = x * nodeSize + nodeSize / 2f;
                    float cy = y * nodeSize + nodeSize / 2f;

                    Vector3f staticLight = staticLights[y * size.X + x];
                    Vector3f light = new Vector3f();

                    // iterate dynamic lights
                    foreach (DynamicLight l in dynamicLights)
                    {
                        float dx = cx - l.position.X;
                        float dy = cy - l.position.Y;

                        if (Math.Abs(dx) <= l.radius && Math.Abs(dy) <= l.radius)
                        {
                            // light is in range of point
                            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                            float intensity = (l.radius - distance) / l.radius;
                            intensity = Math.Max(0, Math.Min(1f, intensity)); // clamp

                            light.X += l.color.X * intensity;
                            light.Y += l.color.Y * intensity;
                            light.Z += l.color.Z * intensity;
                        }
                    }

                    finalLights[y * size.X + x] = new Vector3f(ambient.X + staticLight.X + light.X, ambient.Y + staticLight.Y + light.Y, ambient.Z + staticLight.Z + light.Z);

                }

            }
        }

        public DynamicLight RequestLight()
        {
            dynamicLights.Add(new DynamicLight());
            return dynamicLights.LastOrDefault();
        }

        public void ReleaseLight(DynamicLight light)
        {
            dynamicLights.Remove(light);
        }

        public Color GetLightValue(int x, int y)
        {
            if (x < 0 || x >= size.X || y < 0 || y >= size.Y)
            {
                return new Color(0, 0, 0, 255);
            }

            Vector3f color = finalLights[y * size.X + x];

            color.X = Math.Max(0, Math.Min(1f, color.X));
            color.Y = Math.Max(0, Math.Min(1f, color.Y));
            color.Z = Math.Max(0, Math.Min(1f, color.Z));

            return new Color((byte)(color.X * 255f), (byte)(color.Y * 255f), (byte)(color.Z * 255f));
        }

        public void AddStaticLightCell(int x, int y, Color c)
        {
            if (x < 0 || x >= size.X || y < 0 || y >= size.Y)
            {
                return;
            }

            staticLights[y * size.X + x] = new Vector3f(c.R, c.G, c.B);
        }

        public void AddStaticLightRadius(float x, float y, Color c, float radius)
        {
            int minX = (int)((x - radius) / (nodeSize)) - 1;
            int minY = (int)((y - radius) / (nodeSize)) - 1;
            int maxX = (int)((x + radius) / (nodeSize)) + 1;
            int maxY = (int)((y + radius) / (nodeSize)) + 1;

            for (int cx = minX; cx <= maxX; cx++)
            {
                for (int cy = minY; cy <= maxY; cy++)
                {
                    if (cx >= 0 && cx < size.X && cy >= 0 && cy < size.Y)
                    {
                        Vector3f st = staticLights[cy * size.X + cx];

                        float cellx = cx * nodeSize + nodeSize / 2f;
                        float celly = cy * nodeSize + nodeSize / 2f;

                        float dx = cellx - x;
                        float dy = celly - y;

                        float distance = (float) Math.Sqrt(dx * dx + dy * dy);
                        float intensity = (radius - distance) / radius;

                        intensity = Math.Max(0, Math.Min(1f, intensity)); // clamp

                        st.X += c.R * intensity;
                        st.Y += c.G * intensity;
                        st.Z += c.B * intensity;

                        staticLights[cy * size.X + cx] = st;
                    }
                }
            }
        }
    }
}
