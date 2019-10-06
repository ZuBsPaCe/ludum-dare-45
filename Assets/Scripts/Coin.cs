using UnityEngine;
using zs.Assets.Scripts;

namespace zs.Assets
{
    public class Coin : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private Rigidbody2D _rigidbody = null;

        [SerializeField]
        private AnimationCurve _bounceCurve = null;

        [SerializeField]
        private Transform _spriteTransform = null;

        [SerializeField]
        private SpriteRenderer _spriteRenderer = null;

        #endregion Serializable Fields

        #region Private Vars

        private float _time = 0;

        #endregion Private Vars

        #region Public Vars

        public Rigidbody2D Rigidbody
        {
            get { return _rigidbody; }
        }

        #endregion Public Vars

        #region Public Methods
        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            Debug.Assert(_rigidbody);
            Debug.Assert(_spriteTransform);
            Debug.Assert(_spriteRenderer);

            _time = 0;
        }

        void Start()
        {
        }
	
        void Update()
        {
            _time += Time.deltaTime;

            if (_time <= 2f)
            {
                _spriteTransform.localPosition = _spriteTransform.localPosition.with_y(_bounceCurve.Evaluate(_time * 0.5f));
            }
            else if (_time > 4f)
            {
                _spriteRenderer.enabled = ((int) (_time * 6f) % 2 == 0);

                if (_time > 6f)
                {
                    DestroyImmediate(gameObject);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.tag == "Player")
            {
                Player player = collision.collider.GetComponent<Player>();
                if (player.IsAlive)
                {
                    Game.Instance.RegisterCoinPickup(this);
                }
            }

        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
