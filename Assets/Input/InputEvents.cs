using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Raises events for specific buttons based on their state by
/// reading from an <see cref="Input"/> object
/// </summary>
public class InputEvents : MonoBehaviour, ISerializationCallbackReceiver
{
    [Serializable]
    public class ButtonEvent : UnityEvent<string> { }

    [SerializeField]
    private MonoBehaviour _InputBehavior;

    public IInput Input
    {
        get { return (IInput)_InputBehavior; }
        set { _InputBehavior = (MonoBehaviour)value; }
    }

    [SerializeField]
    private List<string> _ButtonNames;

    public List<string> ButtonNames
    {
        get { return _ButtonNames; }
        protected set { _ButtonNames = value; }
    }

    [SerializeField]
    private ButtonEvent _ButtonDownEvent = new ButtonEvent();

    public event UnityAction<string> ButtonDown
    {
        add { _ButtonDownEvent.AddListener(value); }
        remove { _ButtonDownEvent.RemoveListener(value); }
    }

    private void Update()
    {
        if (Input != null)
        {
            foreach (var name in ButtonNames)
            {
                if (Input.GetButtonDown(name))
                    _ButtonDownEvent.Invoke(name);
            }
        }
    }

    public void OnBeforeSerialize()
    {
        // Round-trip property validation
        Input = Input;
    }

    public void OnAfterDeserialize()
    {
    }
}