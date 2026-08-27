using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePointHolder;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float deadZone = 0.2f;
    [SerializeField] private float rotationSpeed = 15f;

    private Vector2 lookInput;
    private Vector3 lastAimDirection = Vector3.back;

    public Vector3 AimDirection => lastAimDirection;


    private void Awake()
    {
        //Debug.Log($"{name} PlayerAim initialized.");
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

        //Debug.Log($"Look Input: {lookInput}");
    }


    private void Update()
    {
        if (lookInput.magnitude < deadZone)
            return;


        Vector3 aimDirection = new Vector3(
            lookInput.x,
            0f,
            lookInput.y
        );


        if (aimDirection.sqrMagnitude < 0.01f)
            return;


        lastAimDirection = aimDirection.normalized;


        Quaternion targetRotation = Quaternion.LookRotation(
            lastAimDirection,
            Vector3.up
        );


        // Rotate weapon orbit point
        firePointHolder.rotation = Quaternion.Slerp(
            firePointHolder.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );


        // Rotate player model
        playerVisual.rotation = Quaternion.Slerp(
            playerVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }


    public Transform GetFirePoint()
    {
        return firePoint;
    }
}

// ScriptRole: Handles player aiming and visual rotation.
// RelatedScripts: AbilityController, Projectile
// UsesSO: None
// ReceivesFrom: Unity Input System
// SendsTo: AbilityController