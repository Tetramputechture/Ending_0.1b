using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.Window;
using Ending.GameState;
using Ending.Input;
using Ending.GameLogic.DungeonTools;
using SFML.System;

namespace Ending.GameWindow
{
    public class Window
    {
        private RenderWindow rw;

        public Window(string title)
        {
            rw = new RenderWindow(new VideoMode(WindowConfig.WINDOW_WIDTH, WindowConfig.WINDOW_HEIGHT), title);
            State.currentScreen = new MainMenuScreen();
        }

        public void SetView(View view)
        {
            rw.SetView(view);
        }

        public void display()
        {
            while (rw.IsOpen)
            {
                InputHandler.HandleEvents(rw);

                Screen s = State.currentScreen;
                s.Update();

                rw.Clear();
                rw.Draw(s);

                rw.Display();
            }
        }
    }
}
