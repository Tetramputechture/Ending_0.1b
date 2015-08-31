using System;
using Ending.Component;
using Ending.Utils;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public class EntityPhysicsComponent : IPhysicsComponent
    {
        private FloatRect _tileBounds = new FloatRect(0, 0, 32, 32);

        public void Update(Entity entity, Map map)
        {
            var pos = entity.Position;

            var bounds = entity.GeometryBoundingBox;

            // if entity is moving diagonally, divide vector by sqrt 2
            if (entity.Velocity.X != 0 && entity.Velocity.Y != 0)
                entity.Velocity /= MathUtils.SqrtTwo;

            entity.Position += entity.Velocity * Game.DeltaTime.AsSeconds();

            // see if entity is colliding with an unpassable tile
            for (var x = 0; x < map.Size.X; x++)
            {
                for (var y = 0; y < map.Size.Y; y++)
                {
                    var t = map.Layer1Cells[x, y];

                    if (t.IsEmpty()) continue;

                    _tileBounds.Left = x * map.CellSize;
                    _tileBounds.Top = y * map.CellSize;

                    FloatRect area;
                    if (!bounds.Intersects(_tileBounds, out area)) continue;

                    // vertical collision
                    if (area.Width > area.Height && Math.Abs(area.Height - bounds.Height) > 0.001f)
                    {
                        // top
                        entity.Position = area.Contains(area.Left, bounds.Top) ? new Vector2f(pos.X, pos.Y + area.Height) : new Vector2f(pos.X, pos.Y - area.Height);
                    }

                    // horizontal collision
                    else if (area.Width < area.Height || Math.Abs(area.Height - bounds.Height) < 0.001f)
                    {
                        // right
                        entity.Position = area.Contains(bounds.Left + bounds.Width - 0.0001f, area.Top + 1) ? new Vector2f(pos.X - area.Width, pos.Y) : new Vector2f(pos.X + area.Width, pos.Y);
                    }
                }
            }

            // set velocity back to 0 for next frame
            entity.Velocity = new Vector2f(0, 0);
        }

    }
}
