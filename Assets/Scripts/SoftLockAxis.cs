using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SoftLockAxis : MonoBehaviour, ISerializationCallbackReceiver
{
    public Rigidbody2D Rigidbody;
    public bool LockX;
    public bool LockY;
    public Vector2 TargetPosition;
    public Vector2 Feathering = new Vector2(4f, 4f);

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        var position = Rigidbody.position;
        if (LockX)
            position.x = Mathf.Lerp(position.x, TargetPosition.x, Feathering.x * Time.fixedDeltaTime);
        if (LockY)
            position.y = Mathf.Lerp(position.y, TargetPosition.y, Feathering.y * Time.fixedDeltaTime);
        Rigidbody.position = position;
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