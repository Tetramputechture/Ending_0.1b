using SFML.Graphics;

namespace Ending.GameLogic.DungeonTools
{
    public class TileType { 
    
        public static readonly TileType Stonefloor = new TileType("sprites/tiles/stonefloor.png", true);

        public static readonly TileType Stoneroof = new TileType("sprites/tiles/stonewall.png", false, false);

        public static readonly TileType StonewallNorth = new TileType("sprites/tiles/stonewallnorth.png", false);

        public static readonly TileType StonewallEast = new TileType("sprites/tiles/stonewalleast.png", false);

        public static readonly TileType StonewallWest = new TileType("sprites/tiles/stonewallwest.png", false);

        public static readonly TileType StonewallSouth = new TileType("sprites/tiles/stonewallsouth.png", true);

        public static readonly TileType Null = new TileType(null, false, false);

        public Texture Texture { get; }

        public bool Passable { get; }

        public bool LightingEnabled { get; }

        TileType(string textureName, bool passable, bool lightingEnabled = true)
        {
            if (textureName != null) Texture = new Texture(textureName);

            Passable = passable;
            LightingEnabled = lightingEnabled;
        }
    }
}