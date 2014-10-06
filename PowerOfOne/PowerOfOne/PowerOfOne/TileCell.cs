using System;
using System.Collections.Generic;

namespace PowerOfOne
{
    [Serializable]
    public class TileCell
    {
        public Tile tile { get; set; }

        public bool hasTile { get; set; }

        public TileCell(int id,int tileSet)
        {
            this.tile = new Tile(id,tileSet);
            MergeTiles = new List<Tile>();
            TopTiles = new List<Tile>();
            hasTile = true;
        }

        public List<Tile> MergeTiles { get; set; }

        public List<Tile> TopTiles { get; set; }

        public void AddMergeTile(int Id,int TileSet)
        {
            MergeTiles.Add(new Tile(Id,TileSet));
        }

        public void AddTopTile(int Id, int TileSet)
        {
            TopTiles.Add(new Tile(Id,TileSet));
        }
    }
}
