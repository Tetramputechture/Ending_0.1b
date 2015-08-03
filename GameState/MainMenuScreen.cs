using Ending.GameLogic;
using Ending.GameState;
using Ending.UI;
using Ending.GameWindow;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameState
{
    public class MainMenuScreen : Screen
    {
        private Sprite backgroundSprite;

        public MainMenuScreen()
        {
            backgroundSprite = new Sprite(new Texture("sprites/mainmenu.png"));

            // enter game button
            Text enterGameText = new Text
            {
                Font = new Font("fonts/Quicksand-Bold.ttf"),
                DisplayedString = "enter game",
                Color = new Color(73, 73, 73)
            };
            enterGameText.SetOriginAtCenter();
            enterGameText.Position = new Vector2f(120, WindowConfig.WINDOW_HEIGHT - 200);

            Button enterGameButton = new Button(enterGameText)
            {
                hoverSoundFilename = "sounds/buttonhover.wav",
                defaultTextColor = new Color(73, 73, 73),
                textHighlightColor = new Color(242, 242, 242)
            };
            enterGameButton.clickAction = () =>
            {
                State.currentScreen = new GameScreen();
            };

            widgets.Add(enterGameButton);

            // options button
            Text optionsText = new Text
            {
                Font = new Font("fonts/Quicksand-Bold.ttf"),
                DisplayedString = "options",
                Color = new Color(73, 73, 73)
            };
            optionsText.SetOriginAtCenter();
            optionsText.Position = new Vector2f(140, WindowConfig.WINDOW_HEIGHT - 140);

            Button optionsButton = new Button(optionsText)
            {
                hoverSoundFilename = "sounds/buttonhover.wav",
                defaultTextColor = new Color(73, 73, 73),
                textHighlightColor = new Color(242, 242, 242)
            };

            widgets.Add(optionsButton);

            // exit game button
            Text exitGameText = new Text
            {
                Font = new Font("fonts/Quicksand-Bold.ttf"),
                DisplayedString = "exit game",
                Color = new Color(73, 73, 73)
            };
            exitGameText.SetOriginAtCenter();
            exitGameText.Position = new Vector2f(210, WindowConfig.WINDOW_HEIGHT - 80);

            Button exitGameButton = new Button(exitGameText)
            {
                hoverSoundFilename = "sounds/buttonhover.wav",
                defaultTextColor = new Color(73, 73, 73),
                textHighlightColor = new Color(242, 242, 242)
            };

            exitGameButton.clickAction = () =>
            {
                State.currentScreen = new GameScreen();
            };

            widgets.Add(exitGameButton);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Clear(Color.Black);
            target.Draw(backgroundSprite);
            foreach (Widget w in widgets) {
                target.Draw(w);
            }
        }
    }
}
