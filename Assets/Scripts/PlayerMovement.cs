using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float gravityScale;
    public float pullGravityMultiplier;
    public float jumpHeight;

    private float gravityY => Physics.gravity.y * gravityScale;
    private bool startJumping => isJumpPressed && controller.isGrounded;

    private CharacterController controller;
    private Vector2 moveInputDirection;
    private bool isJumpPressed;
    private float lastGravityVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext obj)
    {
        moveInputDirection = obj.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext obj)
    {
        isJumpPressed = obj.ReadValueAsButton();
    }

    private void Update()
    {
        var moveDirection = GetLateralMovement() + GetGravityMovement() + GetJumpMovement();
        controller.Move(moveDirection * Time.deltaTime);
        lastGravityVelocity = controller.velocity.y;
    }

    private Vector3 GetLateralMovement()
    {
        if (moveInputDirection.sqrMagnitude == 0)
            return Vector3.zero;
        var directionToMove = new Vector3(moveInputDirection.x, 0, moveInputDirection.y);
        return directionToMove * (speed * moveInputDirection.magnitude);
    }

    private Vector3 GetGravityMovement()
    {
        float yMovement;
        if (startJumping)
            yMovement = 0;
        else if (controller.isGrounded)
            yMovement = gravityY * pullGravityMultiplier;
        else
            yMovement = lastGravityVelocity + gravityY * Time.deltaTime;
        return new Vector3(0, yMovement, 0);
    }

    private Vector3 GetJumpMovement()
    {
        if (!startJumping)
            return Vector3.zero;
        var yMovement = Mathf.Sqrt(jumpHeight * -2 * gravityY);
        return new Vector3(0, yMovement, 0);
    }
}
