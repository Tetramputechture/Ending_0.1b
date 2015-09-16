using Ending.GameLogic.DungeonTools;
using SFML.Graphics;

namespace Ending.GameLogic
{
    public class TileSprite : Sprite
    {
        public bool LightingEnabled;

        private static readonly Shader TileShader = new Shader("shaders/ambientShader.vert", "shaders/ambientShader.frag");

        public readonly string TextureName;

        public TileSprite(TileType type) : this (type, type.LightingEnabled)
        {
        }

        public TileSprite(TileType type, bool lightingEnabled) : this (type.TextureName, lightingEnabled)
        {
        }

        public TileSprite(string textureName, bool lightingEnabled)
        {
            Texture = new Texture(textureName);
            TextureName = textureName;
            LightingEnabled = lightingEnabled;
        }
    }
}
