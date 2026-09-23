using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Follows the currently selected ball and provides smooth mouse-wheel zoom.</summary>
[RequireComponent(typeof(Camera))]
public sealed class BallCameraController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float followSmoothTime = 0.22f;
    [SerializeField, Min(0f)] private float rotationSharpness = 8f;
    [SerializeField, Min(0f)] private float zoomStep = 1.5f;
    [SerializeField, Min(0f)] private float zoomSmoothTime = 0.18f;
    [SerializeField, Min(0.1f)] private float minimumDistance = 5f;
    [SerializeField, Min(0.1f)] private float maximumDistance = 22f;
    [SerializeField, Min(0f)] private float focusHeight = 0.45f;

    private Transform playerOne;
    private Transform playerTwo;
    private Transform currentTarget;
    private Vector3 followDirection;
    private Vector3 followVelocity;
    private float currentDistance;
    private float targetDistance;
    private float zoomVelocity;

    private void Awake()
    {
        GameObject firstPlayer = GameObject.Find("Player");
        GameObject secondPlayer = GameObject.Find("Player2");
        playerOne = firstPlayer != null ? firstPlayer.transform : null;
        playerTwo = secondPlayer != null ? secondPlayer.transform : null;
    }

    private void Start()
    {
        currentTarget = PlayerController.PlayerOneActive || playerTwo == null ? playerOne : playerTwo;
        if (currentTarget == null)
            return;

        Vector3 offset = transform.position - currentTarget.position;
        currentDistance = Mathf.Clamp(offset.magnitude, minimumDistance, maximumDistance);
        targetDistance = currentDistance;
        followDirection = offset.sqrMagnitude > 0.001f ? offset.normalized : new Vector3(0f, 0.3f, -1f).normalized;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        targetDistance = Mathf.Clamp(
            targetDistance - Mathf.Sign(scroll) * zoomStep,
            minimumDistance,
            maximumDistance);
    }

    private void LateUpdate()
    {
        Transform selectedTarget = PlayerController.PlayerOneActive || playerTwo == null ? playerOne : playerTwo;
        if (selectedTarget == null)
            return;

        currentTarget = selectedTarget;
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref zoomVelocity, zoomSmoothTime);

        Vector3 focusPoint = currentTarget.position + Vector3.up * focusHeight;
        Vector3 desiredPosition = focusPoint + followDirection * currentDistance;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, followSmoothTime);

        Vector3 lookDirection = focusPoint - transform.position;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            float rotationBlend = 1f - Mathf.Exp(-rotationSharpness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationBlend);
        }
    }
}
