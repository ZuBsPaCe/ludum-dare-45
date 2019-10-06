using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace zs.Assets.Scripts
{
    public class MainWindow : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private RectTransform _startScreen = null;

        [SerializeField]
        private Text _difficultyButtonText = null;

        [SerializeField]
        private Button _exitButton = null;

        [SerializeField]
        private Button _generateButton = null;

        [SerializeField]
        private RectTransform _levelEndStatusYou = null;

        [SerializeField]
        private RectTransform _levelEndStatusWonSuccess = null;

        [SerializeField]
        private RectTransform _levelEndStatusSheep = null;

        [SerializeField]
        private RectTransform _levelEndStatusWonFailed = null;

        [SerializeField]
        private PlayableAsset _timelineLevelWon = null;

        [SerializeField]
        private PlayableAsset _timelineLevelFailed = null;

        [SerializeField]
        private Sheep _sheepPrefab = null;

        #endregion Serializable Fields

        #region Private Vars

        private PlayableDirector _playableDirector = null;
        private Sheep _startScreenSheep = null;

        #endregion Private Vars

        #region Public Vars

        public static MainWindow Instance { get; private set; }

        #endregion Public Vars

        #region Public Methods

        public void SetStartScreenActive(bool active)
        {
            _startScreen.gameObject.SetActive(active);

            if (active)
            {
                if (_startScreenSheep == null)
                {
                    _startScreenSheep = Instantiate(_sheepPrefab, new Vector3(0, -2.5f, 0), Quaternion.identity);
                    _startScreenSheep.transform.localScale = new Vector3(2f, 2f, 2f);
                }
            }
            else
            {
                if (_startScreenSheep != null)
                {
                    Destroy(_startScreenSheep);
                    _startScreenSheep = null;
                }
            }
        }

        public void StartGame()
        {
            Master.Instance.StartNewGame();
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void CycleDifficulty()
        {
            if (Master.Instance.CurrentDifficulty == Difficulty.Easy)
            {
                Master.Instance.CurrentDifficulty = Difficulty.Normal;
            }
            else if (Master.Instance.CurrentDifficulty == Difficulty.Normal)
            {
                Master.Instance.CurrentDifficulty = Difficulty.Hard;
            }
            else if (Master.Instance.CurrentDifficulty == Difficulty.Hard)
            {
                Master.Instance.CurrentDifficulty = Difficulty.Easy;
            }

            UpdateDifficultyText();
        }

        public void Generate()
        {
            Master.Instance.StartNewGame();
        }

        public void RunTimelineLevelWon()
        {
            _playableDirector.playableAsset = _timelineLevelWon;
            _playableDirector.Play();
        }

        public void RunTimelineLevelFailed()
        {
            _playableDirector.playableAsset = _timelineLevelFailed;
            _playableDirector.Play();
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            Instance = this;

            Debug.Assert(_startScreen);
            Debug.Assert(_difficultyButtonText);
            Debug.Assert(_exitButton);
            Debug.Assert(_generateButton);

            Debug.Assert(_levelEndStatusYou);
            Debug.Assert(_levelEndStatusWonSuccess);
            Debug.Assert(_levelEndStatusSheep);
            Debug.Assert(_levelEndStatusWonFailed);

            _playableDirector = GetComponent<PlayableDirector>();
            Debug.Assert(_playableDirector);

            Debug.Assert(_timelineLevelWon);
        }

        void Start()
        {
            #if !UNITY_STANDALONE

            _exitButton.gameObject.SetActive(false);
            _generateButton.gameObject.SetActive(false);

            #endif

            UpdateDifficultyText();
        }

        #endregion MonoBehaviour

        #region Private Methods

        private void UpdateDifficultyText()
        {
            if (Master.Instance.CurrentDifficulty == Difficulty.Easy)
            {
                _difficultyButtonText.text = "Lovely Sheep";
            }
            else if (Master.Instance.CurrentDifficulty == Difficulty.Normal)
            {
                _difficultyButtonText.text = "Untamed Sheep";
            }
            else if (Master.Instance.CurrentDifficulty == Difficulty.Hard)
            {
                _difficultyButtonText.text = "Barbaric Sheep";
            }
        }

        #endregion Private Methods
    }
}
