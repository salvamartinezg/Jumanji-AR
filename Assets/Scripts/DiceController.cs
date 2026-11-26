using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Reads the result of the physical die thrown by the player's hand.
// Also offers a virtual roll (keyboard) as a fallback.
public class DiceController : MonoBehaviour
{
    [Header("Die")]
    [Tooltip("Rigidbody of the die. If empty, the object called 'Die' is used.")]
    [SerializeField] private Rigidbody die;

    [Header("When is the die considered stopped?")]
    [SerializeField] private float settleSpeed = 0.05f;
    [SerializeField] private float settleSpin = 0.5f;
    [SerializeField] private float settleTime = 0.6f;

    [Header("Respawn")]
    [Tooltip("If the die falls this many meters below its start, it goes back to the tray.")]
    [SerializeField] private float fallLimit = 0.2f;
    [SerializeField] private float respawnLift = 0.03f;

    // Result of the last throw (0 = no result yet)
    public int ThrowResult { get; private set; }

    private XRGrabInteractable grab;
    private Vector3 homePosition;
    private bool armed = false;
    private bool touched = false;   // the player has picked up the die since ArmThrow
    private float stillTime = 0f;

    // Local direction of each face. Opposite faces add up to 7.
    private static readonly Vector3[] FaceDirections =
    {
        Vector3.up,       // 1
        Vector3.back,     // 2
        Vector3.right,    // 3
        Vector3.left,     // 4
        Vector3.forward,  // 5
        Vector3.down      // 6
    };

    private void Awake()
    {
        if (die == null)
        {
            GameObject dieObject = GameObject.Find("Die");
            if (dieObject != null)
                die = dieObject.GetComponent<Rigidbody>();
        }

        if (die == null)
        {
            Debug.LogWarning("DiceController: the die was not found. Assign it in the Inspector.");
            return;
        }

        grab = die.GetComponent<XRGrabInteractable>();
        homePosition = die.transform.position;
    }

    // Virtual roll (keyboard fallback)
    public int RollDice()
    {
        int diceResult = Random.Range(1, 7);

        Debug.Log("Dice result: " + diceResult);

        return diceResult;
    }

    // Starts listening for a throw: the die goes back to the tray
    public void ArmThrow()
    {
        ThrowResult = 0;
        armed = true;
        touched = false;
        stillTime = 0f;
        ResetDie();
    }

    public void Disarm()
    {
        armed = false;
    }

    private void Update()
    {
        if (!armed || die == null)
            return;

        // Fell off the table: back to the tray
        if (die.position.y < homePosition.y - fallLimit)
        {
            touched = false;
            stillTime = 0f;
            ResetDie();
            return;
        }

        // While the player holds the die, nothing to check
        if (grab != null && grab.isSelected)
        {
            touched = true;
            stillTime = 0f;
            return;
        }

        // Nobody has touched the die yet
        if (!touched)
            return;

        bool still = die.linearVelocity.magnitude < settleSpeed
                  && die.angularVelocity.magnitude < settleSpin;

        stillTime = still ? stillTime + Time.deltaTime : 0f;

        if (stillTime >= settleTime)
            FinishThrow();
    }

    private void FinishThrow()
    {
        int value = ReadTopFace(out float alignment);

        // Leaning on a wall or another object: throw again
        if (alignment < 0.8f)
        {
            Debug.Log("The die is leaning on something. Throw it again.");
            touched = false;
            stillTime = 0f;
            ResetDie();
            return;
        }

        ThrowResult = value;
        armed = false;
        Debug.Log("Dice result: " + value);
    }

    // The face pointing most upwards is the result
    private int ReadTopFace(out float alignment)
    {
        int best = 1;
        alignment = -1f;

        for (int i = 0; i < FaceDirections.Length; i++)
        {
            Vector3 worldDirection = die.transform.TransformDirection(FaceDirections[i]);
            float dot = Vector3.Dot(worldDirection, Vector3.up);

            if (dot > alignment)
            {
                alignment = dot;
                best = i + 1;
            }
        }

        return best;
    }

    // Puts the die back in the tray (unless the player is holding it)
    private void ResetDie()
    {
        if (die == null)
            return;

        if (grab != null && grab.isSelected)
            return;

        die.transform.SetPositionAndRotation(
            homePosition + Vector3.up * respawnLift,
            Random.rotation
        );
        die.linearVelocity = Vector3.zero;
        die.angularVelocity = Vector3.zero;
    }
}