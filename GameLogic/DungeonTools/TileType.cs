using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.DungeonTools
{
    public class TileType { 
    
        public static readonly TileType STONEFLOOR = new TileType("sprites/tiles/stonefloor.png", true, true);

        public static readonly TileType STONEROOF = new TileType("sprites/tiles/stonewall.png", false, false);

        public static readonly TileType STONEWALL_NORTH = new TileType("sprites/tiles/stonewallnorth.png", false, true);

        public static readonly TileType STONEWALL_EAST = new TileType("sprites/tiles/stonewalleast.png", false, true);

        public static readonly TileType STONEWALL_WEST = new TileType("sprites/tiles/stonewallwest.png", false, true);

        public static readonly TileType STONEWALL_SOUTH = new TileType("sprites/tiles/stonewallsouth.png", true, true);

        public static readonly TileType NULL = new TileType();

        public Texture texture { get; }

        public bool passable { get; }

        public bool lightingEnabled { get; }

        private TileType()
        {
        }

        TileType(string textureName, bool passable, bool lightingEnabled)
        {
            texture = new Texture(textureName);
            this.passable = passable;
            this.lightingEnabled = lightingEnabled;
        }
    }
}