using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
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

        private float _maxRayDistance;

        public void SetRadius(float radius)
        {
            if (radius == _radius) return;

            _radius = radius;
            _maxRayDistance = (float) Math.Sqrt(2*(radius*radius)) + 0.5f;
            _boundsNeedUpdate = true;
        }

        private readonly List<Segment> _segments;

        private readonly List<Segment> _bounds;

        private List<Segment> _visibleSegments; 

        private FloatRect _boundRect;

        private bool _boundsNeedUpdate;

        private readonly VertexArray _visMesh;

        private bool _visMeshNeedsUpdate;

        public VisbilityMap(Vector2f center, float radius)
        {
            _center = center;
            _radius = radius;
            _maxRayDistance = (float)Math.Sqrt(2 * (radius * radius)) + 0.5f;

            _segments = new List<Segment>();

            _bounds = new List<Segment>(new Segment[4]);

            _visibleSegments = new List<Segment>();

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

            // add all segments to a temp list
            _visibleSegments = new List<Segment>(_segments);

            // get all unique VISIBLE segments' endpoints + bound endpoints
            var points = new HashSet<Vector2f>();
            for (var i = _visibleSegments.Count - 1; i >= 0; i--)
            {
                var s = _visibleSegments[i];

                var edge = s.Start - s.End;
                var normal = new Vector2f(edge.Y, -edge.X);

                // is the segment visible from the ray origin?
                var distVector = s.End - _center;

                var dot = normal.Dot(distVector);

                if (dot > 0 && distVector.Length() <= _radius)
                {
                    points.Add(s.Start);
                    points.Add(s.End);
                }
                else
                {
                    _visibleSegments.RemoveAt(i);
                }
            }

            // add bounding points
            foreach (var s in _bounds)
            {
                points.Add(s.Start);
                points.Add(s.End);
            }

            // add bounding segments to visible segments
            _visibleSegments.AddRange(_bounds);

            // get all angles
            var angles = new List<float>();
            foreach (
                var angle in
                    points.Select(p => Math.Atan2(p.Y - _center.Y, p.X - _center.X)).Select(angle => (float) angle))
            {
                angles.Add(angle - 0.0001f);
                angles.Add(angle);
                angles.Add(angle + 0.0001f);
            }

            // rays to all visible endpoints
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
                        _visibleSegments
                            .Select(s => MathUtils.GetIntersection(rayStart, rayEnd, s.Start, s.End))
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
                TraceLine(target, _center.X, _center.Y, _visMesh[i].Position.X, _visMesh[i].Position.Y, Color.Yellow, true);
        }

        public void TraceVisibleSegments(RenderTarget target)
        {
            foreach (var s in _visibleSegments) 
                TraceLine(target, s.Start.X, s.Start.Y, s.End.X, s.End.Y, Color.Yellow, false);
        }

        public void TraceLine(RenderTarget rt, float x0, float y0, float x1, float y1, Color color, bool drawEndPoint)
        {
            var line = new[]
            {
                new Vertex(new Vector2f(x0, y0), color),
                new Vertex(new Vector2f(x1, y1), color)
            };

            rt.Draw(line, PrimitiveType.Lines);

            if (!drawEndPoint) return;

            var endPoint = new CircleShape(2)
            {
                Origin = new Vector2f(1, 1),
                Position = new Vector2f(x1, y1),
                FillColor = color
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
