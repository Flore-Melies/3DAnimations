using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float gravityScale;
    public float pullGravityMultiplier;
    public float jumpHeight;

    private float gravityY => Physics.gravity.y * gravityScale;
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
        // En multipliant un quaternion par une direction, on peut orienter un axe dans la direction voulue
        // Ici on modifie Vector3.forward (soit un vector3(0,0,1)) pour l’orienter selon la rotation de l’avatar
        var directionToMove = transform.rotation * Vector3.forward;
        return directionToMove * (speed * moveInputDirection.magnitude);
    }

    private Vector3 GetGravityMovement()
    {
        float yMovement;
        var isGrounded = controller.isGrounded;
        var startJumping = isJumpPressed && isGrounded;
        if (startJumping)
            yMovement = 0;
        else if (isGrounded)
            yMovement = gravityY * pullGravityMultiplier;
        else
            yMovement = lastGravityVelocity + gravityY * Time.deltaTime;
        return new Vector3(0, yMovement, 0);
    }

    private Vector3 GetJumpMovement()
    {
        if (!isJumpPressed || !controller.isGrounded)
            return Vector3.zero;
        return new Vector3
        {
            x = 0,
            y = Mathf.Sqrt(jumpHeight * -2 * gravityY),
            z = 0
        };
    }
}
