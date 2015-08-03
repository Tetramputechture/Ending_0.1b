using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.SpriteTools
{
    public class Animation
    {
        public List<IntRect> frames { get; }

        public List<IntRect> frameBounds { get; }

        public Texture spriteSheet;

        public Animation(Texture spriteSheet)
        {
            frames = new List<IntRect>();
            frameBounds = new List<IntRect>();
            this.spriteSheet = spriteSheet;
        }

        public void AddFrame(IntRect rect)
        {
            frames.Add(rect);
            frameBounds.Add(CalculateBounds(rect));
        }

        private IntRect CalculateBounds(IntRect rect)
        {
            var width = Math.Abs(rect.Width) + rect.Left;
            var height = Math.Abs(rect.Height) + rect.Top;

            var minX = int.MaxValue;
            var maxX = 0;
            var minY = minX;
            var maxY = maxX;

            Image img = spriteSheet.CopyToImage();

            for (var x = rect.Left; x < width; x++)
            {
                for (var y = rect.Top; y < height; y++)
                {
                    if (img.GetPixel((uint) x, (uint) y).A != 0) 
                    {
                        if (x < minX)
                        {
                            minX = x - rect.Left;
                        }
                        else if (x > maxX)
                        {
                            maxX = x - rect.Left;
                        }
                        if (y < minY)
                        {
                            minY = y;
                        }
                        else if (y > maxY)
                        {
                            maxY = y;
                        }
                    }
                }
            }

            minY -= rect.Top;
            maxY -= rect.Top;

            return new IntRect(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

    }
}
