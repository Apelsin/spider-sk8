using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class PPCanvas : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField, HideInInspector]
    private Canvas _Canvas;

    public Canvas Canvas
    {
        get { return _Canvas; }
    }

    [SerializeField, HideInInspector]
    private CanvasScaler _CanvasScaler;

    public CanvasScaler CanvasScaler
    {
        get { return _CanvasScaler; }
    }

    private void LateUpdate()
    {
        var camera = Canvas.worldCamera;
        if (camera == null)
            camera = Camera.main;
        var pp_cam = camera.GetComponent<PixelPerfectCamera>();
        if (pp_cam)
            CanvasScaler.scaleFactor = pp_cam.pixelRatio;
    }

    public void OnBeforeSerialize()
    {
        _Canvas = GetComponent<Canvas>();
        _CanvasScaler = GetComponent<CanvasScaler>();
    }

    public void OnAfterDeserialize()
    {
    }
}