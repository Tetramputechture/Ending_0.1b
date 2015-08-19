using Ending.Component;
using Ending.GameLogic.Player;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public class Entity : Transformable
    {
        private readonly IInputComponent _input;
        private readonly IGraphicsComponent _graphics;
        private readonly IPhysicsComponent _physics;

        public Vector2f Velocity;

        public FloatRect EntityBoundingBox;
        public FloatRect GeometryBoundingBox;

        public Entity(IInputComponent input,
            IGraphicsComponent graphics,
            IPhysicsComponent physics)
        {
            _input = input;
            _graphics = graphics;
            _physics = physics;
        }

        public void Update(RenderTarget target, Map map)
        {
            _input.Update(this);
            _graphics.Update(this, target);
            _physics.Update(this, map);
        }

        public static Entity CreatePlayer() => new Entity(
            new PlayerInputComponent(),
            new EntityGraphicsComponent(new Texture("sprites/player_shadowed.png")),
            new EntityPhysicsComponent());
    }
}
