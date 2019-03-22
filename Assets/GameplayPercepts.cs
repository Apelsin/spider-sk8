using UnityEngine;
using UnityEngine.UI;

public class GameplayPercepts : MonoBehaviour
{
    public Rigidbody2D Skateboard;
    public SpooderMotion SpooderMotion;
    public Text DebugText;
    public struct Readings
    {
        public float
            SB_Angle,
            SB_AngularVelocity,
            SB_WorldSpeedX,
            SB_WorldSpeedY,
            IsPhysicallySupported,
            IsGrinding,
            Input_Horizontal,
            Input_Vertical,
            Input_Jump;
    }

    public static Readings GetReadings(
        Rigidbody2D skateboard,
        SpooderMotion.Status status,
        IInput input)
    {
        return new Readings()
        {
            SB_Angle = skateboard.rotation,
            SB_AngularVelocity = skateboard.angularVelocity,
            SB_WorldSpeedX = skateboard.velocity.x,
            SB_WorldSpeedY = skateboard.velocity.y,
            IsPhysicallySupported = status.IsPhysicallySupported ? 1f : 0f,
            IsGrinding = 0f, // TODO
            Input_Horizontal = input.GetAxis("Horizontal"),
            Input_Vertical = input.GetAxis("Vertical"),
            Input_Jump = input.GetAxis("Jump")
        };
    }

    private void Update()
    {
        var readings = GetReadings(
            Skateboard,
            SpooderMotion.GetStatus(),
            SpooderMotion.Input);
        DebugText.text = readings.DebugProperties();
    }
}