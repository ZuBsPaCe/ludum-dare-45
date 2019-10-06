using UnityEngine;

namespace zs.Assets.Scripts
{
    public class Haystack : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private Transform _pitchfork = null;

        #endregion Serializable Fields

        #region Private Vars

        private bool _pitchforkTaken = false;

        #endregion Private Vars

        #region Public Vars
        #endregion Public Vars

        #region Public Methods

        public void SetPitchforkTaken()
        {
            _pitchfork.gameObject.SetActive(false);
            _pitchforkTaken = true;
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            Debug.Assert(_pitchfork);
        }

        void Start()
        {
            _pitchfork.rotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f)) * _pitchfork.rotation;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_pitchforkTaken)
            {
                return;
            }

            if (collision.tag == "Player")
            {
                Player player = collision.GetComponent<Player>();
                if (!player.HasPitchfork)
                {
                    SetPitchforkTaken();
                    player.PickupPitchfork();
                }
            }
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
