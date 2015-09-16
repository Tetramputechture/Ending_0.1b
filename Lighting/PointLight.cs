using System.IO;
using Ending.GameState;
using Ending.GameWindow;
using SFML.Graphics;
using SFML.System;

namespace Ending.Lighting
{
    public class PointLight : Drawable
    {
        public Vector2f Position { get; private set; }

        public void SetPosition(Vector2f position)
        {
            if (position.X == Position.X && position.Y == Position.Y) return;

            Position = position;
            VisMap.SetCenter(Position);
        }

        public Color Color;

        public float Radius { get; private set; }

        public void SetRadius(float radius)
        {
            if (radius == Radius) return;

            Radius = radius;
            VisMap.SetRadius(Radius);
        }

        public float Power;

        public readonly VisbilityMap VisMap;

        private readonly Shader _shader;

        public PointLight() : this(Color.White, 1.0f, 32, new Vector2f())
        {
        }

        public PointLight(Color color, float power, float radius, Vector2f position)
        {
            Position = position;
            Color = color;
            Power = power;
            Radius = radius;

            VisMap = new VisbilityMap(Position, Radius);

            _shader = new Shader("shaders/lightShader.vert", "shaders/lightShader.frag");
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            VisMap.SetCenter(Position);
            VisMap.SetRadius(Radius);

            _shader.SetParameter("lightPosition", Position);
            _shader.SetParameter("lightColor", Color);
            _shader.SetParameter("lightRadius", Radius);

            var newStates = new RenderStates(states)
            {
                BlendMode = BlendMode.Add,
                Shader = _shader
            };

            target.Draw(VisMap.GetVisMesh(), newStates);

            if (State.ShowRaycastingLines) 
                VisMap.TraceIntersectionLines(target);

            if (State.ShowVisibleSegments) 
                VisMap.TraceVisibleSegments(target);
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
