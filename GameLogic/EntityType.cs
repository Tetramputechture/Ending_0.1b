using Ending.Component;
using Ending.GameLogic.Player;
using SFML.Graphics;

namespace Ending.GameLogic
{
    public class EntityType
    {
        public static readonly EntityType Player = new EntityType(
            1,
            new PlayerInputComponent(),
            new EntityGraphicsComponent(new Texture("sprites/player_shadowed.png")),
            new EntityPhysicsComponent());

        public readonly short Id;

        public readonly IInputComponent Input;
        public readonly IGraphicsComponent Graphics;
        public readonly IPhysicsComponent Physics;

        private EntityType(short id,
                    IInputComponent input,
                    IGraphicsComponent graphics,
                    IPhysicsComponent physics)
        {
            Id = id;
            Input = input;
            Graphics = graphics;
            Physics = physics;
        }

    }
}
