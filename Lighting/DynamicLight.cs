using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.Lighting
{
    public class DynamicLight
    {
        public Vector3f position;

        public Vector3f color;

        public float radius;

        public DynamicLight()
        {
            position = new Vector3f();
            color = new Vector3f();
            radius = 0;
        }
    }
}
