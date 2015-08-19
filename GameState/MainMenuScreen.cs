using Ending.GameLogic;
using Ending.GameWindow;
using Ending.UI;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameState
{
    public class MainMenuScreen : Screen
    {
        private readonly Sprite _backgroundSprite;

        public MainMenuScreen()
        {
            _backgroundSprite = new Sprite(new Texture("sprites/mainmenu.png"));

            // enter game button
            var enterGameText = new Text
            {
                Font = new Font("fonts/Quicksand-Bold.ttf"),
                DisplayedString = "enter game",
                Color = new Color(73, 73, 73)
            };
            enterGameText.SetOriginAtCenter();
            enterGameText.Position = new Vector2f(120, WindowConfig.WindowHeight - 200);

            var enterGameButton = new Button(enterGameText)
            {
                HoverSoundFilename = "sounds/buttonhover.wav",
                DefaultTextColor = new Color(73, 73, 73),
                TextHighlightColor = new Color(242, 242, 242),
                ClickAction = () => { State.CurrentScreen = new GameScreen(); }
            };

            Widgets.Add(enterGameButton);

            // options button
            var optionsText = new Text
            {
                Font = new Font("fonts/Quicksand-Bold.ttf"),
                DisplayedString = "options",
                Color = new Color(73, 73, 73)
            };
            optionsText.SetOriginAtCenter();
            optionsText.Position = new Vector2f(140, WindowConfig.WindowHeight - 140);

            var optionsButton = new Button(optionsText)
            {
                HoverSoundFilename = "sounds/buttonhover.wav",
                DefaultTextColor = new Color(73, 73, 73),
                TextHighlightColor = new Color(242, 242, 242)
            };

            Widgets.Add(optionsButton);

            // exit game button
            var exitGameText = new Text
            {
                Font = new Font("fonts/Quicksand-Bold.ttf"),
                DisplayedString = "exit game",
                Color = new Color(73, 73, 73)
            };
            exitGameText.SetOriginAtCenter();
            exitGameText.Position = new Vector2f(210, WindowConfig.WindowHeight - 80);

            var exitGameButton = new Button(exitGameText)
            {
                HoverSoundFilename = "sounds/buttonhover.wav",
                DefaultTextColor = new Color(73, 73, 73),
                TextHighlightColor = new Color(242, 242, 242),
                ClickAction = () => { State.CurrentScreen = new GameScreen(); }
            };


            Widgets.Add(exitGameButton);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Clear(Color.Black);
            target.Draw(_backgroundSprite);
            foreach (var w in Widgets)
            {
                target.Draw(w);
            }
        }
    }
}
