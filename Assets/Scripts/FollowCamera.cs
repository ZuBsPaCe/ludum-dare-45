using UnityEngine;

namespace zs.Assets.Scripts
{
    public class FollowCamera : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private Transform _followObject = null;

        [SerializeField]
        private Transform[] _followObjects = null;

        #endregion Serializable Fields

        #region Private Vars
        #endregion Private Vars

        #region Public Vars

        public static FollowCamera Instance { get; private set; }

        #endregion Public Vars

        #region Public Methods

        public Transform FollowObject
        {
            get { return _followObject; }
            set
            {
                _followObjects = null;
                _followObject = value;
            }
        }

        public Transform[] FollowObjects
        {
            get { return _followObjects; }
            set
            {
                _followObject = null;

                if (value == null || value.Length == 0)
                {
                    _followObjects = null;
                }
                else
                {
                    _followObjects = value;
                }
            }
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
        }
	
        void LateUpdate()
        {
            if (_followObject != null)
            {
                transform.position = _followObject.position.with_z(transform.position.z);
            }
            else if (_followObjects != null && _followObjects.Length > 0)
            {
                int followCount = 0;
                Vector3 center = Vector3.zero;
                foreach (Transform followObject in _followObjects)
                {
                    Player player = followObject.GetComponent<Player>();

                    if (player != null)
                    {
                        if (player.IsAlive)
                        {
                            center += followObject.position;
                            followCount += 1;
                        }
                    }
                    else
                    {
                        center += followObject.position;
                        followCount += 1;
                    }
                }

                if (followCount > 0)
                {
                    center /= followCount;
                    transform.position = center.with_z(transform.position.z);
                }
            }
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
