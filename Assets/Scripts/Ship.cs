using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Ship : DatabaseObject
{
    #region Fields
    [SerializeField] protected int _health = 0;
    [SerializeField] protected float _speed = 0.0f;
    [SerializeField] protected int _currentShootingPatternId = 0;
    [SerializeField] protected int _currentBasicAttackBulletsId = 0;
    [SerializeField] protected float _shootingDelay = 0.0f;
    [SerializeField] protected float _bulletSpeed = 0.0f;

    protected SpriteRenderer _renderer = null;
    protected int _maxHealth = 0;
    protected float _lastShootTime = 0.0f;
    private List<Transform> _shootingTransforms = null;
    #endregion Fields

    #region Private Methods
    virtual protected void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        UpdateShootingTransforms();
        _maxHealth = _health;

        // Make sure the ship can start shooting right away.
        _lastShootTime = -_shootingDelay;
    }

    // Called when we change our shooting pattern
    virtual protected void UpdateShootingTransforms()
    {
        _shootingTransforms = new List<Transform>();
        _shootingTransforms = ShootingPatternManager.Instance.GetPatternTransforms(_currentShootingPatternId);
    }

    protected Vector3 ApplyRotationAroundShip(Vector3 rotatingPoint)
    {
        float radRotation = transform.eulerAngles.z * Mathf.Deg2Rad;

        float x = Mathf.Cos(radRotation) * rotatingPoint.x - Mathf.Sin(radRotation) * rotatingPoint.y;
        float y = Mathf.Sin(radRotation) * rotatingPoint.x + Mathf.Cos(radRotation) * rotatingPoint.y;

        return new Vector3(x, y, rotatingPoint.z);
    }

    protected void FireBasicAttack()
    {
        _lastShootTime = Time.time;

        foreach (Transform t in _shootingTransforms)
        {
            BulletBehaviour bullet = BulletsManager.Instance.GetBullet(_currentBasicAttackBulletsId);
            bullet.transform.position = ApplyRotationAroundShip(t.localPosition) + transform.position;

            float rotZ = t.eulerAngles.z + transform.eulerAngles.z;
            bullet.transform.eulerAngles = new Vector3(t.eulerAngles.x, t.eulerAngles.y, rotZ);

            bullet.TargetTag = GetTargetTag();
        }
    }

    abstract protected void HandleShipDestruction();
    #endregion Private Methods

    #region Public Methods
    virtual public void TakeDamage(int damage)
    {
        _health -= damage;

         if(_health <= 0)
        {
            HandleShipDestruction();
        }
    }

    abstract public string GetTargetTag();
    #endregion Public Methods
}
