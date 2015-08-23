using SFML.Graphics;

namespace Ending.GameLogic.DungeonTools
{
    public class TileType { 
    
        public static readonly TileType Stonefloor = new TileType("sprites/tiles/stonefloor.png");

        public static readonly TileType Stoneroof = new TileType("sprites/tiles/stonewall.png", false);

        public static readonly TileType StonewallNorth = new TileType("sprites/tiles/stonewall.png");

        public static readonly TileType StonewallEast = new TileType("sprites/tiles/stonewalleast.png");

        public static readonly TileType StonewallWest = new TileType("sprites/tiles/stonewallwest.png");

        public static readonly TileType StonewallSouth = new TileType("sprites/tiles/stonewallsouth.png");

        public Texture Texture { get; }

        public readonly string TextureName;

        public bool LightingEnabled { get; }

        TileType(string textureName, bool lightingEnabled = true)
        {
            if (textureName != null) Texture = new Texture(textureName);

            TextureName = textureName;

            LightingEnabled = lightingEnabled;
        }
    }
}