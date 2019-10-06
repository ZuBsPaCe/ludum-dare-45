using UnityEngine;
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

        #endregion Serializable Fields

        #region Private Vars
        #endregion Private Vars

        #region Public Vars

        public static MainWindow Instance { get; private set; }

        #endregion Public Vars

        #region Public Methods

        public void SetStartScreenActive(bool active)
        {
            _startScreen.gameObject.SetActive(active);
        }

        public void StartGame()
        {
            Master.Instance.SetSceneType(SceneType.Game);
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
            Game.Instance.Generate();
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
