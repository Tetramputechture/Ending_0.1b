using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Window;
using Ending.UI;

namespace Ending.GameState
{
    abstract public class Screen : Drawable 
    {
        public List<Widget> widgets { get; } = new List<Widget>();

        public EventHandler<KeyEventArgs> KeyPressed;

        public EventHandler<MouseMoveEventArgs> MouseMoved;

        public EventHandler<MouseButtonEventArgs> MouseButtonPressed;

        protected Screen()
        {
            KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
            MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);
        }

        protected virtual void OnKeyPressed(object sender, KeyEventArgs e)
        {

        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (Widget w in widgets)
            {
                w.Update(e.X, e.Y, false);
            }
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (Widget w in widgets)
            {
                w.Update(e.X, e.Y, true);
            }
        }

        virtual public void Update()
        {

        }

        abstract public void Draw(RenderTarget rt, RenderStates states);
    }
}
