using Ending.Component;
using Ending.GameLogic.DungeonTools;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic
{
    public class EntityPhysicsComponent : PhysicsComponent
    {
        private const float SQRT2 = 1.4142135624f;

        public void Update(Entity entity, Dungeon dungeon)
        {
            Vector2f pos = entity.Position;

            FloatRect bounds = entity.geometryBoundingBox;

            // if entity is moving diagonally, divide vector by sqrt 2
            if (entity.velocity.X != 0 && entity.velocity.Y != 0)
            {
                entity.velocity /= SQRT2;
            }

            entity.Position += entity.velocity * Game.deltaTime.AsSeconds();

            // see if entity is colliding with an unpassable tile
            for (var x = 0; x < dungeon.size.X; x++)
            {
                for (var y = 0; y < dungeon.size.Y; y++)
                {
                    Tile t = dungeon.tileData[x, y];
                    if (!t.type.passable)
                    {
                        FloatRect area;
                        if (bounds.Intersects(t.globalBounds, out area))
                        {
                            // vertical collision
                            if (area.Width > area.Height && area.Height != bounds.Height)
                            {
                                // top
                                if (area.Contains(area.Left, bounds.Top))
                                {
                                    entity.Position = new Vector2f(pos.X, pos.Y + area.Height);
                                }
                                // down
                                else
                                {
                                    entity.Position = new Vector2f(pos.X, pos.Y - area.Height);
                                }
                            }

                            // horizontal collision
                            else if (area.Width < area.Height || area.Height == bounds.Height)
                            {
                                // right
                                if (area.Contains(bounds.Left + bounds.Width - 0.0001f, area.Top + 1))
                                {
                                    entity.Position = new Vector2f(pos.X - area.Width, pos.Y);
                                }
                                // left
                                else
                                {
                                    entity.Position = new Vector2f(pos.X + area.Width, pos.Y);
                                }
                            }
                        }
                    }
                }
            }

            // set velocity back to 0 for next frame
            entity.velocity = new Vector2f(0, 0);
        }

    }
}
