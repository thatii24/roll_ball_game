using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Player 1's ball controller and the shared player-selection UI.</summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float acceleration = 24f;
    [SerializeField, Min(0f)] private float maxSpeed = 8.5f;
    [SerializeField, Min(0f)] private float rollingDrag = 1.25f;
    [SerializeField] private float fallRespawnHeight = -4f;

    private Rigidbody body;
    private Vector3 respawnPosition;
    private bool hasSecondPlayer;
    private static bool playerOneActive = true;

    public static bool PlayerOneActive => playerOneActive;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        respawnPosition = transform.position;
        playerOneActive = true;
        hasSecondPlayer = GameObject.Find("Player2") != null;
        ConfigureBody();
    }

    private void Update()
    {
        if (hasSecondPlayer && Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            SetActivePlayer(!playerOneActive);
    }

    private void ConfigureBody()
    {
        if (body == null)
            return;

        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.linearDamping = rollingDrag;
    }

    private void FixedUpdate()
    {
        if (body == null)
            return;

        if (body.position.y < fallRespawnHeight)
        {
            body.position = respawnPosition;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.WakeUp();
            return;
        }

        if (!playerOneActive || Keyboard.current == null)
            return;

        Vector2 input = Vector2.zero;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.wKey.isPressed) input.y += 1f;

        ApplyRollingInput(input);
    }

    private void ApplyRollingInput(Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;
        body.AddForce(direction * acceleration, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            body.linearVelocity = new Vector3(horizontalVelocity.x, body.linearVelocity.y, horizontalVelocity.z);
        }
    }

    private void OnGUI()
    {
        if (!hasSecondPlayer)
            return;

        const int panelWidth = 250;
        const int panelHeight = 76;
        GUI.Box(new Rect(16f, 16f, panelWidth, panelHeight), "BALL CONTROL");
        GUI.Label(new Rect(30f, 39f, panelWidth - 28f, 20f),
            "Active: Player " + (playerOneActive ? "1 (WASD)" : "2 (WASD)"));

        string nextPlayer = playerOneActive ? "Player 2" : "Player 1";
        if (GUI.Button(new Rect(30f, 59f, panelWidth - 28f, 26f), "Switch to " + nextPlayer + "  [Tab]"))
            SetActivePlayer(!playerOneActive);

    }

    public static void SetActivePlayer(bool playerOne)
    {
        playerOneActive = playerOne;
    }
}
