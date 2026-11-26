using System.Collections;
using UnityEngine;

// Visual events of the board: spikes of the trap, chest and crystals of the treasures,
// and the obstacle of the challenge.
public class BoardEffects : MonoBehaviour
{
    [Header("Trap")]
    [SerializeField] private Transform[] spikes;
    [SerializeField] private float spikeRiseTime = 0.25f;
    [SerializeField] private float spikeStayTime = 1.2f;

    [Header("Treasure: chest")]
    [SerializeField] private Transform chestLid;
    [Tooltip("If the lid opens the wrong way, change the sign (100 / -100).")]
    [SerializeField] private float lidOpenAngle = 100f;
    [SerializeField] private float lidOpenTime = 0.6f;

    [Header("Treasure: crystals")]
    [SerializeField] private Transform[] crystals;
    [SerializeField] private float crystalTime = 1.2f;
    [SerializeField] private float crystalLift = 0.015f;

    [Header("Challenge: obstacle")]
    [SerializeField] private Transform obstacle;
    [SerializeField] private float obstacleTime = 0.35f;

    private Vector3[] spikeStartPositions;
    private float spikeHeight;

    private Vector3 lidStartPosition;
    private Quaternion lidStartRotation;

    private Vector3[] crystalStartPositions;
    private Quaternion[] crystalStartRotations;

    private Vector3 obstacleFullScale;
    private float obstacleHalfHeight;

    private void Awake()
    {
        // Spikes: remember where they stand
        int spikeCount = spikes != null ? spikes.Length : 0;
        spikeStartPositions = new Vector3[spikeCount];
        for (int i = 0; i < spikeCount; i++)
        {
            spikeStartPositions[i] = spikes[i].position;

            Renderer spikeRenderer = spikes[i].GetComponent<Renderer>();
            if (spikeRenderer != null)
                spikeHeight = Mathf.Max(spikeHeight, spikeRenderer.bounds.size.y);
        }

        // Chest lid: remember its closed pose
        if (chestLid != null)
        {
            lidStartPosition = chestLid.position;
            lidStartRotation = chestLid.rotation;
        }

        // Crystals: remember their pose
        int crystalCount = crystals != null ? crystals.Length : 0;
        crystalStartPositions = new Vector3[crystalCount];
        crystalStartRotations = new Quaternion[crystalCount];
        for (int i = 0; i < crystalCount; i++)
        {
            crystalStartPositions[i] = crystals[i].position;
            crystalStartRotations[i] = crystals[i].rotation;
        }

        // Obstacle: remember its size and hide it
        if (obstacle != null)
        {
            obstacleFullScale = obstacle.localScale;

            Renderer obstacleRenderer = obstacle.GetComponent<Renderer>();
            if (obstacleRenderer != null)
                obstacleHalfHeight = obstacleRenderer.bounds.extents.y;

            obstacle.localScale = Vector3.zero;
        }

        HideSpikes();
    }

    // Puts everything back as at the start of a game
    public void ResetAll()
    {
        StopAllCoroutines();
        HideSpikes();

        if (chestLid != null)
            chestLid.SetPositionAndRotation(lidStartPosition, lidStartRotation);

        for (int i = 0; i < crystalStartPositions.Length; i++)
        {
            crystals[i].SetPositionAndRotation(crystalStartPositions[i], crystalStartRotations[i]);
        }

        if (obstacle != null)
            obstacle.localScale = Vector3.zero;
    }

    // ---------- Trap ----------

    public IEnumerator PlayTrap()
    {
        SetSpikesVisible(true);
        yield return StartCoroutine(MoveSpikes(0f, 1f, spikeRiseTime));
        yield return new WaitForSeconds(spikeStayTime);
        yield return StartCoroutine(MoveSpikes(1f, 0f, spikeRiseTime));
        HideSpikes();
    }

