using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.UI
{
    public class Label : Widget
    {
        public Text text { get; private set; }

        public Label(Text text)
        {
            this.text = text;
        }

        public override void Draw(RenderTarget rt, RenderStates states)
        {
            if (view != null)
            {
                rt.SetView(view);
                rt.Draw(text);
                rt.SetView(rt.DefaultView);
            }
            else
            {
                rt.SetView(rt.DefaultView);
                rt.Draw(text);
            }
        }
    }
}
