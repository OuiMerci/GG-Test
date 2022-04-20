using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShootingPattern : DatabaseObject
{
    #region Fields
    private List<Transform> _transformsList = null;
    #endregion Fields

    #region Properties
    public List<Transform> TransformsList
    {
        get { return _transformsList; }
    }
    #endregion Properties

    #region Private Methods
    private void Awake ()
    {
        InitializeTransformsList();
	}

    /// <summary>
    /// Draws arrows to display children's position and orientation in editor. This is meant to simplify the creation of patterns.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (Selection.activeGameObject != transform.gameObject)
        {
            return;
        }

        // Draw a guizmo for each launcher
        Gizmos.color = Color.red;

        foreach (Transform child in transform)
        {
            Gizmos.DrawSphere(child.position, 0.05f);
            DrawArrow(child.position, child.up, Color.red);
        }
    }

    // I copied and modified adapted this method from this post : http://forum.unity3d.com/threads/debug-drawarrow.85980/
    public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
        Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

        Gizmos.color = color;
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, up * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, down * arrowHeadLength);
    }
    #endregion Private Methods

    #region Public Methods
    public void InitializeTransformsList()
    {
        // I planned to use an array and GetComponentsInChildren, but it also returned this object's transform.
        _transformsList = new List<Transform>();

        foreach (Transform child in transform)
        {
            _transformsList.Add(child);
        }

        if (_transformsList.Count < 1)
        {
            Debug.LogWarning("ShootingPattern : No child found in pattern " + name + " ! Please add at least 1 child transform");
        }
    }
    #endregion Public Methods
}
