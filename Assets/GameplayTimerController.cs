using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayTimerController : MonoBehaviour
{
    [Serializable]
    public class TimerEvent : UnityEvent<float> { }

    [SerializeField]
    private float _InitialTime;

    public float InitialTime
    {
        get { return _InitialTime; }
        set { _InitialTime = value; }
    }


    [SerializeField]
    private TimerEvent _TimerUpdated;

    public event UnityAction<float> TimerUpdated
    {
        add { _TimerUpdated.AddListener(value); }
        remove { _TimerUpdated.RemoveListener(value); }
    }

    private float _Time;
    void Start()
    {
        
    }

    void Update()
    {
        _TimerUpdated.Invoke(_Time);
        _Time += Time.deltaTime;
    }
}
