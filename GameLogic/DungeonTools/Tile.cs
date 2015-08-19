using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic.DungeonTools
{
    public class Tile : Drawable
    {
        public const int TileWidth = 32;
        public const int TileHeight = 32;

        public Sprite Sprite { get; }

        public TileType Type { get; }

        public FloatRect GlobalBounds => Sprite.GetGlobalBounds();

        public Stack<Tile> Children { get; }

        public Tile(TileType type)
        {
            Type = type;
            Sprite = new Sprite(type.Texture);
            Children = new Stack<Tile>();
        }

        public bool Contains(TileType type)
        {
            return Type == type || Children.Any(t => t.Type == type);
        }

        public void SetPosition(Vector2f position)
        {
            Sprite.Position = position;
            foreach (var t in Children)
                t.SetPosition(position);
        }

        public void LightPass(Color color)
        {
            if (Type.LightingEnabled)
            {
                Sprite.Color = color;
            }

            foreach (var t in Children.Where(t => t.Type.LightingEnabled))
            {
                t.Sprite.Color = color;
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Sprite, states);

            foreach (var t in Children)
            {
                target.Draw(t, states);
            }
        }

        public static bool operator ==(Tile a, Tile b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
                return true;

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            return a.Type == b.Type;
        }

        public static bool operator !=(Tile a, Tile b) => !(a == b);

    }
}