using System.IO;
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

        public void Write(BinaryWriter bw)
        {
            foreach (var val in new[]
            {
                Position.X,
                Position.Y,
                Color.X,
                Color.Y,
                Color.Z,
                Radius
            })
                bw.Write(val);
        }

        public static DynamicLight Read(BinaryReader br) => new DynamicLight()
        {
            Position = new Vector2f(br.ReadSingle(), br.ReadSingle()),
            Color = new Vector3f(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
            Radius = br.ReadSingle()
        };
    }
}
