using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Ending.SpriteTools
{
    public class Animation
    {
        public List<IntRect> Frames { get; }

        public List<IntRect> FrameBounds { get; }

        public Texture SpriteSheet;

        public Animation(Texture spriteSheet)
        {
            Frames = new List<IntRect>();
            FrameBounds = new List<IntRect>();
            SpriteSheet = spriteSheet;
        }

        public void AddFrame(IntRect rect)
        {
            Frames.Add(rect);
            FrameBounds.Add(CalculateBounds(rect));
        }

        private IntRect CalculateBounds(IntRect rect)
        {
            var width = Math.Abs(rect.Width) + rect.Left;
            var height = Math.Abs(rect.Height) + rect.Top;

            var minX = int.MaxValue;
            var maxX = 0;
            var minY = minX;
            var maxY = maxX;

            var img = SpriteSheet.CopyToImage();

            for (var x = rect.Left; x < width; x++)
            {
                for (var y = rect.Top; y < height; y++)
                {
                    if (img.GetPixel((uint) x, (uint) y).A < 123) continue;

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

            minY -= rect.Top;
            maxY -= rect.Top;

            return new IntRect(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

    }
}
