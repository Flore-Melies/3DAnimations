using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float pullGravityMultiplier;
    public float jumpHeight;

    private CharacterController controller;
    private Vector2 moveInputDirection;
    private bool isJumpPressed;
    private Vector3 lastVelocity;
    private bool wasGroundedLastFrame;

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

    // Update is called once per frame
    private void Update()
    {
        var moveDirection = GetLateralMovement() + GetGravityMovement() + GetJumpMovement();
        wasGroundedLastFrame = controller.isGrounded;
        controller.Move(moveDirection * Time.deltaTime);
        lastVelocity = controller.velocity;
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
        var startFalling = wasGroundedLastFrame && !isGrounded;
        Debug.Log(startFalling);
        if (controller.isGrounded && !isJumpPressed)
            yMovement = Physics.gravity.y * pullGravityMultiplier;
        else if (startJumping)
            yMovement = 0;

        else if (startFalling)
            yMovement = Physics.gravity.y * Time.deltaTime;
        else
            yMovement = lastVelocity.y + Physics.gravity.y * Time.deltaTime;
        return new Vector3(0, yMovement, 0);
    }

    private Vector3 GetJumpMovement()
    {
        if (!isJumpPressed || !controller.isGrounded)
            return Vector3.zero;
        return new Vector3
        {
            x = 0,
            y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y),
            z = 0
        };
    }

    private void LateUpdate()
    {
    }
}
