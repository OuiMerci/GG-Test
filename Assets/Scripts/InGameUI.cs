using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUI : MonoBehaviour {

    #region Fields
    [SerializeField] Text _currentScoreLabel = null;
    [SerializeField] Text _highestScoreLabel = null;
    [SerializeField] Image _lifeBar = null;

    private int _currentScore = 0;
    private int _highestScore = 0;
    #endregion Fields

    #region Private Methods
    // Use this for initialization
    void Start ()
    {
        _highestScore = PlayerPrefs.GetInt("Highscore");
        _highestScoreLabel.text = "Highscore : " + _highestScore.ToString();
        UpdateDisplayedScore();
	}

    void OnEnable()
    {
        PlayerBehaviour.Instance.PlayerScoreUpdatedEvent += OnPlayerScoreUpdated;
        PlayerBehaviour.Instance.PlayerHealthUpdatedEvent += OnPlayerHealthUpdated;
    }

    void OnDisable()
    {
        PlayerBehaviour.Instance.PlayerScoreUpdatedEvent -= OnPlayerScoreUpdated;
        PlayerBehaviour.Instance.PlayerHealthUpdatedEvent -= OnPlayerHealthUpdated;
    }

    private void OnPlayerScoreUpdated(int playerScore)
    {
        _currentScore = playerScore;
        UpdateDisplayedScore();
    }

    private void OnPlayerHealthUpdated(int currentHealth, int maxHealth)
    {
        _lifeBar.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    private void UpdateDisplayedScore()
    {
        _currentScoreLabel.text = "SCORE : " + _currentScore.ToString();
    }
    #endregion Private Methods
}
