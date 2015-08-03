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
        private const int SPEED = 70;

        public void Update(Entity entity)
        {
            Vector2f vel = entity.velocity;

            if (InputHandler.IsKeyPressed(Keyboard.Key.S))
            {
                entity.velocity = new Vector2f(vel.X, vel.Y + SPEED);
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.A))
            {
                entity.velocity = new Vector2f(vel.X - SPEED, vel.Y);
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.D))
            {
                entity.velocity = new Vector2f(vel.X + SPEED, vel.Y);
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.W))
            {
                entity.velocity = new Vector2f(vel.X, vel.Y - SPEED);
            }
        }
    }
}
