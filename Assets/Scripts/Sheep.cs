using UnityEngine;

namespace zs.Assets.Scripts
{
    public class Sheep : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private float _speed = 3f;

        [SerializeField]
        private Transform _spritesRight  = null;

        #endregion Serializable Fields

        #region Private Vars

        private Rigidbody2D _rigidbody = null;
        private Animator _animator = null;

        private bool _walkLeft = false;
        private bool _walkRight = false;

        private Vector2 _velocity = Vector2.zero;
        //private Vector2Int _lastTile = Vector2Int.zero;

        #endregion Private Vars

        #region Public Vars

        public bool IsAlive { get; private set; }

        #endregion Public Vars

        #region Public Methods

        public void Hit()
        {
            if (IsAlive)
            {
                IsAlive = false;
                _rigidbody.velocity = Vector2.zero;

                _animator.SetBool("Dead", true);

                Game.Instance.RegisterDeadSheep(this);
            }
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

            Debug.Assert(_rigidbody);
            Debug.Assert(_animator);
            Debug.Assert(_spritesRight);

            IsAlive = true;
        }

        void Start()
        {
            if (Random.value < 0.5f)
            {
                // Horizontal movement

                if (transform.position.x > 0)
                {
                    _velocity = Vector2.right * _speed;
                }
                else
                {
                    _velocity = Vector2.left * _speed;
                }
            }
            else
            {
                // Vertical movement

                if (transform.position.y > 0)
                {
                    _velocity = Vector2.up * _speed;
                }
                else
                {
                    _velocity = Vector2.down * _speed;
                }
            }

            //_lastTile = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
        }
	
        void Update()
        {
            if (!IsAlive)
            {
                return;
            }

            Map map = Game.Instance.Map;

            Vector2Int currentTile = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

            float inTileX = transform.position.x - currentTile.x;
            float inTileY = transform.position.y - currentTile.y;


            if (_velocity.x > 0)
            {
                if (inTileX >= 0.5f)
                {
                    if (map.GetTile(currentTile.x + 1, currentTile.y) == TileType.Blocked)
                    {
                        float rand = Random.value;
                        if (rand < 2f / 5f &&
                            map.GetTile(currentTile.x, currentTile.y + 1) == TileType.Open)
                        {
                            _velocity.x = 0;
                            _velocity.y = 1;
                        }
                        else if (rand < 4f / 5f &&
                            map.GetTile(currentTile.x, currentTile.y - 1) == TileType.Open)

                        {
                            _velocity.x = 0;
                            _velocity.y = -1;
                        }
                        else
                        {
                            _velocity.x = -1;
                        }

                        transform.position = new Vector3(currentTile.x + 0.5f, currentTile.y + 0.5f, transform.position.z);
                    }
                }
            }
            else if (_velocity.x < 0)
            {
                if (inTileX < 0.5f)
                {
                    if (map.GetTile(currentTile.x - 1, currentTile.y) == TileType.Blocked)
                    {
                        float rand = Random.value;
                        if (rand < 2f / 5f &&
                            map.GetTile(currentTile.x, currentTile.y + 1) == TileType.Open)
                        {
                            _velocity.x = 0;
                            _velocity.y = 1;
                        }
                        else if (rand < 4f / 5f &&
                                 map.GetTile(currentTile.x, currentTile.y - 1) == TileType.Open)

                        {
                            _velocity.x = 0;
                            _velocity.y = -1;
                        }
                        else
                        {
                            _velocity.x = 1;
                        }

                        transform.position = new Vector3(currentTile.x + 0.5f, currentTile.y + 0.5f, transform.position.z);
                    }
                }
            }
            else if (_velocity.y > 0)
            {
                if (inTileY >= 0.5f)
                {
                    if (map.GetTile(currentTile.x, currentTile.y + 1) == TileType.Blocked)
                    {
                        float rand = Random.value;
                        if (rand < 2f / 5f &&
                            map.GetTile(currentTile.x + 1, currentTile.y) == TileType.Open)
                        {
                            _velocity.x = 1;
                            _velocity.y = 0;
                        }
                        else if (rand < 4f / 5f &&
                                 map.GetTile(currentTile.x - 1, currentTile.y) == TileType.Open)

                        {
                            _velocity.x = -1;
                            _velocity.y = 0;
                        }
                        else
                        {
                            _velocity.y = -1;
                        }

                        transform.position = new Vector3(currentTile.x + 0.5f, currentTile.y + 0.5f, transform.position.z);
                    }
                }
            }
            else if (_velocity.y < 0)
            {
                if (inTileY < 0.5f)
                {
                    if (map.GetTile(currentTile.x, currentTile.y - 1) == TileType.Blocked)
                    {
                        float rand = Random.value;
                        if (rand < 2f / 5f &&
                            map.GetTile(currentTile.x + 1, currentTile.y) == TileType.Open)
                        {
                            _velocity.x = 1;
                            _velocity.y = 0;
                        }
                        else if (rand < 4f / 5f &&
                                 map.GetTile(currentTile.x - 1, currentTile.y) == TileType.Open)
                        {
                            _velocity.x = -1;
                            _velocity.y = 0;
                        }
                        else
                        {
                            _velocity.y = 1;
                        }

                        transform.position = new Vector3(currentTile.x + 0.5f, currentTile.y + 0.5f, transform.position.z);
                    }
                }
            }



            bool walkLeft = false;
            bool walkRight = false;


            if (_velocity.x > 0)
            {
                walkRight = true;
                _spritesRight.localScale = _spritesRight.localScale.with_x(1);
            }
            else if (_velocity.x < 0)
            {
                walkLeft = true;
                _spritesRight.localScale = _spritesRight.localScale.with_x(-1);
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


            _velocity = _velocity.normalized * _speed;

            _rigidbody.velocity = _velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsAlive && collision.collider.tag == "Player")
            {
                Player player = collision.collider.GetComponent<Player>();
                player.Hit();
            }
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }

}
