using UnityEngine;
using static Utilities;
public class MotionTester : MonoBehaviour
{
    private static float SquareWave(float t)
    {
        return Mathf.Sign(Mathf.Sin(2f * Mathf.PI * t));
    }
    private void Start()
    {
    }

    private void Update()
    {
        var pos = transform.localPosition;
        pos.x = SmoothFunc(SquareWave, 0.1f, 12)(Time.time);
        transform.localPosition = pos;
    }
}