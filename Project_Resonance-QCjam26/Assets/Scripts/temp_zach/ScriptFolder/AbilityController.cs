using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class AbilityController : MonoBehaviour
{
    [Header("Abilities")]
    [SerializeField] private Ability shootAbility;
    [SerializeField] private Ability blastAbility;
    [SerializeField] private Ability lobAbility;


    private Dictionary<Ability, float> cooldownTimers
        = new Dictionary<Ability, float>();


    private PlayerAim playerAim;



    private void Start()
    {
        playerAim = GetComponent<PlayerAim>();

        InitializeAbility(shootAbility);
        InitializeAbility(blastAbility);
        InitializeAbility(lobAbility);
    }



    private void InitializeAbility(Ability ability)
    {
        if (ability == null)
            return;


        cooldownTimers.Add(
            ability,
            -999f
        );
    }



    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
            ActivateAbility(shootAbility);
        //input action to check beat
    }


    public void OnBlast(InputAction.CallbackContext context)
    {
        if (context.performed)
            ActivateAbility(blastAbility);
    }


    public void OnLob(InputAction.CallbackContext context)
    {
        if (context.performed)
            ActivateAbility(lobAbility);
    }



    private void ActivateAbility(Ability ability)
    {
        if (ability == null)
            return;


        if (!CanUse(ability))
        {
            Debug.Log(
                $"{ability.AbilityName} cooling down."
            );

            return;
        }


        Transform firePoint = playerAim.GetFirePoint();


        if (firePoint == null)
        {
            Debug.LogError("FirePoint missing.");
            return;
        }


        ability.SpawnProjectile(
            firePoint.position,
            firePoint.forward
        );


        cooldownTimers[ability] = Time.time;
    }



    private bool CanUse(Ability ability)
    {
        return Time.time >=
            cooldownTimers[ability] + ability.Cooldown;
    }
}