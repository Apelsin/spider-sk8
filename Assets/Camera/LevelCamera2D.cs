using UnityEngine;
using UnityEngine.Events;
using static Utilities;
public class LevelCamera2D : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _Target;

    public Rigidbody2D Target
    {
        get { return _Target; }
        set
        {
            if (_Target == null && value != null)
                _TargetPosition = value.position;
            _Target = value;
        }
    }

    public RectTransform Constraints;

    [Range(0.1f, 10.0f)]
    public float Feathering = 1f;

    [Range(0f, 1f)]
    public float VelocityLookAheadScale = 0.5f;

    public AnimationCurve _VelocityLookAheadCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public Rect VelocityLookAhead = new Rect();

    [Range(0.1f, 10.0f)]
    public float LookAheadFeathering = 1f;

    public AnimationCurve FeatherCurve = AnimationCurve.Constant(0f, 1f, 1f);

    private Vector2 _FeatheredVelocity;
    private Vector3 _TargetPosition;

    public Vector3 TargetPosition
    {
        get { return _TargetPosition; }
    }

    [SerializeField]
    private Vector3Event _UpdateTargetPosition = new Vector3Event();
    public event UnityAction<Vector3> UpdateTargetPosition
    {
        add { _UpdateTargetPosition.AddListener(value); }
        remove { _UpdateTargetPosition.RemoveListener(value); }
    }

    private void Start()
    {
        if (Target != null)
            _TargetPosition = Target.position;
        else
            _TargetPosition = transform.position;
    }

    private static bool ConstrainVectorInRect(Vector2 v_in, Rect r, out Vector2 v_out)
    {
        v_out = Vector2.Min(Vector2.Max(r.min, v_in), r.max);
        return v_in == v_out;
    }

    private static bool ConstrainVectorInRectTransform(Vector3 v_in, RectTransform rt, out Vector3 v_out)
    {
        Vector2 relative_v_in = rt.InverseTransformPoint(v_in);
        var r = rt.rect;
        var is_in_rect = ConstrainVectorInRect(relative_v_in, r, out var relative_v_out);
        v_out = rt.TransformPoint(relative_v_out);
        return is_in_rect;
    }

    private void Update()
    {
        Vector2 target_pos, constrained_velocity;
        if (Target != null)
        {
            var velocity = VelocityLookAheadScale * Target.velocity;
            ConstrainVectorInRect(velocity, VelocityLookAhead, out constrained_velocity);
            constrained_velocity = Rect.PointToNormalized(VelocityLookAhead, constrained_velocity);
            constrained_velocity.x = _VelocityLookAheadCurve.Evaluate(constrained_velocity.x);
            constrained_velocity.y = _VelocityLookAheadCurve.Evaluate(constrained_velocity.y);
            constrained_velocity = Rect.NormalizedToPoint(VelocityLookAhead, constrained_velocity);
            target_pos = Target.position;
        }
        else
        {
            constrained_velocity = new Vector2();
            target_pos = _TargetPosition;
        }

        _FeatheredVelocity = Vector3.Lerp(_FeatheredVelocity, constrained_velocity, LookAheadFeathering * Time.deltaTime);
        if (Constraints != null)
            ConstrainVectorInRectTransform(target_pos + _FeatheredVelocity, Constraints, out _TargetPosition);
        else
            _TargetPosition = target_pos + _FeatheredVelocity;
        var here = transform.position;
        var distance = Vector3.Distance(here, _TargetPosition);
        var t = Feathering * FeatherCurve.Evaluate(distance);
        transform.position = Vector3.Lerp(here, _TargetPosition, t * Time.deltaTime);
        _UpdateTargetPosition.Invoke(_TargetPosition);
    }
}