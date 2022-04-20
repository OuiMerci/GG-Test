using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ScrollingBackground : MonoBehaviour
{
    #region Fields
    // SpeedX isn't currently used, but could be used for another kind of effect. 
    [SerializeField] private float _speedX = 0.0f;
    [SerializeField] private float _speedY = 0.0f;

    // We add this value to make sur the horizontal scale fits the screen width
    private const float HORIZONTAL_SCALE_SECURITY = 1;
    private Material _mat = null;
    #endregion Fields

    #region Private Methods
    private void Start () {
        _mat = GetComponent<Renderer>().material;
        InitializeScale();
    }
	
	private void Update () {
        UpdateScrolling();
	}

    /// <summary>
    /// Initializes the background's scale to make sure it fits the screen.
    /// </summary>
    private void InitializeScale()
    {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float textureRatio = (float)_mat.mainTexture.width / (float)_mat.mainTexture.height;
        float cameraHeight = Camera.main.orthographicSize * 2;

        // Fit horizontally
        float newXScale = cameraHeight * screenRatio + HORIZONTAL_SCALE_SECURITY;

        // Apply width/height ratio to vertical scale
        float newScaleY = newXScale / textureRatio;

        // Set scale
        transform.localScale = new Vector3(newXScale, newScaleY, transform.localScale.z);
    }

    private void UpdateScrolling()
    {
        Vector2 newOffset = _mat.mainTextureOffset;
        newOffset.x += _speedX * Time.deltaTime;
        newOffset.y += _speedY * Time.deltaTime;
        _mat.SetTextureOffset("_MainTex", newOffset);
    }

    #endregion Private Methods
}
