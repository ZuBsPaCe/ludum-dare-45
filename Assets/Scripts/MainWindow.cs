using UnityEngine;

namespace zs.Assets.Scripts
{
    public class MainWindow : MonoBehaviour
    {
        #region Serializable Fields
        #endregion Serializable Fields

        #region Private Vars
        #endregion Private Vars

        #region Public Vars
        #endregion Public Vars

        #region Public Methods

        public void Generate()
        {
            Game.Instance.Generate();
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
