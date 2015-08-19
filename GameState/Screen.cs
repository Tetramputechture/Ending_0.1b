using System;
using System.Collections.Generic;
using Ending.UI;
using SFML.Graphics;
using SFML.Window;

namespace Ending.GameState
{
    abstract public class Screen : Drawable 
    {
        public List<Widget> Widgets { get; } = new List<Widget>();

        public EventHandler<KeyEventArgs> KeyPressed;

        public EventHandler<MouseMoveEventArgs> MouseMoved;

        public EventHandler<MouseButtonEventArgs> MouseButtonPressed;

        protected Screen()
        {
            KeyPressed += OnKeyPressed;
            MouseMoved += OnMouseMoved;
            MouseButtonPressed += OnMouseButtonPressed;
        }

        protected virtual void OnKeyPressed(object sender, KeyEventArgs e)
        {
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (var w in Widgets)
            {
                w.Update(e.X, e.Y, false);
            }
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (var w in Widgets)
            {
                w.Update(e.X, e.Y, true);
            }
        }

        public virtual void Update()
        {

        }

        public abstract void Draw(RenderTarget rt, RenderStates states);
    }
}
