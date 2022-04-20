using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region Enums
    public enum GameState
    {
        StartMenu,
        Playing,
        Paused,
        GameOver
    }
    #endregion Enums

    #region Events
    public delegate void PauseStartedHandler();
    private PauseStartedHandler _gamePausedEvent = null;

    public delegate void PauseOverHandler();
    private PauseOverHandler _gameUnpausedEvent = null;

    public delegate void GameOverHandler(int score, int highscore, bool newHighscore);
    private GameOverHandler _gameOverEvent = null;
    #endregion Events

    #region Fields
    static private GameManager _instance = null;
    private GameState _gameState = GameState.Playing;
    private EnemyManager _enemyManager = null;
    #endregion Fields

    #region Properties
    static public GameManager Instance
    {
        get { return _instance; }
    }

    public GameState State
    {
        get { return _gameState; }
    }

    public event PauseStartedHandler GamePausedEvent
    {
        add { _gamePausedEvent += value; }
        remove { _gamePausedEvent -= value; }
    }

    public event PauseOverHandler GameUnpausedEvent
    {
        add { _gameUnpausedEvent += value; }
        remove { _gameUnpausedEvent -= value; }
    }

    public event GameOverHandler GameOverEvent
    {
        add { _gameOverEvent += value; }
        remove { _gameOverEvent -= value; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake ()
    {
        _instance = this;
        _gameState = GameState.Playing;
	}

    private void OnEnable()
    {
        PlayerBehaviour.Instance.PlayerDeadEvent += OnPlayerDead;
    }

    private void OnDisable()
    {
        PlayerBehaviour.Instance.PlayerDeadEvent -= OnPlayerDead;
    }

    private void ChangeGameState(GameState state)
    {
        _gameState = state;
    }

    private void OnPlayerDead(int score, int highscore, bool newHighscore)
    {
        _gameState = GameState.GameOver;
        Time.timeScale = 0;

        if(_gameOverEvent != null)
        {
            _gameOverEvent(score, highscore, newHighscore);
        }
    }
    #endregion Private Methods

    #region public Methods
    public void ToggleGamePause()
    {
        if(_gameState == GameState.Paused)
        {
            ChangeGameState(GameState.Playing);
            Time.timeScale = 1;

            if(_gameUnpausedEvent != null)
            {
                _gameUnpausedEvent();
            }
        }
        else
        {
            ChangeGameState(GameState.Paused);

            // If we want to add a time related feature during the pause (like an animation), it may be better to create a custom time value;
            Time.timeScale = 0;

            if (_gamePausedEvent != null)
            {
                _gamePausedEvent();
            }
        }
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion public Methods

}
