namespace zs.Assets.Scripts
{
    public class MapTile
    {
        #region Private Vars
        #endregion Private Vars

        #region Public Vars

        public TileType TileType { get; set; }

        #endregion Public Vars

        #region Public Methods

        public MapTile(TileType tileType)
        {
            TileType = tileType;
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
