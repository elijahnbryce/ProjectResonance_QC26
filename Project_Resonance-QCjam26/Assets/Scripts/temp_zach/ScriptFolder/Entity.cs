using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int moveSpeed = 5;
    [SerializeField] protected float attackCooldown = 1f;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected string deathAnimation = "Death";
    

    protected int currentHealth;
    public event Action<int, int> OnHealthChanged;

    protected float lastAttackTime;

   
    
    protected virtual void Awake()
    {
        currentHealth = maxHealth;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        Debug.Log($"{name} initialized.");
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"{name} took {damage} damage.");

        if (IsDead()) //if health is lower than 0, die
        {
            OnDeath();
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    protected virtual void OnDeath()
    {
        Debug.Log($"{name} died.");

        if (animator != null)
        {
            animator.Play(deathAnimation);
        }
    }

    public virtual void Attack() //if attack time is more than cooldown (more time has past than cooldown time), attack and reset attack time
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;

        Debug.Log($"{name} attacked.");
    }

    public int MoveSpeed => moveSpeed;
}