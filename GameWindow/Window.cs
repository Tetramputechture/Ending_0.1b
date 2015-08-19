using Ending.GameState;
using Ending.Input;
using SFML.Graphics;
using SFML.Window;

namespace Ending.GameWindow
{
    public class Window
    {
        private readonly RenderWindow _rw;

        public Window(string title)
        {
            _rw = new RenderWindow(new VideoMode(WindowConfig.WindowWidth, WindowConfig.WindowHeight), title);
            State.Open = true;
            State.CurrentScreen = new MainMenuScreen();
        }

        public void SetView(View view) => _rw.SetView(view);

        public void Display()
        {
            while (State.Open && _rw.IsOpen)
            {
                InputHandler.HandleEvents(_rw);

                var s = State.CurrentScreen;
                s.Update();

                _rw.Clear();
                _rw.Draw(s);

                _rw.Display();
            }
        }
    }
}
