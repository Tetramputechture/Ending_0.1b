using System.Collections.Generic;
using System.Linq;
using Ending.GameLogic.DungeonTools;
using SFML.Graphics;

namespace Ending.GameLogic
{
    public class MapCell : Drawable
    {
        private readonly Stack<Tile> _tiles = new Stack<Tile>();

        public void AddTile(Tile tile)
        {
            if (_tiles.Count > 0 && _tiles.Peek() == tile) return;

            _tiles.Push(tile);
        }

        public bool IsEmpty() => _tiles.Count == 0;

        public void LightPass(Color color)
        {
            foreach (var t in _tiles.Where(t => t.Type.LightingEnabled))
                t.Sprite.Color = color;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var tile in _tiles)
                tile.Draw(target, states);
        }
    }
}
