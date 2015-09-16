using Ending.GameLogic.DungeonTools;
using Ending.Input;
using Ending.Lighting;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Ending.GameLogic
{
    public class Game
    {
        public View View { get; }

        private readonly Entity _player;

        private static Clock _frameClock;

        public static Time DeltaTime { get; private set; }

        public Map Map { get; }

        private readonly PointLight _playerLight;

        public readonly PointLight _mouseLight;

        public Game()
        {
            View = new View
            {
                Size = new Vector2f(320, 240),
                Center = new Vector2f(320, 240)
            };
            _player = new Entity(EntityType.Player);
            _frameClock = new Clock();

            Map = new Map("testmap", 40, 30);

            for (var x = 0; x < Map.Size.X; x++)
            {
                for (var y = 0; y < Map.Size.Y; y++)
                {
                    Map.AddTile(TileType.Stonefloor, x, y, 0);
                }
            }

            Map.AmbientLightColor = new Color(70, 70, 70, 255);
            _player.Position = new Vector2f(20 * 32, 15 * 32);
            Map.AddEntity(_player);

            _playerLight = new PointLight(Color.White, 1.0f, 128, _player.Position);

            _mouseLight = new PointLight(Color.Red, 1.0f, 128, (Vector2f) InputHandler.MousePosition);

            Map.AddLight(_mouseLight);
            Map.AddLight(_playerLight);

            Map.AddTile(TileType.StonewallNorth, 22, 18, 1);
            Map.AddTile(TileType.StonewallNorth, 22, 14, 1);
            Map.AddTile(TileType.StonewallNorth, 18, 18, 1);
            Map.AddTile(TileType.StonewallNorth, 18, 14, 1);
            Map.AddTile(TileType.StonewallNorth, 20, 12, 1);
            Map.AddTile(TileType.StonewallNorth, 16, 16, 1);
            Map.AddTile(TileType.StonewallNorth, 20, 20, 1);
            Map.AddTile(TileType.StonewallNorth, 24, 16, 1);
        }

        public void Update()
        {
            Map.Center = _player.Position;
            _playerLight.SetPosition(_player.Position);
            View.Center = Map.Center;
            if (_mouseLight.Radius <= 512 && InputHandler.IsKeyPressed(Keyboard.Key.Up))
            {
                _mouseLight.SetRadius(_mouseLight.Radius + 2);
            }
            else if (_mouseLight.Radius >= 0 && InputHandler.IsKeyPressed(Keyboard.Key.Down))
            {
                _mouseLight.SetRadius(_mouseLight.Radius - 2);
            }
            DeltaTime = _frameClock.Restart();
        }
    }
}
