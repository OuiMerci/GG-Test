using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletsManager : MonoBehaviour {

    #region Fields
    [SerializeField] private  BulletDatabase _bulletDatabase = null;
    [SerializeField] private  int _baseBulletsCreatedAtStart = 0;
    [SerializeField] private  int _maxBulletsInstances = 0;
    [SerializeField] private Transform _pooledBulletsParent = null;
    [SerializeField] private Transform _activeBulletsParent = null;

    static private BulletsManager _instance = null;
    private Pool _bulletPool = null;
    public GameObject _minimalBulletPrefab = null;
    #endregion Fields

    #region Properties
    static public BulletsManager Instance
    {
        get { return _instance; }
    }
    #endregion Properties

    #region Private Methods
    // Use this for initialization
    private void Awake()
    {
        _instance = this;
        _bulletPool = new Pool(_minimalBulletPrefab, _baseBulletsCreatedAtStart, _maxBulletsInstances, _pooledBulletsParent, _activeBulletsParent);
	}
    #endregion Private Methods

    #region Public Methods
    public BulletBehaviour GetBullet(int bulletDatabaseId)
    {
        if(bulletDatabaseId == 0)
        {
            Debug.LogError("Bullet Manager : bullet with Database ID 0 is only for initialization and is not meant to be fired ! -> returning null.");
            return null;
        }

        BulletBehaviour newBullet = _bulletPool.GetFromQueue().GetComponent<BulletBehaviour>();
        BulletBehaviour prefabBullet = _bulletDatabase.GetObject(bulletDatabaseId);

        if (prefabBullet != null)
        {
            // Take values from the prefab bullet and give it to the bullet we're firing.
            prefabBullet.CopyValuesToNewBullet(newBullet);
            return newBullet;
        }
        else
        {
            // Instead of returning null, we could return the bullet as it come out of pool, but that probably isn't a good idea
            Debug.LogError("Bullet Manager : BulletType was not found in database ! -> returning null.");
            return null;
        }
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        _bulletPool.AddToPool(bullet);
    }
    #endregion Public Methods
}
