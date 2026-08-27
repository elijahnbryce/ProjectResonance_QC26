using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float lifetime = 5f;

    protected Vector3 direction;

    public virtual void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;

        Destroy(gameObject, lifetime);

        Debug.Log($"{name}: Projectile initialized.");
    }

    public void IgnoreShooter(Collider shooterCollider)
    {
        Collider projectileCollider = GetComponent<Collider>();

        if (projectileCollider == null || shooterCollider == null)
        {
            return;
        }

        Physics.IgnoreCollision(
            projectileCollider,
            shooterCollider
        );

        Debug.Log($"{name}: Ignoring shooter collision.");
    }

    protected virtual void OnHit(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();

        if (entity == null)
        {
            return;
        }

        Debug.Log(
            $"{name}: Hit {other.name} for {damage} damage."
        );
        // later include entity.TakeDamage(damage);

        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }
}