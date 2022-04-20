using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool
{
    #region Fields
    private Queue<GameObject> _objectQueue = null;
    private GameObject _prefab = null;
    private Transform _pooledObjectsParent = null;
    private Transform _releasedObjectsParent = null;
    private int _startCount = 0;
    private int _maxCount = 0;
    private int _currentCount = 0;
    #endregion Fields

    #region Private Methods
    private void InitializeQueue()
    {
        for(int i = 0; i < _startCount; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>
    /// If the limit is not reached, instantiate a new object for the pool and return true. Else, it doesn't instantiate anything and returns false.
    /// </summary>
    /// <returns></returns>
    private bool CreateNewObject()
    {
        if (_currentCount < _maxCount)
        {
            GameObject newObject = GameObject.Instantiate(_prefab, _pooledObjectsParent.position, _pooledObjectsParent.rotation) as GameObject;
            newObject.name = _prefab.name;
            AddToPool(newObject);
            _currentCount++;
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion Private Methods

    #region Public Methods
    public Pool(GameObject prefab, int startCount, int maxCount, Transform pooledObjectsParent, Transform releasedObjectsParent = null)
    {
        _objectQueue = new Queue<GameObject>();
        _prefab = prefab;
        _startCount = startCount;
        _maxCount = maxCount;
        _pooledObjectsParent = pooledObjectsParent;
        _releasedObjectsParent = releasedObjectsParent;

        InitializeQueue();
    }

    public void AddToPool(GameObject obj)
    {
        obj.transform.position = _pooledObjectsParent.transform.position;
        obj.transform.parent = _pooledObjectsParent;
        obj.SetActive(false);
        _objectQueue.Enqueue(obj);
    }

    public GameObject GetFromQueue()
    {
        GameObject newObject = null;
        if (_objectQueue.Count == 0)
        {
            bool objectCreated = CreateNewObject();

            if (objectCreated == false)
            {
                Debug.LogError("Pool : The pool of " + _prefab.name + " has no more objects to provide and has reached its maximum of intantiation.");
                return null;
            }
        }

        newObject = _objectQueue.Dequeue();
        newObject.transform.parent = _releasedObjectsParent;
        newObject.SetActive(true);

        return newObject;
    }
    #endregion Public Methods
}
