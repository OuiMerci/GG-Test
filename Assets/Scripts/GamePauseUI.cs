using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GamePauseUI : MonoBehaviour
{
    #region Fields
    // for some reason, there doesn't seem to be a way to get the list of toggles with the group.
    [SerializeField] private Transform _controllerTogglesParent = null;
    private Toggle[] _controllerToggles = null;
    #endregion Fields

    #region Private Methods
    private void Start()
    {
        GameManager.Instance.GamePausedEvent += OnGamePaused;
        GameManager.Instance.GameUnpausedEvent += OnGameUnpaused;

        _controllerToggles = GetComponentsInChildren<Toggle>();
        CheckToggles(_controllerToggles);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.GamePausedEvent -= OnGamePaused;
        GameManager.Instance.GameUnpausedEvent -= OnGameUnpaused;
    }

    private void OnGamePaused()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// This makes sure that the checked trigger is the right one.
    /// </summary>
    /// <param name="toggles">List of controllers toggles</param>
    private void CheckToggles(Toggle[] toggles)
    {
        InputManager.ControllerType controller = (InputManager.ControllerType)PlayerPrefs.GetInt("Controller");
        string wantedToggleName = controller.ToString() + "Toggle";

        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].name == wantedToggleName)
            {
                toggles[i].isOn = true;
            }
        }
    }

    private void OnGameUnpaused()
    {
        gameObject.SetActive(false);
    }
    #endregion Private Methods

    #region Public Methods
    public void OnMouseControllerToggleClicked()
    {
        InputManager.Instance.SwitchController(InputManager.ControllerType.Mouse);
    }

    public void OnKeyboardControllerToggleClicked()
    {
        InputManager.Instance.SwitchController(InputManager.ControllerType.Keyboard);
    }
    #endregion Public Methods
}
