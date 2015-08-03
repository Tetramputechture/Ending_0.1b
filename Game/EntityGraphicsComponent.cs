using Ending.Audio;
using Ending.Component;
using Ending.GameState;
using Ending.SpriteTools;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic
{
    public class EntityGraphicsComponent : GraphicsComponent
    {
        private AnimatedSprite sprite;

        private Animation south;
        private Animation west;
        private Animation north;
        private Animation east;
        private Animation northwest;
        private Animation northeast;
        private Animation southwest;
        private Animation southeast;

        private Animation currentAnimation;

        private const int SPRITE_WIDTH = 32;
        private const int SPRITE_HEIGHT = 32;

        private const int NUM_FRAMES = 5;

        private RectangleShape entityBoundingBoxOutline;
        private RectangleShape geometryBoundingBoxOutline;

        private Music footstepMusic;

        public EntityGraphicsComponent(Texture spriteSheet)
        {
            entityBoundingBoxOutline = new RectangleShape
            {
                FillColor = Color.Transparent,
                OutlineColor = Color.Red,
                OutlineThickness = 1
            };

            geometryBoundingBoxOutline = new RectangleShape
            {
                FillColor = Color.Transparent,
                OutlineColor = Color.White,
                OutlineThickness = 1
            };

            sprite = new AnimatedSprite(Time.FromSeconds(0.1f), true, false);

            south = new Animation(spriteSheet);
            addFramesToAnimation(south, 0);

            north = new Animation(spriteSheet);
            addFramesToAnimation(north, 32);

            east = new Animation(spriteSheet);
            addFramesToAnimation(east, 64);

            west = new Animation(spriteSheet);
            addFramesToAnimation(west, 96);

            northwest = new Animation(spriteSheet);
            addFramesToAnimation(northwest, 128);

            northeast = new Animation(spriteSheet);
            addFramesToAnimation(northeast, 160);

            southwest = new Animation(spriteSheet);
            addFramesToAnimation(southwest, 192);

            southeast = new Animation(spriteSheet);
            addFramesToAnimation(southeast, 224);

            currentAnimation = south;

            sprite.animation = currentAnimation;

            footstepMusic = new Music("sounds/footstep.wav");
            footstepMusic.Loop = true;
        }

        private void addFramesToAnimation(Animation animation, int height)
        {
            for (int i = 0; i < NUM_FRAMES; i++)
            {
                animation.AddFrame(new IntRect(i * SPRITE_WIDTH, height, SPRITE_WIDTH, SPRITE_HEIGHT));
            }
        }

        public void Update(Entity entity, RenderTarget target, Time deltaTime)
        {
            Vector2f velocity = entity.velocity;

            // south
            if (velocity.X == 0 && velocity.Y > 0)
            {
                currentAnimation = south;
            }
            // east
            if (velocity.X > 0 && velocity.Y == 0)
            {
                currentAnimation = east;
            }
            // north
            if (velocity.X == 0 && velocity.Y < 0)
            {
                currentAnimation = north;
            }
            // west
            if (velocity.X < 0 && velocity.Y == 0)
            {
                currentAnimation = west;
            }
            // northwest
            if (velocity.X < 0 && velocity.Y < 0)
            {
                currentAnimation = northwest;
            }
            // northeast
            if (velocity.X > 0 && velocity.Y < 0)
            {
                currentAnimation = northeast;
            }
            // southwest
            if (velocity.X < 0 && velocity.Y > 0)
            {
                currentAnimation = southwest;
            }
            // southeast
            if (velocity.X > 0 && velocity.Y > 0)
            {
                currentAnimation = southeast;
            }

            sprite.animation = currentAnimation;
            sprite.pause = false;

            // if not moving, stop animation
            if (velocity.X == 0 && velocity.Y == 0)
            {
                sprite.Stop();
                footstepMusic.Stop();
            }
            else
            {
                if (footstepMusic.Status == SoundStatus.Stopped)
                {
                    footstepMusic.Play();
                }
            }

            sprite.Update(deltaTime);

            sprite.Position = entity.Position;

            FloatRect spriteBounds = sprite.globalBounds;
            FloatRect geoBounds = new FloatRect(spriteBounds.Left + 3, spriteBounds.Top + spriteBounds.Height - 3, spriteBounds.Width - 6, 3);

            entity.entityBoundingBox = spriteBounds;
            entity.geometryBoundingBox = geoBounds;

            entityBoundingBoxOutline.Position = new Vector2f(spriteBounds.Left, spriteBounds.Top);
            entityBoundingBoxOutline.Size = new Vector2f(spriteBounds.Width, spriteBounds.Height);

            geometryBoundingBoxOutline.Position = new Vector2f(geoBounds.Left, geoBounds.Top);
            geometryBoundingBoxOutline.Size = new Vector2f(geoBounds.Width, geoBounds.Height);

            target.Draw(sprite);

            if (State.showEntityBoundingBoxes)
            {
                target.Draw(entityBoundingBoxOutline);
                target.Draw(geometryBoundingBoxOutline);
            }
        }
    }
}
