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
            _player = new Entity(EntityType.Player);
            _frameClock = new Clock();

            var tempMap = new Map("testmap", 40, 30);

            for (var x = 0; x < tempMap.Size.X; x++)
            {
                for (var y = 0; y < tempMap.Size.Y; y++)
                {
                    tempMap.AddTile(TileType.Stonefloor, x, y, 0);
                }
            }

            tempMap.AmbientLightColor = new Vector3f(0.5f, 0.5f, 0.5f);
            _player.Position = new Vector2f(20 * 32, 15 * 32);
            tempMap.AddEntity(_player);

            _playerLight = new DynamicLight
            {
                Position = _player.Position,
                Color = new Vector3f(0.85f, 0.85f, 0.85f),
                Radius = 128
            };
            //tempMap.AddLight(_playerLight);

            tempMap.AddTile(TileType.StonewallNorth, 22, 18, 1);
            tempMap.AddTile(TileType.StonewallNorth, 22, 14, 1);
            tempMap.AddTile(TileType.StonewallNorth, 18, 18, 1);
            tempMap.AddTile(TileType.StonewallNorth, 18, 14, 1);
            tempMap.AddTile(TileType.StonewallNorth, 20, 12, 1);
            tempMap.AddTile(TileType.StonewallNorth, 16, 16, 1);
            tempMap.AddTile(TileType.StonewallNorth, 20, 20, 1);
            tempMap.AddTile(TileType.StonewallNorth, 24, 16, 1);

            tempMap.Save("test.map");

            Map = Map.Load("test.map");
            Map._entities[0] = _player;
            //Map._lights[0] = _playerLight;
        }

        public void Update()
        {
            Map.Center = _player.Position;
            _playerLight.Position = new Vector2f(_player.GeometryBoundingBox.Left + _player.GeometryBoundingBox.Width / 2f, _player.GeometryBoundingBox.Top + _player.GeometryBoundingBox.Height / 2f);
            DeltaTime = _frameClock.Restart();
        }
    }
}
