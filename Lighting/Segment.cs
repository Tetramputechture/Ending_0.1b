using SFML.System;

namespace Ending.Lighting
{
    public struct Segment
    {
        public Vector2f Start;
        public Vector2f End;

        public Segment(Vector2f start, Vector2f end)
        {
            Start = start;
            End = end;
        }
    }
}
