using System.Collections.Generic;
using Ending.GameState;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Ending.Input
{
    public class InputHandler
    {
        private static readonly List<Keyboard.Key> KeysPressed = new List<Keyboard.Key>();

        private static readonly List<Mouse.Button> MouseButtonsPressed = new List<Mouse.Button>();

        public static Vector2i MousePosition;

        private static bool _eventsInitialized;

        private static void InitEvents(Window rw)
        {
            rw.Closed += (s, e) => ((RenderWindow)s).Close();

            rw.KeyPressed += OnKeyPressed;

            rw.KeyReleased += OnKeyReleased;

            rw.MouseMoved += OnMouseMoved;

            rw.MouseButtonPressed += OnMouseButtonPressed;

            _eventsInitialized = true;
        }

        public static void HandleEvents(RenderWindow rw)
        {
            if (!_eventsInitialized)
            {
                InitEvents(rw);
            }

            rw.DispatchEvents();
        }

        private static void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (!KeysPressed.Contains(e.Code))
            {
                KeysPressed.Add(e.Code);
            }

            State.CurrentScreen.KeyPressed(sender, e);
        }

        private static void OnKeyReleased(object sender, KeyEventArgs e) => KeysPressed.Remove(e.Code);

        public static bool IsKeyPressed(Keyboard.Key key) => KeysPressed.Contains(key);

        private static void OnMouseMoved(object sender, MouseMoveEventArgs e) => State.CurrentScreen.MouseMoved(sender, e);

        private static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e) => State.CurrentScreen.MouseButtonPressed(sender, e);
    }
}
