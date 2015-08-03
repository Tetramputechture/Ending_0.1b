using SFML.Graphics;
using SFML.System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.UI
{
    abstract public class Widget : Drawable
    {
        public View view;

        virtual public void Update(int mouseX, int mouseY, bool isLeftMouseButtonPressed)
        {

        }

        abstract public void Draw(RenderTarget target, RenderStates states);
    }
}
