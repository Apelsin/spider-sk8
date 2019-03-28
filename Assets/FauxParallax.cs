using UnityEngine;
using UnityEngine.UI;

public class FauxParallax : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private Transform _Camera;

    public Transform Camera
    {
        get { return _Camera; }
        set { _Camera = value; }
    }

    [SerializeField]
    private Transform _ParallaxReference;

    public Transform ParallaxReference
    {
        get { return _ParallaxReference; }
        set { _ParallaxReference = value; }
    }

    protected virtual Vector3 ParallaxReferencePoint
    {
        get
        {
            if (_ParallaxReference != null)
                return _ParallaxReference.position;
            return Vector3.zero;
        }
    }

    [SerializeField]
    [Range(-10, 10f)]
    private float _Distance;

    public float Distance
    {
        get { return _Distance; }
        set { _Distance = value; }
    }

    [SerializeField]
    private Vector2 _DistanceScale = Vector2.one;
    public Vector2 DistanceScale
    {
        get { return _DistanceScale; }
        set { _DistanceScale = value; }
    }

    [SerializeField]
    [Range(0f, 1f)]
    private float _Smoothing;

    public float Smoothing
    {
        get { return _Smoothing; }
        set { _Smoothing = value; }
    }

    protected Transform GetCamera()
    {
        if (Camera != null)
            return Camera;
        return UnityEngine.Camera.main.transform;
    }

    private void Start()
    {
    }

    private void Update()
    {
        var camera = GetCamera();
        if (camera != null)
        {
            var offset = ParallaxReferencePoint - camera.position;
            var magnitude = Mathf.Pow(2f, -Distance) - 1f;
            var position = magnitude * offset;
            position.Scale(_DistanceScale);
            transform.localPosition = position;
        }
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
    }
}