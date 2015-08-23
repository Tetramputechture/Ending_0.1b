using System;
using System.IO;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public class Entity : Transformable
    {
        public readonly EntityType Type;

        public Vector2f Velocity;

        public FloatRect EntityBoundingBox;
        public FloatRect GeometryBoundingBox;

        public Entity(EntityType type)
        {
            Type = type;
        }

        public void Update(RenderTarget target, Map map)
        {
            Type.Input.Update(this);
            Type.Graphics.Update(this, target);
            Type.Physics.Update(this, map);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Type.Id);

            bw.Write(Position.X);
            bw.Write(Position.Y);
        }

        public static Entity Read(BinaryReader br)
        {
            var id = br.ReadInt16();

            EntityType type;

            switch (id)
            {
                case 1:
                    type = EntityType.Player;
                    break;
                default:
                    type = EntityType.Player;
                    break;
            }

            var pos = new Vector2f(br.ReadSingle(), br.ReadSingle());

            return new Entity(type) { Position = pos };
        }
    }
}
