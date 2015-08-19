using SFML.Graphics;

namespace Ending.UI
{
    abstract public class Widget : Drawable
    {
        public View View;

        virtual public void Update(int mouseX, int mouseY, bool isLeftMouseButtonPressed)
        {

        }

        abstract public void Draw(RenderTarget target, RenderStates states);
    }
}
