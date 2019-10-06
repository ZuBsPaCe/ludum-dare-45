using System;
using UnityEngine;
using zs.Assets.Scripts;

public class SoundPlayer : MonoBehaviour
{
	#region Serializable Fields

    [SerializeField]
    private AudioClip _sheepHit = null;

    [SerializeField]
    private AudioClip _playerHit = null;

    [SerializeField]
    private AudioClip _click = null;

    [SerializeField]
    private AudioClip _bling = null;

    [SerializeField]
    private AudioClip _playerFire = null;

    [SerializeField]
    private AudioClip _scoreBam = null;

	#endregion Serializable Fields

	#region Private Vars

    private AudioSource _audioSource;

	#endregion Private Vars

	#region Public Vars

    public static SoundPlayer Instance { get; private set; }

    #endregion Public Vars

	#region Public Methods

    public void Play(Sound sound, AudioSource audioSource = null)
    {
        if (audioSource == null)
        {
            audioSource = _audioSource;
        }

        switch (sound)
        {
            case Sound.SheepHit:
                audioSource.PlayOneShot(_sheepHit);
                break;
            case Sound.PlayerHit:
                audioSource.PlayOneShot(_playerHit);
                break;
            case Sound.Click:
                audioSource.PlayOneShot(_click);
                break;
            case Sound.Bling:
                audioSource.PlayOneShot(_bling, 0.7f);
                break;
            case Sound.PlayerFire:
                audioSource.PlayOneShot(_playerFire);
                break;
            case Sound.ScoreBam:
                audioSource.PlayOneShot(_scoreBam);
                break;
        }
    }

	#endregion Public Methods

	#region MonoBehaviour
	
	void Awake()
    {
        Instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

	void Start()
	{
	}
	
	void Update()
	{
	}

	#endregion MonoBehaviour

	#region Private Methods
	#endregion Private Methods
}
