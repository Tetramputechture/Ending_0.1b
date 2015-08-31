using System;
using Ending.GameLogic.DungeonTools;
using Ending.Lighting;
using SFML.Graphics;
using SFML.System;

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

            Map.AmbientLightColor = new Vector3f(0.3f, 0.3f, 0.3f);
            _player.Position = new Vector2f(20 * 32, 15 * 32);
            Map.AddEntity(_player);

            _playerLight = new PointLight()
            {
                Color = Color.White,
                Power = 1.0f,
                Radius = 128,
                Position = _player.Position
            };

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
            _playerLight.Position = new Vector2f(_player.GeometryBoundingBox.Left + _player.GeometryBoundingBox.Width / 2f, _player.GeometryBoundingBox.Top + _player.GeometryBoundingBox.Height / 2f);
            DeltaTime = _frameClock.Restart();
        }
    }
}
