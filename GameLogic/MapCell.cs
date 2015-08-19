using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Ending.GameLogic.DungeonTools;
using Ending.SpriteTools;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public class MapCell : Drawable
    {
        private readonly Stack<TileSprite> _tiles = new Stack<TileSprite>();

        public void AddTile(TileType type)
        {
            if (_tiles.Count > 0 && _tiles.Peek().Type == type) return;

            _tiles.Push(new TileSprite(type));
        }

        public bool IsEmpty() => _tiles.Count == 0;

        public void SetPosition(int x, int y)
        {
            foreach (var t in _tiles)
                t.Position = new Vector2f(x, y);
        }

        public void LightPass(Color color)
        {
            foreach (var t in _tiles.Where(t => t.LightingEnabled))
                t.Color = color;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var tile in _tiles)
                tile.Draw(target, states);
        }

        public void Write(BinaryWriter bw)
        {
            
        }
    }
}
