using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static Utilities;

[RequireComponent(typeof(Rigidbody2D))]
public class SkateboardJointController : MonoBehaviour, ISerializationCallbackReceiver
{
    [Serializable]
    public class FixedJointEvent : UnityEvent<FixedJoint2D> { }
    [SerializeField]
    private Rigidbody2D _Rigidbody;

    public Rigidbody2D Rigidbody
    {
        get { return _Rigidbody; }
        private set { _Rigidbody = value; }
    }

    [Range(0f, 5f)]
    public float RideCooldown = 1f;

    private FixedJoint2D _Joint;

    [MinMax(0f, 5000f)]
    [FormerlySerializedAs("BreakForce")]
    private Vector2 _BreakForce = new Vector2(4000f, 5000f);

    public Vector2 BreakForce
    {
        get { return _BreakForce; }
        set
        {
            _BreakForce = value;
            UpdateBreakForce();
        }
    }

    private float _BreakTime;

    [SerializeField]
    private FixedJointEvent _JointCreated;

    public event UnityAction<FixedJoint2D> JointCreated
    {
        add { _JointCreated.AddListener(value); }
        remove { _JointCreated.RemoveListener(value); }
    }

    private void UpdateBreakForce()
    {
        var t = Mathf.Cos(Rigidbody.rotation * Mathf.PI / 180f);
        _Joint.breakForce = Mathf.Lerp(BreakForce.x, BreakForce.y, t);
    }

    void Start()
    {
        _Joint = GetComponent<FixedJoint2D>();
    }

    void Update()
    {
        if (_Joint != null)
        {
            _BreakTime = Time.time + RideCooldown;
            UpdateBreakForce();
        }
    }

    void RideSkateboard(Rigidbody2D rigidbody)
    {
        var joint = gameObject.AddComponent<FixedJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector2.zero;
        rigidbody.position = Rigidbody.position;
        rigidbody.rotation = Rigidbody.rotation;
        joint.connectedBody = rigidbody;
        _Joint = joint;
        _JointCreated.Invoke(joint);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody != null)
        {
            if (collision.rigidbody.tag == "Skateboard")
            {
                if (_BreakTime < Time.time)
                {
                    RideSkateboard(collision.rigidbody);
                    _BreakTime = Time.time + RideCooldown;
                }
            }
        }
    }

    public void OnBeforeSerialize()
    {
        if (Rigidbody == null)
            Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnAfterDeserialize()
    {
    }
}
