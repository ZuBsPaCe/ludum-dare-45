using UnityEngine;

namespace zs.Assets.Scripts
{
    public class Pitchfork : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private Player _player = null;

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
        }

        void Start()
        {
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_player != null && collision.tag == "Sheep")
            {
                Sheep sheep = collision.GetComponent<Sheep>();
                sheep.Hit(_player);
            }
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
