using Ending.Component;
using Ending.Input;
using static SFML.Window.Keyboard.Key;

namespace Ending.GameLogic.Player
{
    public class PlayerInputComponent : IInputComponent
    {
        private const int Speed = 90;

        public void Update(Entity entity)
        {
            if (InputHandler.IsKeyPressed(S))
            {
                entity.Velocity.Y += Speed;
            }
            if (InputHandler.IsKeyPressed(A))
            {
                entity.Velocity.X -= Speed;
            }
            if (InputHandler.IsKeyPressed(D))
            {
                entity.Velocity.X += Speed;
            }
            if (InputHandler.IsKeyPressed(W))
            {
                entity.Velocity.Y -= Speed;
            }
        }
    }
}
