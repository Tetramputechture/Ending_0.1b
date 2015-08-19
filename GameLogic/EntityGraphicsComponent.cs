using Ending.Component;
using Ending.GameState;
using Ending.SpriteTools;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public class EntityGraphicsComponent : IGraphicsComponent
    {
        private readonly AnimatedSprite _sprite;

        private readonly Animation _south;
        private readonly Animation _west;
        private readonly Animation _north;
        private readonly Animation _east;
        private readonly Animation _northwest;
        private readonly Animation _northeast;
        private readonly Animation _southwest;
        private readonly Animation _southeast;

        private Animation _currentAnimation;

        private const int SpriteWidth = 32;
        private const int SpriteHeight = 32;

        private const int NumFrames = 5;

        private readonly RectangleShape _entityBoundingBoxOutline;
        private readonly RectangleShape _geometryBoundingBoxOutline;

        private readonly Music _footstepMusic;

        public EntityGraphicsComponent(Texture spriteSheet)
        {
            _entityBoundingBoxOutline = new RectangleShape
            {
                FillColor = Color.Transparent,
                OutlineColor = Color.Red,
                OutlineThickness = 1
            };

            _geometryBoundingBoxOutline = new RectangleShape
            {
                FillColor = Color.Transparent,
                OutlineColor = Color.White,
                OutlineThickness = 1
            };

            _sprite = new AnimatedSprite(Time.FromSeconds(0.1f), true, false);

            _south = new Animation(spriteSheet);
            AddFramesToAnimation(_south, SpriteHeight * 0);

            _north = new Animation(spriteSheet);
            AddFramesToAnimation(_north, SpriteHeight * 1);

            _east = new Animation(spriteSheet);
            AddFramesToAnimation(_east, SpriteHeight * 2);

            _west = new Animation(spriteSheet);
            AddFramesToAnimation(_west, SpriteHeight * 3);

            _northwest = new Animation(spriteSheet);
            AddFramesToAnimation(_northwest, SpriteHeight * 4);

            _northeast = new Animation(spriteSheet);
            AddFramesToAnimation(_northeast, SpriteHeight * 5);

            _southwest = new Animation(spriteSheet);
            AddFramesToAnimation(_southwest, SpriteHeight * 6);

            _southeast = new Animation(spriteSheet);
            AddFramesToAnimation(_southeast, SpriteHeight * 7);

            _currentAnimation = _south;

            _sprite.Animation = _currentAnimation;

            _footstepMusic = new Music("sounds/footstep.wav") { Loop = true };
        }

        private static void AddFramesToAnimation(Animation animation, int height)
        {
            for (var i = 0; i < NumFrames; i++)
            {
                animation.AddFrame(new IntRect(i * SpriteWidth, height, SpriteWidth, SpriteHeight));
            }
        }

        public void Update(Entity entity, RenderTarget target)
        {
            var velocity = entity.Velocity;

            // south
            if (velocity.X == 0 && velocity.Y > 0)
            {
                _currentAnimation = _south;
            }
            // east
            if (velocity.X > 0 && velocity.Y == 0)
            {
                _currentAnimation = _east;
            }
            // north
            if (velocity.X == 0 && velocity.Y < 0)
            {
                _currentAnimation = _north;
            }
            // west
            if (velocity.X < 0 && velocity.Y == 0)
            {
                _currentAnimation = _west;
            }
            // _northwest
            if (velocity.X < 0 && velocity.Y < 0)
            {
                _currentAnimation = _northwest;
            }
            // northeast
            if (velocity.X > 0 && velocity.Y < 0)
            {
                _currentAnimation = _northeast;
            }
            // southwest
            if (velocity.X < 0 && velocity.Y > 0)
            {
                _currentAnimation = _southwest;
            }
            // southeast
            if (velocity.X > 0 && velocity.Y > 0)
            {
                _currentAnimation = _southeast;
            }

            _sprite.Animation = _currentAnimation;
            _sprite.Pause = false;

            // if not moving, stop animation
            if (velocity.X == 0 && velocity.Y == 0)
            {
                _sprite.Stop();
                _footstepMusic.Stop();
            }
            else
            {
                if (_footstepMusic.Status == SoundStatus.Stopped)
                {
                    _footstepMusic.Play();
                }
            }

            _sprite.Update(Game.DeltaTime);

            _sprite.Position = entity.Position;

            var spriteBounds = _sprite.GlobalBounds;
            var geoBounds = new FloatRect(spriteBounds.Left + 3, spriteBounds.Top + spriteBounds.Height - 3, spriteBounds.Width - 6, 3);

            entity.EntityBoundingBox = spriteBounds;
            entity.GeometryBoundingBox = geoBounds;

            _entityBoundingBoxOutline.Position = new Vector2f(spriteBounds.Left, spriteBounds.Top);
            _entityBoundingBoxOutline.Size = new Vector2f(spriteBounds.Width, spriteBounds.Height);

            _geometryBoundingBoxOutline.Position = new Vector2f(geoBounds.Left, geoBounds.Top);
            _geometryBoundingBoxOutline.Size = new Vector2f(geoBounds.Width, geoBounds.Height);

            target.Draw(_sprite);

            if (!State.ShowEntityBoundingBoxes) return;

            target.Draw(_entityBoundingBoxOutline);
            target.Draw(_geometryBoundingBoxOutline);
        }
    }
}
