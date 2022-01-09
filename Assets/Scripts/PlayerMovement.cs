using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public bool isJumping => !controller.isGrounded;

    public float speed;
    public float pullGravityMultiplier;

    private CharacterController controller;
    private Vector2 moveInputDirection;
    private bool wasGroundedLastFrame;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext obj)
    {
        moveInputDirection = obj.ReadValue<Vector2>();
    }

    // Update is called once per frame
    private void Update()
    {
        wasGroundedLastFrame = controller.isGrounded;
        var moveDirection = GetLateralMovement() + GetGravityMovement();
        controller.Move(moveDirection * Time.deltaTime);
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
        if (controller.isGrounded)
            return Physics.gravity * pullGravityMultiplier;
        if (wasGroundedLastFrame && controller.velocity.y < 0)
            return Vector3.zero;
        return controller.velocity + Physics.gravity * Time.deltaTime;
    }
}
