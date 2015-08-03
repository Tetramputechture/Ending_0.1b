using Ending.GameLogic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameState
{
    public class GameScreen : Screen
    {
        private Game game;

        private View view;

        private bool viewToggle;

        public GameScreen()
        {
            game = new Game();

            view = new View();
        }

        protected override void OnKeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Space:
                    game.GenerateNewDungeon();
                    break;
                case Keyboard.Key.Z:
                    viewToggle = !viewToggle;
                    break;
                case Keyboard.Key.B:
                    State.showEntityBoundingBoxes = !State.showEntityBoundingBoxes;
                    break;
            }
        }

        public override void Update()
        {
            game.Update();

            if (viewToggle)
            {
                view = game.view;
            } 
            else
            {
                view = new View();
                view.Size = new Vector2f(1280, 960);
                view.Center = new Vector2f(640, 480);
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Clear(Color.Black);

            target.SetView(view);
            target.Draw(game);
        }
    }
}
