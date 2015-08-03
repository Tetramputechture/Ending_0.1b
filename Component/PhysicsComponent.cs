using Ending.GameLogic;
using Ending.GameLogic.DungeonTools;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.Component
{
    public interface PhysicsComponent
    {
        void Update(Entity entity, Dungeon dungeon, Time deltaTime);
    }
}
