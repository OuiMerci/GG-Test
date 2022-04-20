using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerBehaviour : Ship
{
    #region Events
    public delegate void PlayerScoreUpdatedHandler(int playerScore);
    private PlayerScoreUpdatedHandler _playerScoreUpdatedEvent = null;

    public delegate void PlayerHealthUpdatedHandler(int currentHealth, int maxHealth);
    private PlayerHealthUpdatedHandler _playerHealthUpdatedEvent = null;

    public delegate void PlayerDeadHandler(int score, int highscore, bool newHighscore);
    private PlayerDeadHandler _playerDeadEvent = null;
    #endregion Events

    #region Fields
    [SerializeField] private AudioClip _basicAttackSound = null;
    [SerializeField] private AudioClip _shipExplosionSound = null;

    static private PlayerBehaviour _instance = null;
    private AudioSource _audio = null;
    private int _score = 0;

    #region Screen Limits
    private float _baseZ = 0.0f;
    private float _maxX = 0.0f;
    private float _minX = 0.0f;
    private float _maxY = 0.0f;
    private float _minY = 0.0f;
    #endregion Screen Limits
    #endregion Fields

    #region properties
    static public PlayerBehaviour Instance
    {
        get { return _instance; }
    }

    public event PlayerScoreUpdatedHandler PlayerScoreUpdatedEvent
    {
        add { _playerScoreUpdatedEvent += value; }
        remove { _playerScoreUpdatedEvent -= value; }
    }

    public event PlayerHealthUpdatedHandler PlayerHealthUpdatedEvent
    {
        add { _playerHealthUpdatedEvent += value; }
        remove { _playerHealthUpdatedEvent -= value; }
    }

    public event PlayerDeadHandler PlayerDeadEvent
    {
        add { _playerDeadEvent += value; }
        remove { _playerDeadEvent -= value; }
    }
    #endregion properties

    #region Private Methods
    private void Awake()
    {
        _instance = this;
        _audio = GetComponent<AudioSource>();
    }

    override protected void Start ()
    {
        base.Start();
        InitializeMinMaxPositions();
    }

    private void OnEnable()
    {
        EnemyManager.Instance.EnemyDestroyedEvent += OnEnemyDestroyed;
    }

    private void OnDisable()
    {
        EnemyManager.Instance.EnemyDestroyedEvent -= OnEnemyDestroyed;
    }

    private void InitializeMinMaxPositions()
    {
        Sprite sprite = _renderer.sprite;
        // This is not a very expensive operation, but since it is called every time the player moves, I think it's nice to store the result as it won't change.
        _maxX = GameCamera.RightLimit - sprite.bounds.extents.x;
        _minX = GameCamera.LeftLimit + sprite.bounds.extents.x;
        _maxY = GameCamera.UpperLimit - sprite.bounds.extents.y;
        _minY = GameCamera.Bottom + sprite.bounds.extents.y;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        float x = Mathf.Clamp(position.x, _minX, _maxX);
        float y = Mathf.Clamp(position.y, _minY, _maxY);

        return new Vector3(x, y, _baseZ);
    }

    private void OnEnemyDestroyed(int scoreValue)
    {
        UpdateScore(scoreValue);
    }

    private void UpdateScore(int scoreValue)
    {
        _score += scoreValue;

        if(_playerScoreUpdatedEvent != null)
        {
            _playerScoreUpdatedEvent(_score);
        }

        Debug.Log("New player score : " + _score);
    }

    private bool CheckHighScore(out int highscore)
    {
        highscore = PlayerPrefs.GetInt("Highscore");

        bool newHighScore = false;
        if (_score > highscore)
        {
            PlayerPrefs.SetInt("Highscore", _score);
            highscore = _score;
            newHighScore = true;
        }

        return newHighScore;
    }
    #endregion Private Methods

    #region Protected Methods
    protected override void HandleShipDestruction()
    {
        // This method is not affected by the timescale
        AudioSource.PlayClipAtPoint(_shipExplosionSound, transform.position);

        int highscore = 0;
        bool newHighscore = false;

        newHighscore = CheckHighScore(out highscore);

        if(_playerDeadEvent != null)
        {
            _playerDeadEvent(_score, highscore, newHighscore);
        }

        gameObject.SetActive(false);
    }
    #endregion Protected Methods

    #region Public Methods
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (_playerHealthUpdatedEvent != null)
        {
            _playerHealthUpdatedEvent(_health, _maxHealth);
        }
    }

    public void Move(Vector3 movement)
    {
        Vector3 newPosition = transform.position + movement * _speed * Time.deltaTime;
        transform.position = ClampPosition(newPosition);
    }

    public void ApplyMousePosition(Vector3 mousePosition)
    {
        transform.position = ClampPosition(mousePosition);
    }

    // This version of FireBasicAttack is specific to the player. It is public, so it can be accessed from the input manager and handle the fire shooting delay as we want it for the player.
    new public void FireBasicAttack()
    {
        if(_lastShootTime + _shootingDelay > Time.time)
        {
            return;
        }

        base.FireBasicAttack();

        _audio.PlayOneShot(_basicAttackSound);
    }

    public override string GetTargetTag()
    {
        return "Enemy";
    }
    #endregion Public Methods
}