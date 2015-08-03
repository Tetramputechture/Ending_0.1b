using Ending.GameLogic;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.Component
{
    public interface GraphicsComponent
    {
        void Update(Entity entity, RenderTarget target, Time deltaTime);
    }
}