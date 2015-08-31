using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ending.Lighting;
using SFML.System;

namespace Ending.Utils
{
    public class MathUtils
    {
        public static float SqrtTwo = (float) Math.Sqrt(2);

        public static Ray GetIntersection(Vector2f rayStart, Vector2f rayEnd,
            Vector2f segmentStart, Vector2f segmentEnd)
        {
            // ray in parametric form: Point + Direction * T1
            var rPx = rayStart.X;
            var rPy = rayStart.Y;
            var rDx = rayEnd.X - rPx;
            var rDy = rayEnd.Y - rPy;

            // segment in parametric form: Point * Direction * T2
            var sPx = segmentStart.X;
            var sPy = segmentStart.Y;
            var sDx = segmentEnd.X - sPx;
            var sDy = segmentEnd.Y - sPy;

            // if lines are parallel, no intersection
            var rMag = Math.Sqrt(rDx * rDx + rDy * rDy);
            var sMag = Math.Sqrt(sDx * sDx + sDy * sDy);

            const float tolerance = 0.00001f;

            if (Math.Abs(rDx / rMag - sDx / sMag) < tolerance && Math.Abs(rDy / rMag - sDy / sMag) < tolerance)
                // directions are the same
                return null;

            // solve for T1 and T2
            var t2 = (rDx * (sPy - rPy) + rDy * (rPx - sPx)) / (sDx * rDy - sDy * rDx);
            var t1 = (sPx + sDx * t2 - rPx) / rDx;

            // must be within parametric bounds
            if (t1 < 0 || t2 < 0 || t2 > 1)
                return null;

            // return the point of intersection
            return new Ray()
            {
                Position = new Vector2f(rPx + rDx * t1, rPy + rDy * t1),
                Length = t1,
                Angle = 0
            };
        }
    }
}
