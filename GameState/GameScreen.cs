using Ending.GameLogic;
using Ending.GameWindow;
using Ending.UI;
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

        private bool fpsToggle;

        private Label msFLabel;

        public GameScreen()
        {
            game = new Game();

            view = new View();

            // ms / f text
            Text msFText = new Text
            {
                Font = new Font("fonts/Quicksand-Bold.ttf"),
                DisplayedString = " ms / frame: ",
                Color = Color.White,
                CharacterSize = 19,
            };
            msFText.SetOriginAtCenter();
            msFText.Position = new Vector2f(75, WindowConfig.WINDOW_HEIGHT - 25);

            msFLabel = new Label(msFText);

            widgets.Add(msFLabel);
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
                case Keyboard.Key.F:
                    fpsToggle = !fpsToggle;
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
                view.Size = new Vector2f(game.dungeon.size.X * 32, game.dungeon.size.Y * 32);
                game.dungeon.center = new Vector2f(game.dungeon.size.X * 16, game.dungeon.size.Y * 16);
            }

            msFLabel.text.DisplayedString = "ms / frame : " + Game.deltaTime.AsMilliseconds();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Clear(Color.Black);

            target.SetView(view);
            game.dungeon.Draw(target, states);
            if (fpsToggle)
            {
                msFLabel.Draw(target, states);
            }
        }
    }
}
