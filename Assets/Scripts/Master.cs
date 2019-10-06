using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;
using zs.Assets.Scripts;

namespace zs.Assets
{
    public class Master : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private SceneType _initialSceneType = SceneType.StartScreen;

        [SerializeField]
        private int _initialSeed = 0;

        [SerializeField]
        private int _initialLevel = 0;

        [SerializeField]
        private int _initialPlayerCount = 0;

        
        #endregion Serializable Fields

        #region Private Vars

        private SceneType _currentSceneType = SceneType.StartScreen;
        private Difficulty _currentDifficulty;
        private bool _escapeButtonHandled = false;

        #endregion Private Vars

        #region Public Vars

        public static Master Instance { get; private set; }

        public Difficulty CurrentDifficulty
        {
            get { return _currentDifficulty; }
            set
            {
                _currentDifficulty = value;
                PlayerPrefs.SetInt("Difficulty", (int) _currentDifficulty);
                PlayerPrefs.Save();
            }
        }

        public int CurrentLevel { get; private set; }
        public int CurrentPlayerCount { get; private set; }
        public int CurrentPitchForkCount { get; private set; }

        public int TotalScore { get; set; }

        #endregion Public Vars

        #region Public Methods

        public void SetSceneType(SceneType sceneType)
        {
            if (_currentSceneType == sceneType)
            {
                return;
            }

            _currentSceneType = sceneType;

            SceneManager.LoadScene("MainScene");
        }

        public void StartNewGame()
        {
            TotalScore = 0;
            CurrentPlayerCount = _initialPlayerCount;
            CurrentLevel = _initialLevel;
            CurrentPitchForkCount = 0;

            SetSceneType(SceneType.Game);
        }

        public void StartNextLevel()
        {
            CurrentLevel += 1;
            Master.Instance.CurrentPlayerCount = Game.Instance.AlivePlayerCount + 1;

            CurrentPitchForkCount = 0;
            foreach (Player player in Game.Instance.Players)
            {
                if (player.IsAlive && player.HasPitchfork)
                {
                    CurrentPitchForkCount += 1;
                }
            }

            SceneManager.LoadScene("MainScene");
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            if (Instance == null)
            {
                Debug.Log("Master Awake");

                DontDestroyOnLoad(gameObject);

                SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;

                _currentSceneType = _initialSceneType;

                Instance = this;

                CurrentDifficulty = (Difficulty) PlayerPrefs.GetInt("Difficulty", (int) Difficulty.Normal);

                SetSceneType(_currentSceneType);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        void Start()
        {
        }

        void Update()
        {
            #if UNITY_STANDALONE

            if (Input.GetKeyDown(KeyCode.Escape) && !_escapeButtonHandled)
            {
                _escapeButtonHandled = true;

                if (_currentSceneType == SceneType.StartScreen)
                {
                    Application.Quit();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                _escapeButtonHandled = false;
            }

            #endif
        }

        private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.Log($"Scene Loaded: [{_currentSceneType}]");

            MainWindow.Instance.SetStartScreenActive(false);

            switch (_currentSceneType)
            {
                case SceneType.StartScreen:
                    MainWindow.Instance.SetStartScreenActive(true);
                    break;

                case SceneType.Game:
                    int initialSeed = _initialSeed;

                    if (initialSeed == 0)
                    {
                        initialSeed = Guid.NewGuid().GetHashCode();
                    }

                    Game.Instance.Generate(initialSeed, CurrentLevel, CurrentPlayerCount, CurrentDifficulty);
                    break;

                default:
                    Debug.Assert(false, "Unknown SceneType");
                    break;
            }
        }

        #endregion MonoBehaviour

        #region Private Methods
        #endregion Private Methods
    }
}
