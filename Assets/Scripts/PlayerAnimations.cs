using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimations : MonoBehaviour
{
    public float stickLerpTime;
    public float aimLerpTime;

    private Coroutine stickLerpCoroutine;
    private Coroutine aimLerpCoroutine;
    private Animator animator;
    private Vector2 computedStickDirection;
    private Vector2 realStickDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext obj)
    {
        if(obj.started)
            return;
        realStickDirection = obj.ReadValue<Vector2>();
        if(stickLerpCoroutine != null)
            StopCoroutine(stickLerpCoroutine);
        stickLerpCoroutine = StartCoroutine(StickLerp());
    }

    public void OnAim(InputAction.CallbackContext obj)
    {
        if(obj.started)
            return;
        if(aimLerpCoroutine != null)
            StopCoroutine(aimLerpCoroutine);
        aimLerpCoroutine = StartCoroutine(AimLerp(obj.performed));
    }

    private void Update()
    {
        animator.SetFloat("StickX", computedStickDirection.x);
        animator.SetFloat("StickY", computedStickDirection.y);
        animator.SetFloat("Speed", computedStickDirection.sqrMagnitude);
    }

    private IEnumerator AimLerp(bool isAiming)
    {
        var targetValue = isAiming ? 1f : 0f;
        var startValue = animator.GetLayerWeight(1);
        var startTime = Time.time;
        var endTime = startTime + aimLerpTime;
        while (Time.time < endTime)
        {
            var currentLerpTime = Time.time - startTime;
            var lerpFactor = currentLerpTime / aimLerpTime;
            var lerp = Mathf.Lerp(startValue, targetValue, lerpFactor);
            animator.SetLayerWeight(1, lerp);
            yield return null;
        }
    }

    private IEnumerator StickLerp()
    {
        if (stickLerpTime <= 0)
            computedStickDirection = realStickDirection;
        var startTime = Time.time;
        var endTime = startTime + stickLerpTime;
        var startStick = computedStickDirection;
        while (Time.time < endTime)
        {
            var currentLerpTime = Time.time - startTime;
            var lerpFactor = currentLerpTime / stickLerpTime;
            computedStickDirection = Vector2.Lerp(startStick, realStickDirection, lerpFactor);
            yield return null;
        }
    }
}
