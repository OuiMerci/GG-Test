// This class is used mainly as a tool rather than a class that would interact with other objects, that is why I would rather use a static class than a singleton but this may not be the better solution.

using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {
    #region Fields
    static private float _maxX = 0.0f;
    static private float _minX = 0.0f;
    static private float _maxY = 0.0f;
    static private float _minY = 0.0f;
    #endregion Fields

    #region Properties
    static public float RightLimit { get { return _maxX; } }
    static public float LeftLimit { get { return _minX; } }
    static public float UpperLimit { get { return _maxY; } }
    static public float Bottom { get { return _minY; } }
    #endregion Properties

    #region Private Methods
    private void Awake()
    {
        InitializeScreenInfoValues();
    }

    private void InitializeScreenInfoValues()
    {
        // Camera's orthographic size is linked to the height, we multiply it by the screen ratio to get the width
        float screenRatio = (float)Screen.width / (float)Screen.height;

        // I assume the camera will stay as (0, 0), but it costs nothing to use Camera.main.transform.position for this one-time operation.
        // If for any reason the camera is subject to move during the game, this method should be called again.
        _maxX = Camera.main.transform.position.x + Camera.main.orthographicSize * screenRatio;
        _minX = Camera.main.transform.position.x - Camera.main.orthographicSize * screenRatio;
        _maxY = Camera.main.transform.position.y + Camera.main.orthographicSize;
        _minY = Camera.main.transform.position.y - Camera.main.orthographicSize;
    }
    #endregion Private Methods

    #region Public Methods
    /// <summary>
    /// Returns true if the position is inside the screen view.
    /// </summary>
    /// <param name="point">The position to check.</param>
    /// <param name="usePixelCoordinates">If true, uses Pixels coordinates, if false, uses world coordinates</param>
    /// <param name="safeDistance">Add a safe zone to make sure the whole object is outside of screen view.</param>
    /// <returns></returns>
    static public bool IsOnScreen(Vector2 point, bool usePixelCoordinates, float safeDistance = 0)
    {
        if(usePixelCoordinates)
        {
            return point.x > 0 - safeDistance && point.x < Screen.width + safeDistance
                && point.y > 0 - safeDistance && point.y < Screen.height + safeDistance;
        }
        else
        {
            return point.x > _minX - safeDistance && point.x < _maxX + safeDistance
                && point.y > _minY - safeDistance && point.y < _maxY + safeDistance;
        }
    }
    #endregion Public Methods
}
