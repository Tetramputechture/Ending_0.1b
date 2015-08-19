using SFML.System;

namespace Ending.Lighting
{
    public class DynamicLight
    {
        public Vector2f Position;

        public Vector3f Color;

        public float Radius;

        public DynamicLight()
        {
            Position = new Vector2f();
            Color = new Vector3f();
            Radius = 0;
        }
    }
}
