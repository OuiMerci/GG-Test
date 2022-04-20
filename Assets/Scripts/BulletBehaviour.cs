using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletBehaviour : DatabaseObject
{
    #region Fields
    [SerializeField] private int _damage = 0;
    [SerializeField] private float _speed = 0.0f;

    private string _targetTag = "";
    private float _safeDistance = 0.0f;
    private BoxCollider2D _collider = null;
    private SpriteRenderer _renderer = null;

    #endregion Fields

    #region Properties
    public string TargetTag
    {
        get { return _targetTag; }
        set { _targetTag = value; }
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
        Move();
        CheckPosition();
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
        BulletsManager.Instance.ReturnBulletToPool(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == _targetTag)
        {
            other.GetComponent<Ship>().TakeDamage(_damage);
            ReturnToPool();
        }
    }
    #endregion Private Methods

    #region Public
    /// <summary>
    /// Called when a new bullet is got from the pool and initializes its fields;
    /// </summary>
    public void InitializeBullet(int damage, float speed, Sprite sprite, BoxCollider2D collider, string prefabName)
    {
        _damage = damage;
        _speed = speed;
        _renderer.sprite = sprite;
        _safeDistance = sprite.bounds.size.y + sprite.bounds.size.x;
        name = prefabName;

        // We resize a box collider. This means that all bullets must use the same kind of collider.
       _collider.offset = collider.offset;
       _collider.size = collider.size;

        /// Instead of resizing the collider, we could replace the current one by the one related to the bullet prefab we use.
        /// Pros : it allows using different colliders, and using the collider specifically tweaked for that bullet.
        /// Cons : On a big project, the cost of destroying a component and copying a new one at runtime (with reflexion) could cause performance issues.
        /// In the end, box collider can fit most kind of bullets almost perfectly and since it is quite common to have imprecise hitboxes I think the best choice is to use only BoxCollider2D
    }

    /// <summary>
    /// Called by the prefab bullets to share its informations with a bullet got from the pool;
    /// </summary>
    public void CopyValuesToNewBullet(BulletBehaviour newBullet)
    {
        // Prefabs linked to the manager don't go through Awake(), so the first time it is called, this values will be null.
        if(_collider == null)
        {
            _collider = GetComponent<BoxCollider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        newBullet.InitializeBullet(_damage, _speed, _renderer.sprite, _collider, name);
    }
    #endregion Public
}
