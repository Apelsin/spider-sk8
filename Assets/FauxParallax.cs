using UnityEngine;

public class FauxParallax : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Transform of the Camera to move with respect to")]
    private Transform _CameraTransform;

    /// <summary>
    /// Transform of the <see cref="Camera"/> to move with respect to
    /// </summary>
    public Transform CameraTransform
    {
        get { return _CameraTransform; }
        set { _CameraTransform = value; }
    }

    [SerializeField]
    [Range(-10, 10f)]
    [Tooltip("Distance multiplier (positive = farther, negative = closer, zero = in-plane)")]
    private float _Distance;

    /// <summary>
    /// Multiplier on the distance measured from the Camera to this object's
    /// parent transform to position this object relative to its parent.
    /// </summary>
    /// <remarks>
    /// positive = farther
    /// negative = closer
    /// zero = in-plane
    /// </remarks>
    public float Distance
    {
        get { return _Distance; }
        set { _Distance = value; }
    }

    protected Transform GetCameraTrasform()
    {
        if (CameraTransform != null)
            return CameraTransform;
        return Camera.main.transform;
    }

    private void Start()
    {
    }

    private void Update()
    {
        var camera_xform = GetCameraTrasform();
        var parent = transform.parent;
        if (camera_xform != null && parent != null)
        {
            var offset = parent.position - camera_xform.position;
            var magnitude = Mathf.Pow(2f, -Distance) - 1f;
            var position = magnitude * offset;
            transform.localPosition = position;
            return;
        }

        if (camera_xform == null)
            Debug.LogWarning("No camera.");
        if (parent == null)
            Debug.LogWarning("Must be attached to a GameObject with a parented (non-root) Transform.");
    }
}