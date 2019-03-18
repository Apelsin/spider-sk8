using UnityEngine;

/// <summary>
/// Input class that reads from the player's input by
/// the static UnityEngine.<see cref="Input"/> class.
/// </summary>
public class PlayerInput : MonoBehaviour, IInput
{
    private void Start()
    {
    }

    public float GetAxis(string axis_name)
    {
        if (!isActiveAndEnabled)
            return default;
        // Use static Input class by default
        return Input.GetAxis(axis_name);
    }

    public bool GetButton(string button_name)
    {
        if (!isActiveAndEnabled)
            return default;
        return Input.GetButton(button_name);
    }

    public bool GetButtonDown(string button_name)
    {
        if (!isActiveAndEnabled)
            return default;
        return Input.GetButtonDown(button_name);
    }

    public bool GetButtonUp(string button_name)
    {
        if (!isActiveAndEnabled)
            return default;
        return Input.GetButtonUp(button_name);
    }
}