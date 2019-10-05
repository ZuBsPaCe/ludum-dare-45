using UnityEngine;

namespace zs.Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private float _speed = 3f;

        #endregion Serializable Fields

        #region Private Vars

        private Rigidbody2D _rigidbody = null;

        #endregion Private Vars

        #region Public Vars
        #endregion Public Vars

        #region Public Methods
        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            Debug.Assert(_rigidbody);
        }

        void Start()
        {
        }
	
        void Update()
        {
            Vector2 velocity = Vector2.zero;

            float horAxis = Input.GetAxisRaw("Horizontal");
            float verAxis = Input.GetAxisRaw("Vertical");

            if (horAxis > 0)
            {
                velocity.x += 1;
            }

            if (horAxis < 0)
            {
                velocity.x -= 1;
            }

            if (verAxis > 0)
            {
                velocity.y += 1;
            }

            if (verAxis < 0)
            {
                velocity.y -= 1;
            }

            velocity = velocity.normalized * _speed;

            _rigidbody.velocity = velocity;
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
