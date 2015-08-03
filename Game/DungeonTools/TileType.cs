using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.DungeonTools
{
    public class TileType
    {
        public static readonly TileType UNUSED = new TileType("sprites/tiles/unused.png", false);

        public static readonly TileType STONEFLOOR = new TileType("sprites/tiles/stonefloor.png", true);

        public static readonly TileType STONEVOID = new TileType("sprites/tiles/stonewall.png", false);

        public static readonly TileType STONEWALL_NORTH = new TileType("sprites/tiles/stonewallnorth.png", false);

        public static readonly TileType STONEWALL_EAST = new TileType("sprites/tiles/stonewalleast.png", false);

        public static readonly TileType STONEWALL_WEST = new TileType("sprites/tiles/stonewallwest.png", false);

        public static readonly TileType STONEWALL_SOUTH = new TileType("sprites/tiles/stonewallsouth.png", true);

        public static readonly TileType UPSTAIRS = new TileType("sprites/tiles/upstairs.png", false);

        public static readonly TileType DOWNSTAIRS = new TileType("sprites/tiles/downstairs.png", false);

        public Texture texture { get; }

        public bool passable { get; }

        TileType(string textureName, bool passable) {
            texture = new Texture(textureName);
            this.passable = passable;
        }
    }
}
