using Ending.GameLogic.DungeonTools;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic
{
    public class Game : Drawable
    {
        private Dungeon dungeon;

        private DungeonGenerator generator;

        public View view { get; }

        private Entity player;

        private Clock frameClock;

        private Time deltaTime;

        public Game()
        {
            generator = new DungeonGenerator();
            dungeon = generator.Generate(new StoneDungeonStyle(), 40, 30);
            view = new View();
            view.Size = new Vector2f(400, 300);
            player = Entity.CreatePlayer();
            dungeon.AddEntity(20, 15, player);
            frameClock = new Clock();
        }

        public void GenerateNewDungeon()
        {
            dungeon = generator.Generate(new StoneDungeonStyle(), 40, 30);
            dungeon.AddEntity(20, 15, player);
        }

        public void Update()
        {
            view.Center = player.Position;
            deltaTime = frameClock.Restart();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            dungeon.Update(target, states, deltaTime);
        }
    }
}
