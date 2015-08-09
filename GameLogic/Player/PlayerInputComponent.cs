using Ending.Component;
using Ending.Input;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.Player
{
    public class PlayerInputComponent : InputComponent
    {
        private const int SPEED = 90;

        public void Update(Entity entity)
        {
            if (InputHandler.IsKeyPressed(Keyboard.Key.S))
            {
                entity.velocity.Y += SPEED;
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.A))
            {
                entity.velocity.X -= SPEED;
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.D))
            {
                entity.velocity.X += SPEED;
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.W))
            {
                entity.velocity.Y -= SPEED;
            }
        }
    }
}
