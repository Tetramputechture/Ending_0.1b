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

        public Vector2f velocity { get; set; }

        public FloatRect entityBoundingBox { get; set; }
        public FloatRect geometryBoundingBox { get; set; }

        public Entity(InputComponent input,
            GraphicsComponent graphics,
            PhysicsComponent physics)
        {
            this.input = input;
            this.graphics = graphics;
            this.physics = physics;
        }

        public void Update(RenderTarget target, Dungeon dungeon, Time deltaTime)
        {
            input.Update(this);
            graphics.Update(this, target, deltaTime);
            physics.Update(this, dungeon, deltaTime);
        }

        public static Entity CreatePlayer()
        {
            return new Entity(new PlayerInputComponent(),
                new EntityGraphicsComponent(new Texture("sprites/player.png")),
                new EntityPhysicsComponent());
        }

    }
}
