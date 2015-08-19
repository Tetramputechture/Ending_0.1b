using SFML.Graphics;

namespace Ending.UI
{
    public class Label : Widget
    {
        public Text Text { get; }

        public Label(Text text)
        {
            Text = text;
        }

        public override void Draw(RenderTarget rt, RenderStates states)
        {
            if (View != null)
            {
                rt.SetView(View);
                rt.Draw(Text);
                rt.SetView(rt.DefaultView);
            }
            else
            {
                rt.SetView(rt.DefaultView);
                rt.Draw(Text);
            }
        }
    }
}
