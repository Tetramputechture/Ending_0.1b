using Ending.GameLogic.DungeonTools;
using SFML.Graphics;

namespace Ending.SpriteTools
{
    public class TileSprite : Sprite
    {
        public bool LightingEnabled;

        public readonly string TextureName;

        public TileSprite(TileType type)
        {
            Texture = type.Texture;
            TextureName = type.TextureName;
            LightingEnabled = type.LightingEnabled;
        }

        public TileSprite(TileType type, bool lightingEnabled)
        {
            Texture = type.Texture;
            TextureName = type.TextureName;
            LightingEnabled = lightingEnabled;
        }

        public TileSprite(string textureName, bool lightingEnabled)
        {
            Texture = new Texture(textureName);
            TextureName = textureName;
            LightingEnabled = lightingEnabled;
        }
    }
}
