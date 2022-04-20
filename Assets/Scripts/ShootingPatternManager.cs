using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShootingPatternManager : MonoBehaviour {

    #region Fields
    [SerializeField] ShootingPatternDatabase _patternDatabase = null;

    static private ShootingPatternManager _instance = null;
    #endregion Fields

    #region Properties
    static public ShootingPatternManager Instance
    {
        get { return _instance; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake ()
    {
        _instance = this;
	}
    #endregion Private Methods

    #region Public Methods
    /// <summary>
    /// Returns a list of transforms related to the pattern passed for parameter. These transforms will be used to spawn bullets.
    /// </summary>
    public List<Transform> GetPatternTransforms(int ID)
    {
        ShootingPattern pattern;
        pattern = _patternDatabase.GetObject(ID);

        if(pattern != null)
        {
            return pattern.TransformsList;
        }
        else
        {
            Debug.LogError("Pattern Manager : Pattern ID was not found in database ! -> Returning 'null'.");
            return null;
        }
    }
    #endregion Public Methods
}
