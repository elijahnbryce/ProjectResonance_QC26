using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HitscanProjectile : Projectile
{
    [Header("Hitscan Settings")]
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask hitLayers;

    [Header("Laser Visual")]
    [SerializeField] private float laserLifetime = 0.1f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        Debug.Log($"{name}: HitscanProjectile Awake.");
    }

    public override void Initialize(Vector3 shootDirection)
    {
        base.Initialize(shootDirection);

        Fire();
    }

    private void Fire()
    {
        Debug.Log("Hitscan fired.");

        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + direction.normalized * range;

        RaycastHit hit;

        if (Physics.Raycast(
            startPoint,
            direction.normalized,
            out hit,
            range,
            hitLayers))
        {
            Debug.Log($"Hitscan hit: {hit.collider.name}");

           
            endPoint = hit.point;

            OnHit(hit.collider);
        }
        else
        {
            Debug.Log("Hitscan missed.");
        }

        
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        
        Destroy(gameObject, laserLifetime);
    }
}
