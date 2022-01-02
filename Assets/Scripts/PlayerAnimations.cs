using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimations : MonoBehaviour
{
    public float stickLerpTime;

    private Animator animator;
    private Vector2 computedStickDirection, realStickDirection;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        realStickDirection = value.Get<Vector2>();
        StopCoroutine(StickLerp());
        StartCoroutine(StickLerp());
    }

    private void Update()
    {
        animator.SetFloat("StickX", computedStickDirection.x);
        animator.SetFloat("StickY", computedStickDirection.y);
        animator.SetFloat("Speed", computedStickDirection.sqrMagnitude);
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
