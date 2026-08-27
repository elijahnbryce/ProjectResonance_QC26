using UnityEngine;

[CreateAssetMenu(
    fileName = "New Ability",
    menuName = "Abilities/Ability"
)]
public class Ability : ScriptableObject
{
    [Header("Ability Info")]
    [SerializeField] private string abilityName;


    [Header("Weapon Settings")]
    [SerializeField] private float cooldown;


    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;


    public string AbilityName => abilityName;

    public float Cooldown => cooldown;

    public Projectile ProjectilePrefab => projectilePrefab;



    public void SpawnProjectile(Vector3 position, Vector3 direction)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{abilityName} has no projectile assigned.");
            return;
        }


        Projectile projectile = Instantiate(
            projectilePrefab,
            position,
            Quaternion.LookRotation(direction)
        );


        projectile.Initialize(direction);


        Debug.Log($"Ability Fired: {abilityName}");
    }
}