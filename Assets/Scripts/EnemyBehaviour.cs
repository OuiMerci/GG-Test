using UnityEngine;
using System.Collections;
using System;

public class EnemyBehaviour : Ship
{
    #region Fields
    [SerializeField] private int _scoreValue = 0;
    private BoxCollider2D _collider = null;
    private float _safeDistance = 0.0f;
    #endregion Fields

    #region Properties
    public float SafeDistance
    {
        get { return _safeDistance; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update ()
    {
        CheckBasicAttackCooldown();
        Move();
        CheckPosition();
	}

    private void CheckBasicAttackCooldown()
    {
        if(_lastShootTime + _shootingDelay <= Time.time)
        {
            FireBasicAttack();
        }
    }

    private void Move()
    {
        Vector3 movement = _speed * Time.deltaTime * transform.up;
        transform.Translate(movement, Space.World);
    }

    private void CheckPosition()
    {
        if (GameCamera.IsOnScreen(transform.position, false, _safeDistance) == false)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        EnemyManager.Instance.ReturnEnemyToPool(gameObject);
    }
    #endregion Private Methods

    #region Public
    /// <summary>
    /// Called when a new bullet is got from the pool and initializes its fields;
    /// </summary>
    public void InitializeEnemy(int health, float speed, int currentShootingPatternId, int currentBasicAttackBulletsId, float shootingDelay, float bulletSpeed, int scoreValue, Sprite sprite, BoxCollider2D collider, string prefabName)
    {
        _health = health;
        _speed = speed;
        _currentShootingPatternId = currentShootingPatternId;
        _currentBasicAttackBulletsId = currentBasicAttackBulletsId;
        _shootingDelay = shootingDelay;
        _bulletSpeed = bulletSpeed;
        _scoreValue = scoreValue;
        _renderer.sprite = sprite;
        _safeDistance = sprite.bounds.size.y + sprite.bounds.size.x;
        name = prefabName;

        // We resize a box collider. This means that all bullets must use the same kind of collider.
       _collider.offset = collider.offset;
       _collider.size = collider.size;

        UpdateShootingTransforms();
    }

    /// <summary>
    /// Called by the prefab bullets to share its informations with a bullet got from the pool;
    /// </summary>
    public void CopyValuesToNewBullet(EnemyBehaviour newEnemy)
    {
        // Prefabs linked to the manager don't go through Awake(), so the first time it is called, this values will be null.
        if(_collider == null || _renderer == null)
        {
            _collider = GetComponent<BoxCollider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        newEnemy.InitializeEnemy(_health, _speed, _currentShootingPatternId, _currentBasicAttackBulletsId, _shootingDelay, _bulletSpeed, _scoreValue, _renderer.sprite, _collider, name);
    }
    #endregion Public

    #region Protected Methods
    protected override void HandleShipDestruction()
    {
        Debug.Log("Enemy destroyed !");
        EnemyManager.Instance.OnEnemyDestroyed(_scoreValue, gameObject);
    }
    #endregion Protected Methods

    #region Public Methods
    public override string GetTargetTag()
    {
        return "Player";
    }
    #endregion Public Methods
}
