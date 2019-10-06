using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace zs.Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private float _speed = 3f;

        [SerializeField]
        private Transform _spritesLeft = null;

        [SerializeField]
        private Transform _pitchforkLeft = null;

        #endregion Serializable Fields

        #region Private Vars

        private Rigidbody2D _rigidbody = null;
        private Animator _animator = null;

        private bool _walkLeft = false;
        private bool _walkRight = false;

        private float _lastFireTime = 0;

        #endregion Private Vars

        #region Public Vars

        public bool HasPitchfork { get; private set; }

        public bool IsAlive { get; private set; }

        #endregion Public Vars

        #region Public Methods

        public void PickupPitchfork()
        {
            if (HasPitchfork)
            {
                Debug.Assert(false);
                return;
            }

            HasPitchfork = true;

            _pitchforkLeft.gameObject.SetActive(true);
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            Debug.Assert(_rigidbody);
            Debug.Assert(_animator);
            Debug.Assert(_spritesLeft);

            Debug.Assert(_pitchforkLeft);

            IsAlive = true;
        }

        void Start()
        {
        }
	
        void Update()
        {
            if (Game.Instance.LevelDone)
            {
                _rigidbody.velocity = Vector2.zero;
                return;
            }

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

            bool walkLeft = false;
            bool walkRight = false;


            if (velocity.x > 0)
            {
                walkRight = true;
                _spritesLeft.localScale = _spritesLeft.localScale.with_x(-1);
            }
            else if (velocity.x < 0)
            {
                walkLeft = true;
                _spritesLeft.localScale = _spritesLeft.localScale.with_x(1);
            }

            if (_walkRight != walkRight)
            {
                _walkRight = walkRight;
                _animator.SetBool("WalkRight", _walkRight);
            }

            if (_walkLeft != walkLeft)
            {
                _walkLeft = walkLeft;
                _animator.SetBool("WalkLeft", _walkLeft);
            }


            velocity = velocity.normalized * _speed;

            _rigidbody.velocity = velocity;


            if (HasPitchfork)
            {
                if (Time.time - _lastFireTime > 0.5f)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        _animator.SetBool("Fire", true);
                        _lastFireTime = Time.time;
                    }
                }
                else
                {
                    _animator.SetBool("Fire", false);
                }
            }
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
