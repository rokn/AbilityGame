using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PowerOfOne
{
    public static class TileSet
    {
        public static List<Texture2D> SpriteSheet { get; set; }

        public static int tileWidth { get; set; }

        public static int tileHeight { get; set; }

        public static Rectangle GetSourceRectangle(Tile tile)
        {
            int tilesPerRow = SpriteSheet[tile.tileSet].Width / tileWidth;
            int sourceY = tile.Id / tilesPerRow;
            int sourceX = tile.Id - sourceY * tilesPerRow;
            Rectangle source = new Rectangle(sourceX * tileWidth, sourceY * tileHeight, tileWidth, tileHeight);
            return source;
        }
    }
}
