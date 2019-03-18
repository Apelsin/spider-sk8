using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simulates input that can be read by a motion controller, etc.
/// </summary>
/// <remarks>
/// TODO: abstract to a base class and inherit
/// </remarks>
public class SimulatedInputDemo01 : MonoBehaviour, IInput
{
    private Dictionary<string, float> _Axes = new Dictionary<string, float>();
    private Dictionary<string, bool> _Button = new Dictionary<string, bool>();
    private Dictionary<string, bool> _PreviousButton = new Dictionary<string, bool>();
    private Dictionary<string, bool> _ButtonDownFlag = new Dictionary<string, bool>();
    private Dictionary<string, bool> _ButtonUpFlag = new Dictionary<string, bool>();

    public float GetAxis(string axis_name)
    {
        if (_Axes.TryGetValue(axis_name, out float value))
            return value;
        Debug.LogWarning($"No axis named \"{axis_name}\"");
        return 0f;
    }

    private bool GetButton(string button_name, out bool value, out bool previous)
    {
        if (_Button.TryGetValue(button_name, out value) &&
            _PreviousButton.TryGetValue(button_name, out previous))
            return true;
        Debug.LogWarning($"No button named \"{button_name}\"");
        value = false;
        previous = false;
        return false;
    }

    public bool GetButton(string button_name)
    {
        if (_Button.TryGetValue(button_name, out bool value))
            return value;
        Debug.LogWarning($"No button named \"{button_name}\"");
        return false;
    }

    public bool GetButtonDown(string button_name)
    {
        if (_ButtonDownFlag.TryGetValue(button_name, out bool value))
        {
            _ButtonDownFlag[button_name] = false;
            return value;
        }
        return false;
    }

    public bool GetButtonUp(string button_name)
    {
        if (_ButtonUpFlag.TryGetValue(button_name, out bool value))
        {
            _ButtonUpFlag[button_name] = false;
            return value;
        }
        return false;
    }

    /// <summary>
    /// Tracks and updates button up/down flags based on rising and falling edges of button press state
    /// </summary>
    private void ButtonUpdate()
    {
        foreach (var e in _Button)
        {
            var value = e.Value;
            if (_PreviousButton.TryGetValue(e.Key, out bool prev))
            {
                var up = !value && prev;
                var down = value && !prev;

                //Debug.Log($"{e.Key}.up: {up}\n{e.Key}.down: {down}");

                _ButtonUpFlag.TryGetValue(e.Key, out bool was_up);
                _ButtonUpFlag[e.Key] = was_up || up;

                _ButtonDownFlag.TryGetValue(e.Key, out bool was_down);
                _ButtonDownFlag[e.Key] = was_down || down;
            }
            _PreviousButton[e.Key] = value;
        }
    }

    private IEnumerator Start()
    {
        _Axes["Horizontal"] = 0f;
        _Axes["Vertical"] = 0f;
        _Button["Jump"] = false;
        ButtonUpdate();
        for (; ; )
        {
            _Button["Jump"] = false;
            _Axes["Horizontal"] = -1f;
            yield return new WaitForSeconds(1.5f);
            _Button["Jump"] = true;
            yield return new WaitForSeconds(0.5f);
            _Button["Jump"] = false;
            _Axes["Horizontal"] = 1f;
            yield return new WaitForSeconds(1.5f);
            _Button["Jump"] = true;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        ButtonUpdate();
    }
}