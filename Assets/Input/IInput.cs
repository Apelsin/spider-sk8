/// <summary>
/// Interface for input sources
/// </summary>
public interface IInput
{
    float GetAxis(string axis_name);

    bool GetButton(string button_name);

    bool GetButtonDown(string button_name);

    bool GetButtonUp(string button_name);
}