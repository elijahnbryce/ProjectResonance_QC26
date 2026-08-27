using UnityEngine;

public class StraightProjectile : Projectile
{
    [SerializeField] private float speed = 20f;


    private void Update()
    {
        transform.position +=
            direction * speed * Time.deltaTime;
    }


    public override void Initialize(Vector3 shootDirection)
    {
        base.Initialize(shootDirection);

        Debug.Log("Straight projectile initialized.");
    }
}