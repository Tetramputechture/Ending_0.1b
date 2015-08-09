using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ending.GameLogic.DungeonTools
{
    public class Tile : Drawable
    {
        public const int TILE_WIDTH = 32;
        public const int TILE_HEIGHT = 32;

        public Sprite sprite { get; }

        public TileType type { get; }

        public FloatRect globalBounds
        {
            get
            {
                return sprite.GetGlobalBounds();
            }
        }

        public Stack<Tile> children { get; }

        public Tile(TileType type)
        {
            this.type = type;
            sprite = new Sprite(type.texture);
            children = new Stack<Tile>();
        }

        public bool Contains(TileType type)
        {
            if (this.type == type)
            {
                return true;
            }

            foreach (Tile t in children)
            {
                if (t.type == type)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetPosition(Vector2f position)
        {
            sprite.Position = position;
            foreach (Tile t in children)
            {
                t.SetPosition(position);
            }
        }

        public void LightPass(Color color)
        {
            if (type.lightingEnabled)
            {
                sprite.Color = color;
            }
            foreach (Tile t in children)
            {
                if (t.type.lightingEnabled)
                {
                    t.sprite.Color = color;
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(sprite, states);

            foreach (Tile t in children)
            {
                target.Draw(t, states);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Tile t = (Tile)obj;

            return type == t.type && type.passable == type.passable;
        }
    }
}