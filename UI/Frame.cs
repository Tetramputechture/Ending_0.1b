using SFML.Graphics;

namespace Ending.UI
{
    public class Frame : Widget
    {
        public RectangleShape BorderRect { get; private set; }

        public FloatRect BoundingRect { get; private set; }

        public Frame(RectangleShape borderRect, Color fillColor)
        {
            BorderRect = borderRect;
            BorderRect.FillColor = fillColor;

            BoundingRect = borderRect.GetGlobalBounds();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (View != null)
            {
                target.SetView(View);
                target.Draw(BorderRect);
                target.SetView(target.DefaultView);
            }
            else
            {
                target.Draw(BorderRect);
            }
        }

        public void SetBorderRect(RectangleShape borderRect)
        {
            BorderRect = borderRect;
            BoundingRect = borderRect.GetGlobalBounds();
        }
    }
}
