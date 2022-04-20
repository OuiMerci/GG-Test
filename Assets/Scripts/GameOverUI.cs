using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    #region Fields
    [SerializeField] Text _scoreLabel = null;
    [SerializeField] Text _highScoreLabel = null;
    [SerializeField] GameObject _newHighscore = null;
    #endregion Fields

    #region Private Methods
    private void Start()
    {
        GameManager.Instance.GameOverEvent += OnGameOver;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.GameOverEvent -= OnGameOver;
    }

    private void OnGameOver(int score, int highscore, bool newHighscore)
    {
        gameObject.SetActive(true);

        _scoreLabel.text = "Your score : " + score.ToString();
        _highScoreLabel.text = "Highscore : " + highscore.ToString();
        _newHighscore.SetActive(newHighscore);
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
