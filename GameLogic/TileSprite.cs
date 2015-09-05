using Ending.GameLogic.DungeonTools;
using SFML.Graphics;

namespace Ending.GameLogic
{
    public class TileSprite : Sprite
    {
        public bool LightingEnabled;

        private static readonly Shader TileShader = new Shader("shaders/ambientShader.vert", "shaders/ambientShader.frag");

        public readonly string TextureName;

        public Color AmbientColor;

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

        public new void Draw(RenderTarget target, RenderStates states)
        {
            TileShader.SetParameter("texture", Shader.CurrentTexture);
            TileShader.SetParameter("ambientLightColor", AmbientColor);

            var newStates = new RenderStates(states)
            {
                BlendMode = BlendMode.Add,
                Shader = TileShader
            };

            base.Draw(target, newStates);
        }
    }
}
