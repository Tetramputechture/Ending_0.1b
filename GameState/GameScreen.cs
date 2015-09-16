using System;
using Ending.GameLogic;
using Ending.GameLogic.DungeonTools;
using Ending.GameWindow;
using Ending.Input;
using Ending.UI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Ending.GameState
{
    public class GameScreen : Screen
    {
        private readonly Game _game;

        private View _view;

        private bool _viewToggle;

        private bool _fpsToggle;

        private readonly Label _msFLabel;

        public GameScreen()
        {
            _game = new Game();

            _view = new View();

            // ms / f text
            var msFText = new Text
            {
                Font = new Font("fonts/Quicksand-Bold.ttf"),
                DisplayedString = " ms / frame: ",
                Color = Color.White,
                CharacterSize = 19
            };
            msFText.SetOriginAtCenter();
            msFText.Position = new Vector2f(75, WindowConfig.WindowHeight - 25);

            _msFLabel = new Label(msFText);

            Widgets.Add(_msFLabel);
        }

        protected override void OnKeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Z:
                    _viewToggle = !_viewToggle;
                    break;
                case Keyboard.Key.B:
                    State.ShowEntityBoundingBoxes = !State.ShowEntityBoundingBoxes;
                    break;
                case Keyboard.Key.F:
                    _fpsToggle = !_fpsToggle;
                    break;
                case Keyboard.Key.R:
                    State.ShowRaycastingLines = !State.ShowRaycastingLines;
                    break;
                case Keyboard.Key.V:
                    State.ShowVisibleSegments = !State.ShowVisibleSegments;
                    break;
            }
        }


        protected override void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            var target = (RenderTarget) sender;
            var mousePos = target.MapPixelToCoords(new Vector2i(e.X, e.Y));
            _game.Map.AddTile(TileType.StonewallNorth, (int) mousePos.X / _game.Map.CellSize, (int)mousePos.Y / _game.Map.CellSize, 1);
        }

        public override void Update()
        {
            _game.Update();

            if (_viewToggle)
            {
                _view = _game.View;
            }
            else
            {
                _view = new View
                {
                    Size = new Vector2f(_game.Map.Size.X * _game.Map.CellSize, _game.Map.Size.Y * _game.Map.CellSize),
                    Center = new Vector2f(_game.Map.Size.X * (_game.Map.CellSize / 2), _game.Map.Size.Y * (_game.Map.CellSize / 2))
                };
            }

            _msFLabel.Text.DisplayedString = "ms / frame : " + Math.Round(Game.DeltaTime.AsMicroseconds() / 1000f, 2);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Clear(Color.Black);

            target.SetView(_view);

            _game._mouseLight.SetPosition(target.MapPixelToCoords(InputHandler.MousePosition));
            _game.Map.Draw(target, states);
            if (_fpsToggle)
                _msFLabel.Draw(target, states);
        }
    }
}
