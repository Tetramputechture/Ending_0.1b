using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic
{
    public static class Utils
    {
        public static Direction RandDirection(Random rand)
        {
            Array values = Enum.GetValues(typeof(Direction));
            return (Direction)values.GetValue(rand.Next(values.Length));
        }

        public static void SetOriginAtCenter(this Text text)
        {
            FloatRect bounds = text.GetLocalBounds();
            text.Origin = new Vector2f(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height / 2f);
        }

        public static void SetOriginAtCenter(this Sprite sprite)
        {
            Texture t = sprite.Texture;
            sprite.Origin = new Vector2f(t.Size.X / 2, t.Size.Y / 2);
        }
    }
}
