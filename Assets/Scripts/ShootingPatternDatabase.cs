using UnityEngine;
using System.Collections;

public class ShootingPatternDatabase : Database<ShootingPattern>
{
    override protected void Awake()
    {
        base.Awake();

        foreach (ShootingPattern patt in _objectDictionnary.Values)
        {
            patt.InitializeTransformsList();
        }
    }
}
