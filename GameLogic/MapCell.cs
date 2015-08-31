using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ending.GameLogic.DungeonTools;
using Ending.SpriteTools;
using SFML.Graphics;
using SFML.System;

namespace Ending.GameLogic
{
    public class MapCell : Drawable
    {
        public readonly Stack<TileSprite> Tiles = new Stack<TileSprite>();

        public void AddTile(TileType type)
        {
            if (Tiles.Count > 0 && Tiles.Peek().TextureName == type.TextureName) return;

            Tiles.Push(new TileSprite(type));
        }

        public bool IsEmpty() => Tiles.Count == 0;

        public void SetPosition(int x, int y)
        {
            foreach (var t in Tiles)
                t.Position = new Vector2f(x, y);
        }

        public void LightPass(Color color)
        {
            foreach (var t in Tiles.Where(t => t.LightingEnabled))
                t.Color = color;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var tile in Tiles)
                tile.Draw(target, states);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Tiles.Count);
            foreach (var t in Tiles)
            {
                bw.Write(t.TextureName);
                bw.Write(t.LightingEnabled);
            }
        }

        public static MapCell Read(BinaryReader br)
        {
            var cell = new MapCell();

            var tileCount = br.ReadInt32();
            for (var i = 0; i < tileCount; i++)
                cell.Tiles.Push(new TileSprite(br.ReadString(), br.ReadBoolean()));

            return cell;
        }
    }
}
