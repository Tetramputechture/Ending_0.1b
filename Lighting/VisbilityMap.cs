using System;
using System.Collections.Generic;
using System.Linq;
using Ending.Utils;
using SFML.Graphics;
using SFML.System;

namespace Ending.Lighting
{
    public class VisbilityMap
    {
        private Vector2f _center;

        public void SetCenter(Vector2f center)
        {
            if (center.X == _center.X && center.Y == _center.Y) return;

            _center = center;
            _boundsNeedUpdate = true;
        }

        private float _radius;

        public void SetRadius(float radius)
        {
            if (radius == _radius) return;

            _radius = radius;
            _boundsNeedUpdate = true;
        }

        private readonly List<Segment> _segments;

        private readonly Segment[] _bounds;

        private FloatRect _boundRect;

        private bool _boundsNeedUpdate;

        private readonly VertexArray _visMesh;

        private bool _visMeshNeedsUpdate;

        public VisbilityMap(Vector2f center, float radius)
        {
            _center = center;
            _radius = radius;

            _segments = new List<Segment>();

            _bounds = new Segment[4];

            _boundRect = new FloatRect();

            _boundsNeedUpdate = true;

            ComputeBoundaries();

            _visMesh = new VertexArray(PrimitiveType.TrianglesFan);

            _visMeshNeedsUpdate = true;
        }

        public VertexArray GetVisMesh()
        {
            if (_boundsNeedUpdate)
            {
                ComputeBoundaries();
                _boundsNeedUpdate = false;
            }

            // if segments state is clean, no need to calculate mesh again
            if (!_visMeshNeedsUpdate)
                return _visMesh;

            // get all unique endpoints
            var points = new HashSet<Vector2f>();
            foreach (var s in _bounds)
            {
                points.Add(s.Start);
                points.Add(s.End);
            }

            foreach (var s in _segments)
            {
                points.Add(s.Start);
                points.Add(s.End);
            }

            // get all angles
            var angles = new List<float>();
            foreach (
                var fAngle in
                    points.Select(p => Math.Atan2(p.Y - _center.Y, p.X - _center.X)).Select(angle => (float) angle))
            {
                angles.Add(fAngle - 0.0001f);
                angles.Add(fAngle);
                angles.Add(fAngle + 0.0001f);
            }

            // rays in all directions
            var intersections = new List<Ray>();
            foreach (var angle in angles)
            {
                var dx = Math.Cos(angle);
                var dy = Math.Sin(angle);

                var rayStart = _center;
                var rayEnd = new Vector2f(_center.X + (float) dx, _center.Y + (float) dy);

                // find closest intersection
                Ray closestIntersect = null;

                foreach (
                    var intersect in
                        _segments.Select(s => MathUtils.GetIntersection(rayStart, rayEnd, s.Start, s.End))
                            .Where(intersect => intersect != null)
                            .Where(intersect => closestIntersect == null || intersect.Length < closestIntersect.Length))
                    closestIntersect = intersect;

                foreach (
                    var intersect in
                        _bounds.Select(s => MathUtils.GetIntersection(rayStart, rayEnd, s.Start, s.End))
                            .Where(intersect => intersect != null)
                            .Where(intersect => closestIntersect == null || intersect.Length < closestIntersect.Length))
                    closestIntersect = intersect;

                if (closestIntersect == null) continue;

                closestIntersect.Angle = angle;

                intersections.Add(closestIntersect);
            }

            // sort intersects by angle
            intersections.Sort((a, b) => a.Angle.CompareTo(b.Angle));

            // make visibility mesh
            _visMesh.Clear();

            _visMesh.Append(new Vertex(_center, _center));

            foreach (var hit in intersections)
                _visMesh.Append(new Vertex(hit.Position, hit.Position));

            _visMesh.Append(new Vertex(intersections[0].Position, intersections[0].Position));

            _visMeshNeedsUpdate = false;

            Console.WriteLine("mesh updated with " + intersections.Count + " hits");

            return _visMesh;
        }

        public void TraceIntersectionLines(RenderTarget target)
        {
            for (uint i = 0; i < _visMesh.VertexCount; i++)
                TraceLine(target, _center.X, _center.Y, _visMesh[i].Position.X, _visMesh[i].Position.Y);
        }

        public void TraceLine(RenderTarget rt, float x0, float y0, float x1, float y1)
        {
            var line = new[]
            {
                new Vertex(new Vector2f(x0, y0), Color.Red),
                new Vertex(new Vector2f(x1, y1), Color.Red)
            };

            rt.Draw(line, PrimitiveType.Lines);

            var endPoint = new CircleShape(2)
            {
                Origin = new Vector2f(1, 1),
                Position = new Vector2f(x1, y1),
                FillColor = Color.Red
            };

            rt.Draw(endPoint);
        }

        private void AddSegment(Vector2f start, Vector2f end)
        {
            _segments.Add(new Segment(start, end));
            _visMeshNeedsUpdate = true;
        }

        public void AddRectangleOccluder(FloatRect rect)
        {
            var topLeft = new Vector2f(rect.Left, rect.Top);
            var topRight = new Vector2f(rect.Left + rect.Width, rect.Top);
            var bottomRight = new Vector2f(rect.Left + rect.Width, rect.Top + rect.Height);
            var bottomLeft = new Vector2f(rect.Left, rect.Top + rect.Height);

            // top
            AddSegment(topLeft, topRight);

            // right
            AddSegment(topRight, bottomRight);

            // bottom
            AddSegment(bottomRight, bottomLeft);

            // left
            AddSegment(bottomLeft, topLeft);
        }

        private void ComputeBoundaries()
        {
            var topLeft = new Vector2f(_center.X - _radius, _center.Y - _radius);
            var topRight = new Vector2f(_center.X + _radius, _center.Y - _radius);
            var bottomRight = new Vector2f(_center.X + _radius, _center.Y + _radius);
            var bottomLeft = new Vector2f(_center.X - _radius, _center.Y + _radius);

            // top
            _bounds[0] = new Segment(topLeft, topRight);

            // right
            _bounds[1] = new Segment(topRight, bottomRight);

            // bottom
            _bounds[2] = new Segment(bottomRight, bottomLeft);

            // left
            _bounds[3] = new Segment(bottomLeft, topLeft);

            _boundRect = new FloatRect(topLeft.X, topLeft.Y, Math.Abs(topRight.X - topLeft.X), Math.Abs(bottomRight.Y - topRight.Y));

            _visMeshNeedsUpdate = true;
        }
    }
}
