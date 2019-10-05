using UnityEditorInternal;
using UnityEngine;

namespace zs.Assets.Scripts
{
    public class FollowCamera : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private Transform _followObject = null;

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
            set { _followObject = value; }
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            Debug.Assert(_followObject);

            Instance = this;
        }

        void Start()
        {
        }
	
        void LateUpdate()
        {
            if (_followObject == null)
            {
                return;
            }

            transform.position = _followObject.position.with_z(transform.position.z);
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
