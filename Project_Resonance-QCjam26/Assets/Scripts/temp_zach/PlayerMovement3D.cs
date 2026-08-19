using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement3D : MonoBehaviour
{
    public enum ControlScheme { WASD, ArrowKeys }  //set control scheme uniquely per player character

    [Header("Controls")]
    [SerializeField] private ControlScheme controls = ControlScheme.WASD;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    private InputAction moveAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");

        var composite = moveAction.AddCompositeBinding("2DVector");

        if (controls == ControlScheme.WASD)
        {
            composite.With("Up", "<Keyboard>/w")
                     .With("Down", "<Keyboard>/s")
                     .With("Left", "<Keyboard>/a")
                     .With("Right", "<Keyboard>/d");
        }
        else // ArrowKeys
        {
            composite.With("Up", "<Keyboard>/upArrow")
                     .With("Down", "<Keyboard>/downArrow")
                     .With("Left", "<Keyboard>/leftArrow")
                     .With("Right", "<Keyboard>/rightArrow");
        }
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void OnDestroy()
    {
        moveAction.Dispose();
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

 
        Vector3 horizontalMove = new Vector3(input.x, 0f, input.y) * moveSpeed;

        // Simple gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; 
        }
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = horizontalMove;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}