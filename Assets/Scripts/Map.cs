using System;
using System.Collections.Generic;
using UnityEngine;

namespace zs.Assets.Scripts
{
    public class Map
    {
        #region Private Vars

        private List<MapTile> _tiles;

        #endregion Private Vars

        #region Public Vars

        public int Width { get; private set; }
        public int Height { get; private set; }

        #endregion Public Vars

        #region Public Methods

        public Map(int width, int height)
        {
            Width = width;
            Height = height;

            int tileCount = width * height;

            _tiles = new List<MapTile>(tileCount);

            for (int i = 0; i < tileCount; ++i)
            {
                _tiles.Add(new MapTile(TileType.Outside));
            }
        }

        public TileType GetTile(int x, int y)
        {
            if (!IsValid(x, y))
            {
                Debug.Assert(false, "Get Tile Coord out of bounds!");
                return TileType.Outside;
            }

            return _tiles[y * Width + x].TileType;
        }

        public void SetTile(int x, int y, TileType tileType)
        {
            if (!IsValid(x, y))
            {
                Debug.Assert(false, "Set Tile Coord out of bounds!");
                return;
            }

            _tiles[y * Width + x].TileType = tileType;
        }

        public bool IsTileSurroundedBy(int x, int y, TileType tileType)
        {
            for (int currentY = y - 1; currentY <= y + 1; ++currentY)
            {
                for (int currentX = x - 1; currentX <= x + 1; ++currentX)
                {
                    if (!IsValid(currentX, currentY))
                    {
                        return false;
                    }

                    if (GetTile(currentX, currentY) != tileType)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void AddBorder(int borderSize, TileType tileType)
        {
            int newWidth = Width + 2 * borderSize;
            int newHeight = Height + 2 * borderSize;


            int newTileCount = newWidth * newHeight;

            var newTiles = new List<MapTile>(newTileCount);

            for (int i = 0; i < newTileCount; ++i)
            {
                newTiles.Add(new MapTile(tileType));
            }


            int offsetX = borderSize;
            int offsetY = borderSize;

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    newTiles[(offsetY + y) * newWidth + (offsetY + x)].TileType = GetTile(x, y);
                }
            }

            Width = newWidth;
            Height = newHeight;
            _tiles = newTiles;
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
