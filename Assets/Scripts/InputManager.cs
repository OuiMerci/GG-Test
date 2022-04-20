// TODO : réorganiser les fonctions et les switchs

using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    #region Enums
    public enum ControllerType
    {
        Mouse,
        Keyboard
    }
    #endregion Enums

    #region Fields
    private const int MAX_RAYCAST_DISTANCE = 25;
    private static InputManager _instance = null;
    private GameManager _gameManager = null;
    private PlayerBehaviour _player = null;
    private ControllerType _controller = ControllerType.Mouse;
    private int _backgroundLayerMask = 0;
    #endregion Fields

    #region Properties
    static public InputManager Instance
    {
        get { return _instance; }
    }

    public ControllerType Controller
    {
        get { return _controller; }
    }
    #endregion Properties

    #region Private Methods
    private void OnEnable()
    {
        GameManager.Instance.GamePausedEvent += OnGamePaused;
        GameManager.Instance.GameUnpausedEvent += OnGameUnpaused;
        GameManager.Instance.GameOverEvent += OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.Instance.GamePausedEvent -= OnGamePaused;
        GameManager.Instance.GameUnpausedEvent -= OnGameUnpaused;
        GameManager.Instance.GameOverEvent -= OnGameOver;
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start ()
    {
        _gameManager = GameManager.Instance;
        _player = PlayerBehaviour.Instance;

        // Let's hide the mouse's cursor
        Cursor.visible = false;

        //If there is no "Controller" key inside the player prefs, the result will be 0 and will keep the _controller set on Mouse.
        _controller = (ControllerType)PlayerPrefs.GetInt("Controller");

        // We use the background for raycasting when following the mouse's movement
        _backgroundLayerMask = 1 << LayerMask.NameToLayer("Background");
	}
	
	private void Update ()
    {
        switch(_gameManager.State)
        {
            case GameManager.GameState.Playing:
                CheckShipInput();
                break;
            case GameManager.GameState.Paused:
                CheckGamePaused();
                break;
        }
	}

    private void CheckShipInput()
    {
        string basicAttackButton = "";
        string pauseButton = "";

        switch (_controller)
        {
            case ControllerType.Mouse:
                FollowMouseMovement();
                basicAttackButton = "BasicAttackMouse";
                pauseButton = "Pause";
                break;

            case ControllerType.Keyboard:
                GetKeyboardMovementInput();
                basicAttackButton = "BasicAttackKeyboard";
                pauseButton = "Pause";
                break;

            default:
                Debug.LogError("Input Manager : The controller '" + _controller + "' is not implemented !");
                break;
        }

        CheckBasicFireInput(basicAttackButton);
        CheckPauseButton(pauseButton);
    }

    private void CheckGamePaused()
    {
        string pauseButton = "";

        switch (_controller)
        {
            case ControllerType.Mouse:
                pauseButton = "Pause";
                break;

            case ControllerType.Keyboard:
                pauseButton = "Pause";
                break;

            default:
                Debug.LogError("Input Manager : The controller '" + _controller + "' is not implemented !");
                break;
        }

        CheckPauseButton(pauseButton);
    }

    private void FollowMouseMovement()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, MAX_RAYCAST_DISTANCE, _backgroundLayerMask))
        {
            _player.ApplyMousePosition(hit.point);
        }
    }

    private void GetKeyboardMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        _player.Move(new Vector3(horizontalInput, verticalInput, 0));
    }

    private void CheckBasicFireInput(string basicAttackButton)
    {
        if (Input.GetButton(basicAttackButton))
        {
            _player.FireBasicAttack();
        }
    }

    private void CheckPauseButton(string pauseButton)
    {
        // We use the same key for keyboard and mouse controller. But we might use another if we add a gamepad controller;
        if(Input.GetButtonDown(pauseButton))
        {
            GameManager.Instance.ToggleGamePause();
        }
    }

    private void OnGamePaused()
    {
        Cursor.visible = true;
    }

    private void OnGameUnpaused()
    {
        Cursor.visible = false;
    }

    private void OnGameOver(int score, int highscore, bool newHighScore)
    {
        Cursor.visible = true;
    }
    #endregion Private Methods

    #region Public Methods
    public void SwitchController(ControllerType type)
    {
        _controller = type;

        PlayerPrefs.SetInt("Controller", (int)_controller);
    }
    #endregion Public Methods
}
