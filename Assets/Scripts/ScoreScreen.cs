using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace zs.Assets.Scripts
{
    public class ScoreScreen : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private Text _sheepLabel = null;

        [SerializeField]
        private Text _coinsLabel = null;

        [SerializeField]
        private Text _peasantLabel = null;

        [SerializeField]
        private Text _timeLabel = null;

        [SerializeField]
        private Text _totalLabel = null;

        [SerializeField]
        private Text _sheepCounter = null;

        [SerializeField]
        private Text _coinsCounter = null;

        [SerializeField]
        private Text _peasantCounter = null;

        [SerializeField]
        private Text _timeCounter = null;

        [SerializeField]
        private Text _totalCounter = null;

        [SerializeField]
        private Text _continueText = null;

        [SerializeField]
        private Text _levelText = null;

        #endregion Serializable Fields

        #region Private Vars

        private int _sequence = 0;
        private float _waitUntilTime = 0;

        private int _displayedSheep = 0;
        private int _displayedCoins = 0;
        private int _displayedTime = 0;
        private int _displayedPeasants = 0;

        private int _displayedMultiplier = 0;

        private bool _skip = false;

        #endregion Private Vars

        #region Public Vars
        #endregion Public Vars

        #region Public Methods
        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            Debug.Assert(_sheepLabel);
            Debug.Assert(_coinsLabel);
            Debug.Assert(_timeLabel);
            Debug.Assert(_peasantLabel);
            Debug.Assert(_totalLabel);


            Debug.Assert(_sheepCounter);
            Debug.Assert(_coinsCounter);
            Debug.Assert(_peasantCounter);
            Debug.Assert(_timeCounter);
            Debug.Assert(_totalCounter);

            Debug.Assert(_levelText);
            Debug.Assert(_continueText);
        }

        private void OnEnable()
        {
            _sequence = 0;
            _waitUntilTime = 0;

            _levelText.text = "Level " + (Master.Instance.CurrentLevel + 1);

            _displayedSheep = Game.Instance.KilledSheep;
            _displayedCoins = Game.Instance.Coins;
            _displayedTime = Game.Instance.SecondsLeft;
            _displayedPeasants = Game.Instance.AlivePlayerCount;

            UpdateCounters();

            _skip = false;
        }

        void Update()
        {
            if (SkipPressed())
            {
                _skip = true;
            }

            if (!_skip && Time.time < _waitUntilTime)
            {
                return;
            }

            if (_sequence == 0)
            {
                _totalLabel.gameObject.SetActive(true);
                _totalCounter.gameObject.SetActive(true);
                SoundPlayer.Instance.Play(Sound.ScoreBam);

                _waitUntilTime = Time.time + 1f;
                _sequence += 1;
            }
            else if (_sequence == 1)
            {
                _sheepLabel.gameObject.SetActive(true);
                _sheepCounter.gameObject.SetActive(true);
                SoundPlayer.Instance.Play(Sound.ScoreBam);

                _waitUntilTime = Time.time + 0.3f;
                _sequence += 1;
            }
            else if (_sequence == 2)
            {
                _coinsLabel.gameObject.SetActive(true);
                _coinsCounter.gameObject.SetActive(true);
                SoundPlayer.Instance.Play(Sound.ScoreBam);

                _waitUntilTime = Time.time + 0.3f;
                _sequence += 1;
            }
            else if (_sequence == 3)
            {
                _timeLabel.gameObject.SetActive(true);
                _timeCounter.gameObject.SetActive(true);
                SoundPlayer.Instance.Play(Sound.ScoreBam);

                _waitUntilTime = Time.time + 0.3f;
                _sequence += 1;
            }
            else if (_sequence == 4)
            {
                _peasantLabel.gameObject.SetActive(true);
                _peasantCounter.gameObject.SetActive(true);
                SoundPlayer.Instance.Play(Sound.ScoreBam);

                _waitUntilTime = Time.time + 1.5f;
                _sequence += 1;

                if (_displayedPeasants == 0)
                {
                    _sequence += 1;
                }
            }
            else if (_sequence == 5)
            {
                _displayedPeasants -= 1;
                _displayedMultiplier += 1;

                SoundPlayer.Instance.Play(Sound.Bling);

                UpdateCounters();

                if (_displayedPeasants > 0)
                {
                    _waitUntilTime = Time.time + 0.3f;
                }
                else
                {
                    _waitUntilTime = Time.time + 1.5f;
                    _sequence += 1;
                }
            }
            else if (_sequence == 6)
            {
                bool notFinished = false;
                int multiplier = Mathf.Max(1, _displayedMultiplier);

                if (_displayedSheep > 0)
                {
                    _displayedSheep -= 1;
                    notFinished = true;
                    Master.Instance.TotalScore += multiplier;
                }

                if (_displayedCoins > 0)
                {
                    _displayedCoins -= 1;
                    notFinished = true;
                    Master.Instance.TotalScore += multiplier;
                }

                if (_displayedTime > 0)
                {
                    _displayedTime -= 1;
                    notFinished = true;
                    Master.Instance.TotalScore += multiplier;
                }

                if (notFinished)
                {
                    SoundPlayer.Instance.Play(Sound.Bling);
                    UpdateCounters();
                    _waitUntilTime = Time.time + 0.05f;
                }
                else
                {
                    _waitUntilTime = Time.time + 1f;
                    _sequence += 1;
                }
            }
            else if(_sequence == 7)
            {
                _continueText.gameObject.SetActive(true);
                _sequence += 1;
            }
            else
            {
                if (SkipPressed())
                {
                    SoundPlayer.Instance.Play(Sound.Click);

                    if (Game.Instance.AlivePlayerCount > 0)
                    {
                        Master.Instance.StartNextLevel();
                    }
                    else
                    {
                        Master.Instance.SetSceneType(SceneType.StartScreen);
                    }
                }
            }
        }

        #endregion MonoBehaviour

        #region Private Methods

        private bool SkipPressed()
        {
            if (Input.anyKeyDown)
            {
                return true;
            }

            return false;
        }

        private void UpdateCounters()
        {
            _sheepCounter.text = _displayedSheep.ToString();
            _coinsCounter.text = _displayedCoins.ToString();
            _timeCounter.text = _displayedTime.ToString();
            _peasantCounter.text = _displayedPeasants.ToString();
            _totalCounter.text = Master.Instance.TotalScore.ToString();

            if (_displayedMultiplier > 0)
            {
                _sheepCounter.text += "  x" + _displayedMultiplier;
                _coinsCounter.text += "  x" + _displayedMultiplier;
                _timeCounter.text += "  x" + _displayedMultiplier;
                _peasantCounter.text += "  x" + _displayedMultiplier;
            }
        }

        #endregion Private Methods
    }
}
