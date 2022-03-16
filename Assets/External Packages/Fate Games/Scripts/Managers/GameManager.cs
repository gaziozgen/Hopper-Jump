using UnityEngine;
using System.IO;

namespace FateGames
{
    public class GameManager : MonoBehaviour
    {
        #region Properties
        private LevelManager levelManager;
        [SerializeField] private ControlType controlType;

        [SerializeField] private bool showLevelText = true;

        [HideInInspector] public LevelManager LevelManager { get => levelManager; }
        #endregion

        private void Initialize()
        {
            if (!AvoidDuplication()) return;
            PlayerProgression.InitializePlayerData();
            AnalyticsManager.Initialize();
            FacebookManager.Initialize(SceneManager.LoadCurrentLevel);
        }

        public void OnLevelWasLoaded(int level)
        {
            levelManager = FindObjectOfType<LevelManager>();
            GlobalEventDispatcher.Emit("UPDATE_MONEY");
        }

        #region Unity Callbacks

        private void Awake()
        {
            Initialize();
        }
        private void Update()
        {
            if (State != GameState.LOADING_SCREEN)
                CheckInput();
        }

        #endregion

        #region Singleton
        private static GameManager instance;
        public static GameManager Instance { get => instance; }


        private bool AvoidDuplication()
        {
            if (!instance)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
                return true;
            }
            else
                DestroyImmediate(gameObject);
            return false;
        }

        #endregion

        private void CheckInput()
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.X) && State == GameState.IN_GAME)
                    SceneManager.FinishLevel(true);
                else if (Input.GetKeyDown(KeyCode.C) && State == GameState.IN_GAME)
                    SceneManager.FinishLevel(false);
            }
            if (Input.GetMouseButtonDown(0) && State == GameState.START_SCREEN)
                SceneManager.StartLevel();
        }

        #region State Management
        private GameState state = GameState.LOADING_SCREEN;
        public GameState State { get => state; }
        public bool ShowLevelText { get => showLevelText; }
        public ControlType ControlType { get => controlType; }

        public void UpdateGameState(GameState newState)
        {
            state = newState;
        }
        #endregion

    }
    public enum GameState { LOADING_SCREEN, START_SCREEN, IN_GAME, PAUSE_SCREEN, FINISHED, COMPLETE_SCREEN }
    public enum ControlType { JOYSTICK, SWIPE }
}