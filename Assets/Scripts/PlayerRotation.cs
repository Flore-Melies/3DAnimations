using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    public float turnTime;

    private Vector2 moveInputDirection;
    private Coroutine turnCoroutine;

    public void OnMove(InputAction.CallbackContext obj)
    {
        if(!obj.performed)
            return;
        moveInputDirection = obj.ReadValue<Vector2>();
        var targetLook = new Vector3(moveInputDirection.x, 0, moveInputDirection.y);
        if(turnCoroutine != null)
            StopCoroutine(turnCoroutine);
        turnCoroutine = StartCoroutine(TurnCoroutine(targetLook));
    }

    private IEnumerator TurnCoroutine(Vector3 targetLook)
    {
        var startTime = Time.time;
        var endTime = startTime + turnTime;
        var startRotation = transform.rotation;
        var endRotation = Quaternion.LookRotation(targetLook);
        while (Time.time < endTime)
        {
            var currentLerpTime = Time.time - startTime;
            var lerpFactor = currentLerpTime / turnTime;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpFactor);
            yield return null;
        }
    }
}
