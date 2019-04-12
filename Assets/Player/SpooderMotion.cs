using System;
using System.Collections.Generic;
using UnityEngine;

using static Utilities;

/// <summary>
/// Motion controller for protagonist game objects
/// Handles rigidbody motion by reading axis + button values from an input object
/// </summary>
/// <remarks>
/// TODO: abstract to a base class and inherit
/// </remarks>
[RequireComponent(typeof(Rigidbody2D))]
public class SpooderMotion : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private MonoBehaviour _InputBehavior;

    public IInput Input
    {
        get { return (IInput)_InputBehavior; }
        set { _InputBehavior = (MonoBehaviour)value; }
    }

    [SerializeField]
    private Rigidbody2D _Rigidbody;

    public Rigidbody2D Rigidbody
    {
        get { return _Rigidbody; }
        protected set { _Rigidbody = value; }
    }

    [SerializeField]
    private BoxCollider2D _PhysicalSupport;

    /// <summary>
    /// The trigger collider to sense whether the protagonist is
    /// physically supported (i.e. standing on a surface, grounded, etc)
    /// This is used primarily for jump logic
    /// </summary>
    public BoxCollider2D PhysicalSupport
    {
        get { return _PhysicalSupport; }
        protected set { _PhysicalSupport = value; }
    }

    [SerializeField]
    private LayerMask _PhysicalSupportLayerMask;

    public LayerMask PhysicalSupportLayerMask
    {
        get { return _PhysicalSupportLayerMask; }
        protected set { _PhysicalSupportLayerMask = value; }
    }

    [SerializeField]
    private FixedJoint2D _SkateboardJoint;

    public FixedJoint2D SkateboardJoint
    {
        get { return _SkateboardJoint; }
        set { _SkateboardJoint = value; }
    }

    [SerializeField]
    private Collider2D _BoardCollider;

    public Collider2D BoardCollider
    {
        get { return _BoardCollider; }
        set { _BoardCollider = value; }
    }

    [Serializable]
    public class Forces
    {
        /// <summary>
        /// Horizontal "running" force
        /// </summary>
        [Range(0.1f, 500f)]
        public float HorizontalForce = 24f;

        [Range(0.1f, 500f)]
        public float HorizontalForceInAir = 3f;

        /// <summary>
        /// Jump impulse velocity added to vertical speed
        /// when the protagonist jumps
        /// </summary>
        [Range(1f, 30f)]
        public float JumpStrength = 3.5f;

        [Range(0f, 8f)]
        public float HorizontalDrag = 0.9f;

        [Range(0f, 8f)]
        public float HorizontalDragInAir = 0.7f;
    }

    [SerializeField]
    private Forces _OnBoardForces;
    public Forces OnBoardForces { get { return _OnBoardForces; } }

    [SerializeField]
    private Forces _OffBoardForces;
    public Forces OffBoardForces {  get { return _OffBoardForces; } }

    public bool EnableMaxLean = true;

    [Range(0f, 180f)]
    public float MaxLean = 89f;

    public Vector2 ForceOffset = new Vector2();

    private bool _MaxLeanAirFlag = false;

    public bool MaxLeanAirFlag
    {
        get { return _MaxLeanAirFlag; }
        set { _MaxLeanAirFlag = value; }
    }

    [SerializeField]
    private Vector2 _OnBoardCenterOfMass;

    public Vector2 OnBoardCenterOfMass
    {
        get { return _OnBoardCenterOfMass; }
        set { _OnBoardCenterOfMass = value; }
    }

    [SerializeField]
    private Vector2 _OffBoardCenterOfMass;

    public Vector2 OffBoardCenterOfMass
    {
        get { return _OffBoardCenterOfMass; }
        set { _OffBoardCenterOfMass = value; }
    }



    #region Input Logic

    /// <summary>
    /// Logical values for handling input
    /// </summary>
    public struct InputLogic
    {
        public bool HasHorizontal;
        public float Horizontal;
        public bool Jump, JumpHold;
        public bool MoveDown;
    }

    public static InputLogic GetInputLogic(
        float horizontal,
        float vertical,
        bool jump,
        bool jump_hold)
    {
        return new InputLogic()
        {
            HasHorizontal = !Mathf.Approximately(0f, horizontal),
            Horizontal = horizontal,
            Jump = jump,
            JumpHold = jump_hold,
            MoveDown = vertical < 0f
        };
    }

    /// <summary>
    /// Gets the input logic by reading from an input object
    /// </summary>
    /// <param name="input">The input object to read from</param>
    /// <returns>Calculated input logic</returns>
    public static InputLogic GetInputLogic(IInput input)
    {
        return GetInputLogic(
            input.GetAxis("Horizontal"),
            input.GetAxis("Vertical"),
            input.GetButtonDown("Jump"),
            input.GetButton("Jump"));
    }

    #endregion Input Logic

    #region Status

    public struct PreStatus
    {
        public bool IsOnBoard;
    }

    public PreStatus GetPreStatus()
    {
        return new PreStatus
        {
            IsOnBoard = SkateboardJoint && SkateboardJoint.connectedBody
        };
    }

    /// <summary>
    /// Values indicating the current status of the protagonist,
    /// such as whether the protagonist is physically suppported, etc.
    /// </summary>
    public struct Status
    {
        public bool IsPhysicallySupported;
        public Vector2 CombinedCenterOfMass;
        public bool GrindReady;
        public bool IsOnBoard;
    }

    public Status GetStatus(PreStatus prestatus)
    {
        // Calculate whether the protagonist is currently physically
        // supported using a trigger collider overlap test
        var filter = new ContactFilter2D();
        filter.SetLayerMask(PhysicalSupportLayerMask);
        var results = new Collider2D[1];
        var number_of_physical_suports =
            PhysicalSupport.OverlapCollider(filter, results);

        Vector2 center_of_mass = new Vector2();
        float mass = 0;
        var bodies = new List<Rigidbody2D>
        {
            Rigidbody
        };
        if (prestatus.IsOnBoard)
            bodies.Add(SkateboardJoint.connectedBody);
        foreach (var rb in bodies)
        {
            center_of_mass += rb.worldCenterOfMass * rb.mass;
            mass += rb.mass;
        }
        center_of_mass /= mass;

        return new Status()
        {
            // If there is at least one physical support,
            // then the protagonist is physically supported
            IsPhysicallySupported = number_of_physical_suports > 0,
            CombinedCenterOfMass = center_of_mass,
            GrindReady = Rigidbody.velocity.y <= 0.01f,
        };
    }

    /// <summary>
    /// Gets the current status of the protagonist
    /// </summary>
    /// <returns>Status object</returns>
    public Status GetStatus()
    {
        return GetStatus(GetPreStatus());
    }

    #endregion Status

    private float _FreezeRotationTime = float.NegativeInfinity;

    private InputLogic _Logic;

    private void Update()
    {
        _Logic = GetInputLogic(Input);
    }

    /// <summary>
    /// Called once every physics update
    /// </summary>
    private void FixedUpdate()
    {
        var prestatus = GetPreStatus();
        Forces forces;
        if (prestatus.IsOnBoard)
        {
            Rigidbody.centerOfMass = OnBoardCenterOfMass;
            forces = OnBoardForces;
        }
        else
        {
            Rigidbody.centerOfMass = OffBoardCenterOfMass;
            forces = OffBoardForces;
        }
        var status = GetStatus(prestatus);
        if (Input != null)
        {
            // Get the rigidbody velocity
            var velocity = Rigidbody.velocity;
            var force_locus = status.CombinedCenterOfMass + (Vector2)transform.TransformVector(ForceOffset);
            if (_Logic.HasHorizontal)
            {
                float force_coef, h_drag_coef;
                Func<Vector3, Vector3> itf_func, tv_func;
                if (status.IsPhysicallySupported)
                {
                    force_coef = forces.HorizontalForce;
                    h_drag_coef = forces.HorizontalDrag;
                    itf_func = transform.InverseTransformVector;
                    tv_func = transform.TransformVector;
                }
                else
                {
                    force_coef = forces.HorizontalForceInAir;
                    h_drag_coef = forces.HorizontalDragInAir;
                    itf_func = x => x;
                    tv_func = x => x;
                }

                var h_force = new Vector3(force_coef * _Logic.Horizontal, 0f);

                var vel = Rigidbody.GetPointVelocity(force_locus);
                var rel_vel = itf_func(vel);
                var h_drag = new Vector3(Mathf.Abs(rel_vel.x) * rel_vel.x * h_drag_coef, 0f);

                var force = tv_func(h_force - h_drag);
                Rigidbody.AddForceAtPosition(force, force_locus, ForceMode2D.Force);
            }
            // Jumping is allowed if the protagonist is physically supported
            if (_Logic.Jump && status.IsPhysicallySupported)
            {
                //velocity.y += JumpStrength;
                var impulse_constant = 1f / Time.fixedDeltaTime;
                Vector3 jump_impulse = impulse_constant * new Vector3(0f, forces.JumpStrength);
                Rigidbody.AddForceAtPosition(jump_impulse, force_locus, ForceMode2D.Force);
                _Logic.Jump = false;
            }
            // Set the rigidbody velocity
            Rigidbody.velocity = velocity;

            // Cancel griding by moving down
            status.GrindReady &= !_Logic.MoveDown;
        }

        if (EnableMaxLean)
        {
            // Limit the lean angle
            var up = transform.up;
            var fwd = transform.forward;
            var angle = Vector3.SignedAngle(Vector3.up, up, fwd);
            var lim_func = Curry2x1(Mathf.Clamp, -MaxLean, MaxLean);
            var limited_angle = lim_func(angle);

            var angle_oob = angle != limited_angle;

            // Reset the flag if angle returns in-bounds
            MaxLeanAirFlag &= angle_oob;

            if (!MaxLeanAirFlag)
            {
                if (angle_oob)
                    _FreezeRotationTime = Time.fixedTime + Time.fixedDeltaTime;
                var frozen = Time.fixedTime < _FreezeRotationTime;
                Rigidbody.freezeRotation = frozen;
                var soft_limited_angle = SmoothFunc(lim_func, 10f, 3)(angle);
                //var soft_limited_angle = lim_func(angle);
                transform.rotation = Quaternion.AngleAxis(soft_limited_angle, fwd);
            }
        }
        else
        {
            Rigidbody.freezeRotation = false;
        }

        if (status.GrindReady)
            BoardCollider.gameObject.layer = LayerMask.NameToLayer("Board");
        else
            BoardCollider.gameObject.layer = LayerMask.NameToLayer("No Grind");

        MaxLeanAirFlag &= !status.IsPhysicallySupported;
    }

    public void OnBeforeSerialize()
    {
        // Round-trip property validation
        Input = Input;

        // Serialize required components on this GameObject
        if (Rigidbody == null)
            Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnAfterDeserialize()
    {
        // A vestigial organ. This method is rarely used and
        // the stub is required for ISerializationCallbackReceiver
    }
}