using UnityEditor;
using UnityEngine;

namespace zs.Editor
{
    public class SpriteImporter : AssetPostprocessor
    {
        #region Serializable Fields
        #endregion Serializable Fields

        #region Private Vars
        #endregion Private Vars

        #region Public Vars
        #endregion Public Vars

        #region Public Methods
        #endregion Public Methods

        #region MonoBehaviour
	
        void OnPostprocessTexture(Texture2D texture)
        {
            TextureImporter textureImporter = (TextureImporter) assetImporter;
            textureImporter.spritePixelsPerUnit = 256;
            //textureImporter.alphaSource = TextureImporterAlphaSource.None;
            textureImporter.filterMode = FilterMode.Point;
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
