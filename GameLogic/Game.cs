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

        private readonly DynamicLight _playerLight;

        public Game()
        {
            View = new View
            {
                Size = new Vector2f(320, 240),
                Center = new Vector2f(320, 240)
            };
            _player = Entity.CreatePlayer();
            _frameClock = new Clock();

            Map = new Map(40, 30);

            for (var x = 0; x < Map.Size.X; x++)
            {
                for (var y = 0; y < Map.Size.Y; y++)
                {
                    Map.AddTile(TileType.Stonefloor, x, y, 0);
                }
            }

            Map.AmbientLightColor = new Vector3f(0.1f, 0.1f, 0.1f);
            _player.Position = new Vector2f(20 * 32, 15 * 32);
            Map.AddEntity(_player);

            _playerLight = new DynamicLight
            {
                Position = _player.Position,
                Color = new Vector3f(0.85f, 0.85f, 0.85f),
                Radius = 128
            };
            Map.AddLight(_playerLight);

            Map.AddTile(TileType.StonewallNorth, 22, 17, 1);
            Map.AddTile(TileType.StonewallNorth, 22, 14, 1); 
            Map.AddTile(TileType.StonewallNorth, 18, 17, 1);
            Map.AddTile(TileType.StonewallNorth, 18, 14, 1);
        }

        public void Update()
        {
            Map.Center = _player.Position;
            _playerLight.Position = new Vector2f(_player.GeometryBoundingBox.Left + _player.GeometryBoundingBox.Width / 2f, _player.GeometryBoundingBox.Top + _player.GeometryBoundingBox.Height / 2f);
            DeltaTime = _frameClock.Restart();
        }
    }
}
