// I would prefer to use an interface for this, but I'm using a class to make serialization and edition easier
// I am currently using a int to identify different models of a same category : "playerBullet.id = 1 / enemyBullet.id = 2".
// But with more time, it would be nice to add a other layer so when choosing a model we can simply say "this enemy uses a plamaBullet" instead of "this enemy uses the bullet with databaseRelatedId = 13". 
using UnityEngine;
using System.Collections;

public class DatabaseObject : MonoBehaviour {

    #region Fields
    [SerializeField] private int _databaseRelatedId = 0;
    #endregion Fields

    #region Properties
    public int RelatedDatabaseID
    {
        get { return _databaseRelatedId; }
    }
    #endregion Properties
}
