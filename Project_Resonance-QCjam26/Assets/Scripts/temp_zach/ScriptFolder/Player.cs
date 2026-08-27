using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : Entity
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    private Rigidbody rb;

    private Vector2 moveInput;
    private float currentSpeed;

    public event Action<float> OnSpeedChanged;


    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();

        Debug.Log($"{name} initialized.");
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }


    private void FixedUpdate()
    {
        Vector2 normalizedInput = moveInput;

        // Prevent diagonal speed boost
        if (normalizedInput.sqrMagnitude > 1f)
            normalizedInput.Normalize();


        Vector3 movement = new Vector3(
            -normalizedInput.x,
            0f,
            -normalizedInput.y
        );


        float currentAcceleration = GetCurrentAcceleration();


        // Accelerate
        if (movement != Vector3.zero)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                MoveSpeed,
                currentAcceleration * Time.fixedDeltaTime
            );
        }
        // Decelerate
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                0f,
                deceleration * Time.fixedDeltaTime
            );
        }


        rb.MovePosition(
            rb.position +
            movement * currentSpeed * Time.fixedDeltaTime
        );


        // Update UI
        OnSpeedChanged?.Invoke(currentSpeed);
    }


    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }


    public float GetCurrentAcceleration()
    {
        return acceleration;
    }


    public override void Attack()
    {
        base.Attack();

        Debug.Log($"{name} Player Attacked.");
    }
}
