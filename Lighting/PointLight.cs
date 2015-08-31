using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ending.SpriteTools;
using SFML.Graphics;
using SFML.System;

namespace Ending.Lighting
{
    public class PointLight : Drawable
    {
        public Vector2f Position;

        public Color Color;

        public float Radius;

        public float Power;

        public readonly VisbilityMap VisMap;

        private readonly Shader shader;

        public PointLight()
        {
            Position = new Vector2f();
            Color = new Color();
            Radius = 0;
            Power = 1;

            VisMap = new VisbilityMap(Position, Radius);

            shader = new Shader("shaders/lightShader.vert", "shaders/lightShader.frag");
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            VisMap.SetCenter(Position);
            VisMap.SetRadius(Radius);

            shader.SetParameter("lightPosition", Position);
            shader.SetParameter("lightColor", Color.Red);
            shader.SetParameter("lightRadius", Radius);

            target.Draw(VisMap.GetVisMesh(), new RenderStates(shader));
            //VisMap.TraceIntersectionLines(target);
        }

        public void Write(BinaryWriter bw)
        {
            foreach (var val in new[]
            {
                Position.X,
                Position.Y,
                Color.R,
                Color.G,
                Color.B,
                Color.A,
                Radius
            })

            bw.Write(val);
        }

        public static PointLight Read(BinaryReader br) => new PointLight()
        {
            Position = new Vector2f(br.ReadSingle(), br.ReadSingle()),
            Color = new Color(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte()),
            Radius = br.ReadSingle()
        };
    }
}
