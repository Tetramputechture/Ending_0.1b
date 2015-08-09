using Ending.Component;
using Ending.GameLogic.DungeonTools;
using Ending.GameLogic.Player;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic
{
    public class Entity : Transformable
    {
        private InputComponent input;
        private GraphicsComponent graphics;
        private PhysicsComponent physics;

        public Vector2f velocity;

        public FloatRect entityBoundingBox;
        public FloatRect geometryBoundingBox;

        public Entity(InputComponent input,
            GraphicsComponent graphics,
            PhysicsComponent physics)
        {
            this.input = input;
            this.graphics = graphics;
            this.physics = physics;
        }

        public void Update(RenderTarget target, Dungeon dungeon)
        {
            input.Update(this);
            graphics.Update(this, target);
            physics.Update(this, dungeon);
        }

        public static Entity CreatePlayer()
        {
            return new Entity(new PlayerInputComponent(),
                new EntityGraphicsComponent(new Texture("sprites/player_shadowed.png")),
                new EntityPhysicsComponent());
        }

    }
}