    // progress 0 = hidden under the board, 1 = fully out
    private IEnumerator MoveSpikes(float from, float to, float time)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            float progress = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)));
            SetSpikeProgress(progress);
            yield return null;
        }
    }

    private void SetSpikeProgress(float progress)
    {
        for (int i = 0; i < spikes.Length; i++)
        {
            spikes[i].position = spikeStartPositions[i] - Vector3.up * spikeHeight * (1f - progress);
        }
    }

    private void SetSpikesVisible(bool visible)
    {
        for (int i = 0; i < spikes.Length; i++)
        {
            Renderer spikeRenderer = spikes[i].GetComponent<Renderer>();
            if (spikeRenderer != null)
                spikeRenderer.enabled = visible;
        }
    }

    private void HideSpikes()
    {
        if (spikes == null)
            return;

        SetSpikeProgress(0f);
        SetSpikesVisible(false);
    }

    // ---------- Treasure ----------

    // The treasure shown is the one closest to the tile (chest or crystals)
    public IEnumerator PlayTreasure(Vector3 tilePosition)
    {
        float chestDistance = chestLid != null
            ? Vector3.Distance(chestLid.position, tilePosition)
            : float.MaxValue;

        float crystalDistance = (crystals != null && crystals.Length > 0)
            ? Vector3.Distance(crystals[0].position, tilePosition)
            : float.MaxValue;

        if (chestDistance <= crystalDistance)
            yield return StartCoroutine(OpenChest());
        else
            yield return StartCoroutine(SpinCrystals());
    }

    private IEnumerator OpenChest()
    {
        if (chestLid == null)
            yield break;

        Renderer lidRenderer = chestLid.GetComponent<Renderer>();
        if (lidRenderer == null)
            yield break;

        // The hinge is the back-bottom edge of the lid (the lock is at the front, -Z)
        Bounds bounds = lidRenderer.bounds;
        Vector3 hinge = new Vector3(bounds.center.x, bounds.min.y, bounds.max.z);

        float opened = 0f;
        float total = Mathf.Abs(lidOpenAngle);
        float direction = Mathf.Sign(lidOpenAngle);

        while (opened < total)
        {
            float step = (total / lidOpenTime) * Time.deltaTime;
            step = Mathf.Min(step, total - opened);

            chestLid.RotateAround(hinge, Vector3.right, step * direction);
            opened += step;

            yield return null;
        }
    }

    private IEnumerator SpinCrystals()
    {
        if (crystals == null || crystals.Length == 0)
            yield break;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / crystalTime;
            float k = Mathf.Clamp01(t);

            for (int i = 0; i < crystals.Length; i++)
            {
                crystals[i].rotation = crystalStartRotations[i] * Quaternion.Euler(0f, 360f * k, 0f);
                crystals[i].position = crystalStartPositions[i] + Vector3.up * (crystalLift * Mathf.Sin(k * Mathf.PI));
            }

            yield return null;
        }
    }

    // ---------- Challenge ----------

    // The obstacle grows on the given point (the surface of the board)
    public IEnumerator ShowObstacle(Vector3 surfacePoint)
    {
        if (obstacle == null)
            yield break;

        obstacle.position = surfacePoint + Vector3.up * obstacleHalfHeight;
        yield return StartCoroutine(ScaleObstacle(0f, 1f));
    }

    public IEnumerator HideObstacle()
    {
        if (obstacle == null)
            yield break;

        yield return StartCoroutine(ScaleObstacle(1f, 0f));
    }

    private IEnumerator ScaleObstacle(float from, float to)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / obstacleTime;
            float k = Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)));
            obstacle.localScale = obstacleFullScale * k;
            yield return null;
        }
    }

    // ---------- Quick tests (Play mode): right-click the component title ----------

    [ContextMenu("Test Trap")]
    private void TestTrap() { StartCoroutine(PlayTrap()); }

    [ContextMenu("Test Chest")]
    private void TestChest() { StartCoroutine(OpenChest()); }

    [ContextMenu("Test Crystals")]
    private void TestCrystals() { StartCoroutine(SpinCrystals()); }

    [ContextMenu("Reset All")]
    private void TestReset() { ResetAll(); }
}