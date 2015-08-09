using Ending.GameLogic.DungeonTools;
using Ending.Lighting;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic
{
    public class Game
    {
        public Dungeon dungeon { get; private set; }

        private DungeonGenerator generator;

        public View view { get; }

        private Entity player;

        private DynamicLight playerLight;

        private static Clock frameClock;

        public static Time deltaTime { get; private set; }

        public Game()
        {
            generator = new DungeonGenerator();
            dungeon = generator.Generate(new StoneDungeonStyle(), 40, 30);
            view = new View();
            view.Size = new Vector2f(320, 240);
            player = Entity.CreatePlayer();
            initPlayer();
            frameClock = new Clock();
        }

        private void initPlayer()
        {
            dungeon.AddEntity(20, 15, player);
            dungeon.lightMap.ambient = new Vector3f(0.25f, 0.25f, 0.25f);
            playerLight = dungeon.lightMap.RequestLight();
            playerLight.radius = 128;
            playerLight.color = new Vector3f(0.75f, 0.75f, 0.75f);
            playerLight.position = new Vector3f(player.Position.X, player.Position.Y, 0);
        }

        public void GenerateNewDungeon()
        {
            dungeon = generator.Generate(new StoneDungeonStyle(), 40, 30);
            initPlayer();
        }

        public void Update()
        {
            view.Center = player.Position;
            dungeon.center = view.Center;
            playerLight.position = new Vector3f(player.Position.X, player.Position.Y, 0);
            deltaTime = frameClock.Restart();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            dungeon.Draw(target, states);
        }
    }
}
