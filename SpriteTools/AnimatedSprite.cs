using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.SpriteTools
{
    class AnimatedSprite : Transformable, Drawable
    {
        public Animation animation { get; set; }
        
        public Time frameTime { get; set; }

        private Time currentTime;

        private int currentFrame;

        public bool pause { get; set; }

        public bool loop { get; set; }

        private Vertex[] vertices;

        public FloatRect localBounds
        {
            get
            {
                return (FloatRect)animation.frameBounds[currentFrame];
            }
        }

        public FloatRect globalBounds
        {
            get
            {
                return Transform.TransformRect(localBounds);
            }
        }

        public AnimatedSprite(Time frameTime, bool paused, bool looped)
        {
            this.frameTime = frameTime;
            pause = paused;
            loop = looped;
            vertices = new Vertex[4];
        }

        public void SetFrame(int newFrame, bool resetTime)
        {
            if (animation != null)
            {
                // calculate new vertex positions and texture coordinates 
                IntRect rect = animation.frames[newFrame];

                Vector2f texCoordA = new Vector2f(0, 0);
                Vector2f texCoordB = new Vector2f(0, rect.Height);
                Vector2f texCoordC = new Vector2f(rect.Width, rect.Height);
                Vector2f texCoordD = new Vector2f(rect.Width, 0);

                float left = rect.Left + 0.0001f;
                float right = left + rect.Width;
                float top = rect.Top;
                float bottom = top + rect.Height;

                vertices[0] = new Vertex(texCoordA, new Vector2f(left, top));
                vertices[1] = new Vertex(texCoordB, new Vector2f(left, bottom));
                vertices[2] = new Vertex(texCoordC, new Vector2f(right, bottom));
                vertices[3] = new Vertex(texCoordD, new Vector2f(right, top));
            }

            if (resetTime)
            {
                currentTime = Time.Zero;
            }
        }

        public void Update(Time deltaTime)
        {
            if (!pause && animation != null)
            {
                // add delta time
                currentTime += deltaTime;

                // if current time is bigger than frame time, advance one frame
                if (currentTime > frameTime)
                {
                    // reset time, but keep remainder
                    currentTime = currentTime % frameTime;

                    // get next frame index
                    if (currentFrame + 1 < animation.frames.Count)
                    {
                        currentFrame++;
                    }
                    else
                    {
                        // animation has ended
                        currentFrame = 0;

                        if (!loop)
                        {
                            pause = true;
                        }
                    }

                    SetFrame(currentFrame, false);
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (animation != null)
            {
                RenderStates newStates = new RenderStates(
                    states.BlendMode,
                    states.Transform * Transform,
                    animation.spriteSheet,
                    states.Shader);
                target.Draw(vertices, PrimitiveType.Quads, newStates);
            }
        }

        public void SetAnimation(Animation animation)
        {
            this.animation = animation;
            currentFrame = 0;
            SetFrame(currentFrame, true);
        }

        public void Stop()
        {
            pause = true;
            currentFrame = 0;
            SetFrame(currentFrame, true);
        }

    }
}
