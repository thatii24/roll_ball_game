using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Player 2's independently assigned arrow-key ball controls.</summary>
public class Player2Controller : MonoBehaviour
{
    [SerializeField, Min(0f)] private float acceleration = 24f;
    [SerializeField, Min(0f)] private float maxSpeed = 8.5f;
    [SerializeField, Min(0f)] private float rollingDrag = 1.25f;

    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        if (body == null)
            return;

        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.linearDamping = rollingDrag;
    }

    private void FixedUpdate()
    {
        if (PlayerController.PlayerOneActive || body == null || Keyboard.current == null)
            return;

        Vector2 input = Vector2.zero;
        if (Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
        if (Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
        if (Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
        if (Keyboard.current.upArrowKey.isPressed) input.y += 1f;

        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;
        body.AddForce(direction * acceleration, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            body.linearVelocity = new Vector3(horizontalVelocity.x, body.linearVelocity.y, horizontalVelocity.z);
        }
    }
}
