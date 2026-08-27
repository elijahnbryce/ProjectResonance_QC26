using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ArcProjectile : Projectile
{
    [Header("Launch")]
    [SerializeField] private float launchSpeed = 15f;
    [SerializeField] private float upwardForce = 8f;

    [Header("Gravity")]
    [SerializeField] private float gravity = 9.81f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = false;

        Debug.Log($"{name}: ArcProjectile Awake.");
        Debug.Log($"{name}: Gravity = {gravity}");
    }

    public override void Initialize(Vector3 shootDirection)
    {
        base.Initialize(shootDirection);

        Vector3 launchVelocity =
            direction.normalized * launchSpeed +
            Vector3.up * upwardForce;

        rb.linearVelocity = Vector3.zero;

        rb.AddForce(
            launchVelocity,
            ForceMode.VelocityChange
        );

        Debug.Log(
            $"{name}: LAUNCHED | " +
            $"Direction = {direction} | " +
            $"Velocity = {launchVelocity}"
        );
    }

    private void FixedUpdate()
    {
        rb.AddForce(
            Vector3.down * gravity,
            ForceMode.Acceleration
        );

       
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            $"{name}: COLLISION with {collision.gameObject.name}"
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            $"{name}: TRIGGER with {other.gameObject.name}"
        );
    }

    private void OnDestroy()
    {
        Debug.Log($"{name}: DESTROYED.");
    }
}