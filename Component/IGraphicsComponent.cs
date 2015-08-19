using Ending.GameLogic;
using SFML.Graphics;

namespace Ending.Component
{
    public interface IGraphicsComponent
    {
        void Update(Entity entity, RenderTarget target);
    }
}