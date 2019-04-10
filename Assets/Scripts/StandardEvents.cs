using UnityEngine;
using UnityEngine.Events;

public class StandardEvents : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _OnEnable = new UnityEvent();

    public UnityEvent OnEnableEvent => _OnEnable;

    private void OnEnable()
    {
        OnEnableEvent.Invoke();
    }

    [SerializeField]
    private UnityEvent _OnDisable = new UnityEvent();

    public UnityEvent OnDisableEvent => _OnDisable;

    private void OnDisable()
    {
        OnDisableEvent.Invoke();
    }

    [SerializeField]
    private UnityEvent _Awake = new UnityEvent();

    public UnityEvent AwakeEvent => _Awake;

    private void Awake()
    {
        AwakeEvent.Invoke();
    }

    [SerializeField]
    private UnityEvent _Start = new UnityEvent();

    public UnityEvent StartEvent => _Start;

    private void Start()
    {
        StartEvent.Invoke();
    }
}