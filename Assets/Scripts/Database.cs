using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Database<T> : MonoBehaviour where T : class
{
    #region Fields
    [SerializeField] protected List<DatabaseObject> _objects = null;

    protected Dictionary<int, T> _objectDictionnary = null;
    #endregion Fields

    #region Private Methods
    private void InitializePatternsDictionnary()
    {
        _objectDictionnary = new Dictionary<int, T>();

        for (int i = 0; i < _objects.Count; i++)
        {
            T component = _objects[i].GetComponent<T>();

            if (_objectDictionnary.ContainsKey(_objects[i].RelatedDatabaseID) == false)
            {
                _objectDictionnary.Add(_objects[i].RelatedDatabaseID, component);
            }
            else
            {
                Debug.LogWarning("Database : The same id is found multiple times !");
            }
        }
    }
    #endregion Private Methods
    virtual protected void Awake()
    {
        InitializePatternsDictionnary();
    }
    #region Protected Methods

    #endregion Protected Methods

    #region Public Methods
    /// <summary>
    /// Returns the object stored with the specified ID;
    /// </summary>
    /// <param name="id">ID of the object</param>
    /// <returns></returns>
    public T GetObject(int id)
    {
        T storedObject;
        if (_objectDictionnary.TryGetValue(id, out storedObject))
        {
            return storedObject;
        }
        else
        {
            Debug.LogError("Database : Object of type '" + typeof(T) +"' with ID '" + id + "' was not found in dictionnary ! -> Returning 'null'.");
            return null;
        }
    }
    #endregion Public Methods*/
}
