using UnityEngine;

public class Woosh : MonoBehaviour
{
	#region Serializable Fields

    [SerializeField]
    private SpriteRenderer _spriteRenderer = null;

	#endregion Serializable Fields

	#region Private Vars

    private float _alpha = 0;
    private float _startVal = 0;

	#endregion Private Vars

	#region Public Vars
	#endregion Public Vars

	#region Public Methods
	#endregion Public Methods

	#region MonoBehaviour
	
	void Awake()
	{
        Debug.Assert(_spriteRenderer);
	}

	void Start()
    {
        _alpha = _spriteRenderer.color.a;
        _startVal = _alpha;
    }
	
	void Update()
    {
        _alpha -= Time.deltaTime * _startVal * 10f;

        if (_alpha < 0)
        {
            Destroy(gameObject);
        }
        else
        {
            _spriteRenderer.color = new Color(1, 1, 1, _alpha);

        }
    }

	#endregion MonoBehaviour

	#region Private Methods
	#endregion Private Methods
}
