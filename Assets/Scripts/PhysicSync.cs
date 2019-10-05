using UnityEngine;

namespace zs.Assets.Scripts
{
    public class PhysicSync : MonoBehaviour
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
	
        void Awake()
        {
            Physics2D.autoSimulation = false;
        }

        void Update()
        {
            float physicStep = Mathf.Min(Time.deltaTime, 1f / 30f);
            Physics2D.Simulate(physicStep);
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
