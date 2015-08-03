﻿using SFML.Graphics;
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

        public void RotateAroundCenter(float degrees)
        {
            Vector2f center = (Vector2f) type.texture.Size / 2;
            Vector2f rotateOrigin = sprite.Position + center;
            
        }

        public void RotateBasedOnDirection(Direction direction)
        {
            switch (direction)
            {
                // door sprite is already aligned with North direction
                case Direction.East:
                    RotateAroundCenter(90);
                    break;
                case Direction.South:
                    RotateAroundCenter(180);
                    break;
                case Direction.West:
                    RotateAroundCenter(-90);
                    break;
            }
        }

        public void SetPosition(Vector2f position)
        {
            sprite.Position = position;
            foreach (Tile t in children)
            {
                t.SetPosition(position);
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
