using SFML.System;
using SFML.Window;
using SFML.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ending.GameState;

namespace Ending.Input
{
    public class InputHandler
    {
        private static List<Keyboard.Key> keysPressed = new List<Keyboard.Key>();

        private static List<Mouse.Button> mouseButtonsPressed = new List<Mouse.Button>();

        public static Vector2i mousePosition { get; private set; }

        private static bool eventsInitialized = false;

        private static void InitEvents(RenderWindow rw)
        {
            rw.Closed += (s, e) =>
            {
                ((RenderWindow)s).Close();
            };

            rw.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);

            rw.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyReleased);

            rw.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);

            rw.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);

            eventsInitialized = true;
        }

        public static void HandleEvents(RenderWindow rw)
        {
            if (!eventsInitialized)
            {
                InitEvents(rw);
            }

            rw.DispatchEvents();
        }

        private static void OnKeyPressed(object sender, KeyEventArgs e)     
        {
            if (!keysPressed.Contains(e.Code))
            {
                keysPressed.Add(e.Code);
            }

            State.currentScreen.KeyPressed(sender, e);
        }

        private static void OnKeyReleased(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.Code);
        }

        public static bool IsKeyPressed(Keyboard.Key key)
        {
            return keysPressed.Contains(key);
        }

        private static void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            State.currentScreen.MouseMoved(sender, e);
        }

        private static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            State.currentScreen.MouseButtonPressed(sender, e);
        }
    }
}
