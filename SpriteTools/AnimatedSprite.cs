using SFML.Graphics;
using SFML.System;

namespace Ending.SpriteTools
{
    class AnimatedSprite : Transformable, Drawable
    {
        public Animation Animation;

        public Time FrameTime;

        private Time _currentTime;

        private int _currentFrame;

        public bool Pause;

        public bool Loop;

        private readonly Vertex[] _vertices;

        public FloatRect LocalBounds => (FloatRect)Animation.FrameBounds[_currentFrame];

        public FloatRect GlobalBounds => Transform.TransformRect(LocalBounds);

        public AnimatedSprite(Time frameTime, bool paused, bool looped)
        {
            FrameTime = frameTime;
            Pause = paused;
            Loop = looped;
            _vertices = new Vertex[4];
        }

        public void SetFrame(int newFrame, bool resetTime)
        {
            if (Animation != null)
            {
                // calculate new vertex positions and texture coordinates 
                var rect = Animation.Frames[newFrame];

                var texCoordA = new Vector2f(0, 0);
                var texCoordB = new Vector2f(0, rect.Height);
                var texCoordC = new Vector2f(rect.Width, rect.Height);
                var texCoordD = new Vector2f(rect.Width, 0);

                var left = rect.Left + 0.0001f;
                var right = left + rect.Width;
                float top = rect.Top;
                var bottom = top + rect.Height;

                _vertices[0] = new Vertex(texCoordA, new Vector2f(left, top));
                _vertices[1] = new Vertex(texCoordB, new Vector2f(left, bottom));
                _vertices[2] = new Vertex(texCoordC, new Vector2f(right, bottom));
                _vertices[3] = new Vertex(texCoordD, new Vector2f(right, top));
            }

            if (resetTime)
            {
                _currentTime = Time.Zero;
            }
        }

        public void Update(Time deltaTime)
        {
            if (Pause || Animation == null) return;

            // add delta time
            _currentTime += deltaTime;

            // if current time is bigger than frame time, advance one frame
            if (_currentTime <= FrameTime) return;

            // reset time, but keep remainder
            _currentTime = _currentTime % FrameTime;

            // get next frame index
            if (_currentFrame + 1 < Animation.Frames.Count)
            {
                _currentFrame++;
            }
            else
            {
                // animation has ended
                _currentFrame = 0;

                if (!Loop)
                {
                    Pause = true;
                }
            }

            SetFrame(_currentFrame, false);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (Animation == null) return;

            var newStates = new RenderStates(
                states.BlendMode,
                states.Transform * Transform,
                Animation.SpriteSheet,
                states.Shader);
            target.Draw(_vertices, PrimitiveType.Quads, newStates);
        }

        public void SetAnimation(Animation animation)
        {
            Animation = animation;
            _currentFrame = 0;
            SetFrame(_currentFrame, true);
        }

        public void Stop()
        {
            Pause = true;
            _currentFrame = 0;
            SetFrame(_currentFrame, true);
        }

    }
}
