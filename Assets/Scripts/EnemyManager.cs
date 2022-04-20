using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    #region Events
    public delegate void EnemyDestroyed(int scoreValue);
    private EnemyDestroyed _enemyDestroyedEvent = null;
    #endregion Events

    #region Fields
    [SerializeField] private  EnemyDatabase _enemyDatabase = null;
    [SerializeField] private int _enemiesCreatedAtStart = 0;
    [SerializeField] private int _maxEnemyInstances = 0;
    [SerializeField] private Transform _pooledEnemiesParent = null;
    [SerializeField] private Transform _activeEnemiesParent = null;

    // The sound of the exploding ship. We could also choose to place it on the enemy prefab to have e specific sound per enemy.
    [SerializeField] private AudioClip _explosionSound = null;

    private AudioSource _audio = null;

    static private EnemyManager _instance = null;
    private Pool _enemyPool = null;
    public GameObject _minimalEnemyPrefab = null;
    #endregion Fields

    #region Properties
    static public EnemyManager Instance
    {
        get { return _instance; }
    }

    public event EnemyDestroyed EnemyDestroyedEvent
    {
        add { _enemyDestroyedEvent += value; }
        remove { _enemyDestroyedEvent -= value; }
    }
    #endregion Properties

    #region Private Methods
    // Use this for initialization
    private void Awake()
    {
        _instance = this;
        _audio = GetComponent<AudioSource>();
        _enemyPool = new Pool(_minimalEnemyPrefab, _enemiesCreatedAtStart, _maxEnemyInstances, _pooledEnemiesParent, _activeEnemiesParent);
	}

    private void PlayExplosionSound()
    {
        _audio.PlayOneShot(_explosionSound);
    }
    #endregion Private Methods

    #region Public Methods
    public EnemyBehaviour GetEnemy(int enemyDatabaseId)
    {
        if(enemyDatabaseId == 0)
        {
            Debug.LogError("Enemy Spawner : enemy with Database ID 0 is only for initialization and is not meant to be spawned ! -> returning null.");
            return null;
        }

        EnemyBehaviour newEnemy = _enemyPool.GetFromQueue().GetComponent<EnemyBehaviour>();
        EnemyBehaviour prefabEnemy = _enemyDatabase.GetObject(enemyDatabaseId);

        if (prefabEnemy != null)
        {
            // Take values from the prefab bullet and give it to the bullet we're firing.
            prefabEnemy.CopyValuesToNewBullet(newEnemy);
            return newEnemy;
        }
        else
        {
            // Instead of returning null, we could return the bullet as it come out of pool, but that probably isn't a good idea
            Debug.LogError("Enemy Manager : Enemy ID was not found in database ! -> returning null.");
            return null;
        }
    }

    public void ReturnEnemyToPool(GameObject enemy)
    {
        _enemyPool.AddToPool(enemy);
    }

    public void OnEnemyDestroyed(int scoreValue, GameObject enemy)
    {
        ReturnEnemyToPool(enemy);
        PlayExplosionSound();

        if(_enemyDestroyedEvent != null)
        {
            _enemyDestroyedEvent(scoreValue);
        }
    }
    #endregion Public Methods
}
