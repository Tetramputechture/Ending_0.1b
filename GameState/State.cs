using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameState
{
    public static class State 
    {
        public static Screen currentScreen { get; set; }

        public static bool showEntityBoundingBoxes { get; set; }
    }
}
