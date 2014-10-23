using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PowerOfOne
{
    public class TileMap
    {
        public TileCell[,] tileMap { get; set; }
        SpriteBatch sB;

        public TileMap(Vector2 pos,int width,int height)
        {
            this.Position = pos;
            this.tileMap = new TileCell[width, height];
            this.Width = width;
            this.Height = height;
            Initialize();
        }

        public Vector2 Position { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        private void Initialize()
        {
            
            for (int x = 0; x < Width; x++)
            {

                for (int y = 0; y < Height; y++)
                {
                    tileMap[x, y] = new TileCell(7,2);

                    if (x == 0 && y == 0)
                    {
                        tileMap[x, y] = new TileCell(3, 2);
                    }
                    else if (x == 0)
                    {
                        tileMap[x, y] = new TileCell(6, 2);
                    }
                    else if (y == 0)
                    {
                        tileMap[x, y] = new TileCell(4, 2);
                    }

                    if (x == Width - 1 && y == 0)
                    {
                        tileMap[x, y] = new TileCell(5, 2);
                    }
                    else if (x == Width - 1)
                    {
                        tileMap[x, y] = new TileCell(8, 2);
                    }

                    if (x == Width - 1 && y == Height -1)
                    {
                        tileMap[x, y] = new TileCell(11, 2);
                    }
                    else if (y == Height - 1)
                    {
                        tileMap[x, y] = new TileCell(10, 2);
                    }

                    if (x == 0 && y == Height - 1)
                    {
                        tileMap[x, y] = new TileCell(9, 2);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sB = spriteBatch;
            for (int x = 0; x < Width; x++)
            {

                for (int y = 0; y < Height; y++)
                {
                    float defaultDepth = 0.1f;
                    float topDepth = defaultDepth + (y / 1000f);

                    Vector2 pos = Position + new Vector2(TileSet.tileWidth * x, TileSet.tileHeight * y);

                    if (tileMap[x, y].hasTile)
                    {
                        DrawTile(tileMap[x, y].tile, pos, defaultDepth);
                    }

                    int mergeTilesDepth = 1;

                    foreach (Tile tile in tileMap[x, y].MergeTiles)
                    {
                        DrawTile(tile, pos, defaultDepth + 0.000001f * mergeTilesDepth);
                        mergeTilesDepth++;
                    }

                    int topTilesDepth = 1;

                    foreach (Tile tile in tileMap[x, y].TopTiles)
                    {
                        DrawTile(tile, pos - new Vector2(0, TileSet.tileHeight * topTilesDepth), topDepth + 0.00001f * topTilesDepth + 0.1f);
                        topTilesDepth++;
                    }

                }

            }
        }

        private bool CheckIsTileInView(float x, float y)
        {
            bool left = x + TileSet.tileWidth > Main.camera.Position.X;
            bool top = y + TileSet.tileHeight > Main.camera.Position.Y;
            bool right = x < Main.camera.Position.X + Main.width;
            bool down = y < Main.camera.Position.Y + Main.height;
            return left && top && right && down;
        }

        private void DrawTile(Tile tile,Vector2 pos,float depth)
        {
            if (CheckIsTileInView(pos.X, pos.Y))
            {
                sB.Draw(TileSet.SpriteSheet[tile.tileSet], pos, TileSet.GetSourceRectangle(tile), Color.White, 0, new Vector2(), 1f, SpriteEffects.None, depth);
            }
        }

        public void AddMergeTile(int x, int y, int TileId, int tileSet)
        {
            tileMap[x, y].AddMergeTile(TileId, tileSet);
        }

        public void AddTopTile(int x, int y, int TileId, int tileSet)
        {
            tileMap[x, y].AddTopTile(TileId, tileSet);
        }

        public void ChangeBaseTile(int x, int y, int TileId, int tileSet)
        {
            if(!tileMap[x,y].hasTile)
            {
                tileMap[x, y].hasTile = true;
            }

            tileMap[x, y].tile = new Tile(TileId, tileSet);
        }

        public void RemoveMergeTile(int x, int y)
        {
            if (tileMap[x, y].MergeTiles.Count > 0)
            {
                tileMap[x, y].MergeTiles.RemoveLast();
            }
        }

        public void RemoveTopTile(int x, int y)
        {
            if (tileMap[x, y].TopTiles.Count > 0)
            {
                tileMap[x, y].TopTiles.RemoveLast();
            }
        }

        public void RemoveBaseTile(int x, int y)
        {
            tileMap[x, y].hasTile = false;
        }

        public void ClearTileMap()
        {
            for (int x = 0; x < Width; x++)
            {

                for (int y = 0; y < Height; y++)
                {

                    tileMap[x, y] = new TileCell(7, 2);

                    if (x == 0 && y == 0)
                    {
                        tileMap[x, y] = new TileCell(3, 2);
                    }
                    else if (x == 0)
                    {
                        tileMap[x, y] = new TileCell(6, 2);
                    }
                    else if (y == 0)
                    {
                        tileMap[x, y] = new TileCell(4, 2);
                    }

                    if (x == Width - 1 && y == 0)
                    {
                        tileMap[x, y] = new TileCell(5, 2);
                    }
                    else if (x == Width - 1)
                    {
                        tileMap[x, y] = new TileCell(8, 2);
                    }

                    if (x == Width - 1 && y == Height - 1)
                    {
                        tileMap[x, y] = new TileCell(11, 2);
                    }
                    else if (y == Height - 1)
                    {
                        tileMap[x, y] = new TileCell(10, 2);
                    }

                    if (x == 0 && y == Height - 1)
                    {
                        tileMap[x, y] = new TileCell(9, 2);
                    }
                }

            }

        }
    }
}
