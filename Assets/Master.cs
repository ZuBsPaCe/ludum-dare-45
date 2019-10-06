using System;
using System.Linq.Expressions;
using UnityEngine;
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
        private Difficulty _initialDifficulty = Difficulty.Normal;

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
                else if (_currentSceneType == SceneType.Game)
                {
                    SetSceneType(SceneType.StartScreen);
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
                    Game.Instance.Generate();
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
