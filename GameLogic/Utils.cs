using System;
using Ending.GameLogic.DungeonTools;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public static class Utils
    {
        public static Direction RandDirection(Random rand)
        {
            var values = Enum.GetValues(typeof(Direction));
            return (Direction)values.GetValue(rand.Next(values.Length));
        }

        public static void SetOriginAtCenter(this Text text)
        {
            var bounds = text.GetLocalBounds();
            text.Origin = new Vector2f(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height / 2f);
        }

        public static void SetOriginAtCenter(this Sprite sprite)
        {
            var t = sprite.Texture;
            sprite.Origin = new Vector2f(t.Size.X/2f, t.Size.Y/2f);
        }
    }
}
