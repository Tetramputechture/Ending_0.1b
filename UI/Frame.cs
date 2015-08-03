using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;

namespace Ending.UI
{
    public class Frame : Widget
    {
        public RectangleShape borderRect { get; private set; }

        public FloatRect boundingRect { get; private set; }

        public Frame(RectangleShape borderRect, Color fillColor)
        {
            this.borderRect = borderRect;
            this.borderRect.FillColor = fillColor;

            boundingRect = borderRect.GetGlobalBounds();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (view != null)
            {
                target.SetView(view);
                target.Draw(borderRect);
                target.SetView(target.DefaultView);
            }
            else
            {
                target.Draw(borderRect);
            }
        }

        public void SetBorderRect(RectangleShape borderRect)
        {
            this.borderRect = borderRect;
            boundingRect = borderRect.GetGlobalBounds();
        }
    }
}
