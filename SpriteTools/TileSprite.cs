using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ending.GameLogic.DungeonTools;
using SFML.Graphics;

namespace Ending.SpriteTools
{
    public class TileSprite : Sprite
    {
        public bool LightingEnabled;

        public TileType Type { get; }

        public TileSprite(TileType type)
        {
            Texture = type.Texture;
            Type = type;
            LightingEnabled = type.LightingEnabled;
        }
    }
}
