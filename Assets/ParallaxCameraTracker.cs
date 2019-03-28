using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCameraTracker : MonoBehaviour
{
    public Vector3 CameraPosition; // Smoothed
    public Vector3 CameraTargetPosition; // Un-smoothed
    public AnimationCurve MobilityCurve = AnimationCurve.Constant(0f, 1f, 1f);
    private Vector3 _PreviousCamPos, _Previous_dCamPos;
    void Start()
    {
        _PreviousCamPos = CameraPosition;
    }

    // Update is called once per frame
    void Update()
    {
        var d_cam_pos = CameraPosition - _PreviousCamPos;
        var d2cam_pos = d_cam_pos - _Previous_dCamPos;
        // Magic
        var mobility_t = d_cam_pos.magnitude + d2cam_pos.magnitude;
        var mobility = MobilityCurve.Evaluate(mobility_t);

        transform.position = Vector3.Lerp(CameraTargetPosition, CameraPosition, mobility);
        _Previous_dCamPos = d_cam_pos;
        _PreviousCamPos = CameraPosition;
    }
}
