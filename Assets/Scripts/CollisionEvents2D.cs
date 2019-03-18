using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Raises events invoked by the 2D physics system
/// </summary>
public class CollisionEvents2D : MonoBehaviour
{
    [Serializable]
    public class CollisionEvent2D : UnityEvent<Collision2D> { }

    [Serializable]
    public class ColliderEvent2D : UnityEvent<Collider2D> { }

    [SerializeField]
    private CollisionEvent2D _CollisionEnter = new CollisionEvent2D();

    [SerializeField]
    private CollisionEvent2D _CollisionExit = new CollisionEvent2D();

    [SerializeField]
    private CollisionEvent2D _CollisionStay = new CollisionEvent2D();

    [SerializeField]
    private ColliderEvent2D _TriggerEnter = new ColliderEvent2D();

    [SerializeField]
    private ColliderEvent2D _TriggerExit = new ColliderEvent2D();

    [SerializeField]
    private ColliderEvent2D _TriggerStay = new ColliderEvent2D();

    public event UnityAction<Collision2D> CollisionEnter
    {
        add { _CollisionEnter.AddListener(value); }
        remove { _CollisionEnter.RemoveListener(value); }
    }

    public event UnityAction<Collision2D> CollisionExit
    {
        add { _CollisionExit.AddListener(value); }
        remove { _CollisionExit.RemoveListener(value); }
    }

    public event UnityAction<Collision2D> CollisionStay
    {
        add { _CollisionStay.AddListener(value); }
        remove { _CollisionStay.RemoveListener(value); }
    }

    public event UnityAction<Collider2D> TriggerEnter
    {
        add { _TriggerEnter.AddListener(value); }
        remove { _TriggerEnter.RemoveListener(value); }
    }

    public event UnityAction<Collider2D> TriggerExit
    {
        add { _TriggerExit.AddListener(value); }
        remove { _TriggerExit.RemoveListener(value); }
    }

    public event UnityAction<Collider2D> TriggerStay
    {
        add { _TriggerStay.AddListener(value); }
        remove { _TriggerStay.RemoveListener(value); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _CollisionEnter.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _CollisionExit.Invoke(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _CollisionStay.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        _TriggerEnter.Invoke(collider);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        _TriggerExit.Invoke(collider);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        _TriggerStay.Invoke(collider);
    }
}