using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PlayerMovement : MonoBehaviour
{
    [Header("Board Reference")]
    [SerializeField] private BoardPathManager boardPath;

    [Header("Movement Settings")]
    [Tooltip("Speed in board units per second (scaled with the board).")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Hand Placement")]
    [Tooltip("Snap radius as a fraction of the distance between two tiles.")]
    [SerializeField] private float snapRadiusFactor = 0.7f;

    [Header("Target Tile Marker (light beam)")]
    [Tooltip("Unlit material for the light beam (e.g. Tile Glow).")]
    [SerializeField] private Material markerMaterial;
    [Tooltip("Beam width as a fraction of the distance between two tiles.")]
    [SerializeField] private float beamWidthFactor = 0.22f;
    [Tooltip("Beam height in meters.")]
    [SerializeField] private float beamHeight = 0.12f;
    [SerializeField] private float markerPulseSpeed = 6f;

    private int currentTileIndex = 0;
    private bool isMoving = false;
    private Renderer tokenRenderer;

    // Hand placement
    private XRGrabInteractable grab;
    private bool wasHeld = false;
    private bool awaitingPlacement = false;
    private bool placementDone = false;
    private int targetTileIndex = -1;
    private float snapRadius = 0.07f;

    // Light beam on the target tile
    private Transform marker;
    private float beamWidth = 0.025f;

    public bool IsMoving
    {
        get { return isMoving; }
    }

    // Tile where the token currently is (0 = first tile)
    public int CurrentTileIndex
    {
        get { return currentTileIndex; }
    }

    // True when the player has put the token on the target tile
    public bool PlacementDone
    {
        get { return placementDone; }
    }

    private void Start()
    {
        tokenRenderer = GetComponent<Renderer>();
        grab = GetComponent<XRGrabInteractable>();

        transform.position = GetStandPosition(boardPath.GetTile(0));

        // Sizes from the real distance between two tiles (works at any board scale)
        float spacing = Vector3.Distance(
            boardPath.GetTile(0).position,
            boardPath.GetTile(1).position
        );
        snapRadius = spacing * snapRadiusFactor;
        beamWidth = spacing * beamWidthFactor;

        CreateMarker();
    }

    private void Update()
    {
        PulseMarker();

        if (grab == null)
            return;

        bool held = grab.isSelected;

        // The player has just let go of the token
        if (wasHeld && !held)
            OnTokenReleased();

        wasHeld = held;
    }

    private void OnTokenReleased()
    {
        if (isMoving)
            return;

        if (awaitingPlacement && IsNearTile(targetTileIndex))
        {
            // Close enough: the token snaps onto the tile
            currentTileIndex = targetTileIndex;
            transform.position = GetStandPosition(boardPath.GetTile(currentTileIndex));
            HideMarker();
            awaitingPlacement = false;
            placementDone = true;
            Debug.Log("Token placed on tile " + (currentTileIndex + 1));
        }
        else
        {
            // Wrong place: the token goes back to the tile where it was
            transform.position = GetStandPosition(boardPath.GetTile(currentTileIndex));

            if (awaitingPlacement)
                Debug.Log("Too far! Put the token on the glowing tile.");
        }
    }

    private bool IsNearTile(int index)
    {
        Vector3 target = GetStandPosition(boardPath.GetTile(index));
        Vector3 offset = transform.position - target;
        float horizontal = new Vector2(offset.x, offset.z).magnitude;

        return horizontal <= snapRadius && Mathf.Abs(offset.y) <= snapRadius * 2f;
    }

    // ---------- Light beam ----------

    private void CreateMarker()
    {
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beam.name = "TileHighlight";
        Destroy(beam.GetComponent<Collider>());

        Renderer beamRenderer = beam.GetComponent<Renderer>();
        beamRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        beamRenderer.receiveShadows = false;

        if (markerMaterial != null)
            beamRenderer.sharedMaterial = markerMaterial;
        else
            Debug.LogWarning("Assign a material to 'Marker Material' in PlayerToken (PlayerMovement).");

        marker = beam.transform;
        ApplyBeamScale(1f);
        beam.SetActive(false);
    }

    // A Unity cylinder is 1 unit wide and 2 units tall at scale 1
    private void ApplyBeamScale(float widthMultiplier)
    {
        float width = beamWidth * widthMultiplier;
        marker.localScale = new Vector3(width, beamHeight / 2f, width);
    }

    private void ShowMarker(int tileIndex)
    {
        Transform tile = boardPath.GetTile(tileIndex);
        if (tile == null || marker == null)
            return;

        Renderer tileRenderer = tile.GetComponent<Renderer>();
        float tileTop = tileRenderer != null
            ? tileRenderer.bounds.max.y
            : tile.position.y;

        // Centered on the tile, standing on its surface
        marker.position = new Vector3(
            tile.position.x,
            tileTop + beamHeight / 2f,
            tile.position.z
        );
        marker.gameObject.SetActive(true);
    }

    private void HideMarker()
    {
        if (marker != null)
            marker.gameObject.SetActive(false);
    }

    private void PulseMarker()
    {
        if (marker == null || !marker.gameObject.activeSelf)
            return;

        ApplyBeamScale(0.9f + 0.1f * Mathf.Sin(Time.time * markerPulseSpeed));
    }

    // ---------- Placement ----------

    // The target tile lights up and waits for the player to place the token on it
    public void BeginPlacement(int targetIndex)
    {
        CancelPlacement();

        targetTileIndex = targetIndex;
        awaitingPlacement = true;
        placementDone = false;
        ShowMarker(targetIndex);
    }

    public void CancelPlacement()
    {
        HideMarker();
        awaitingPlacement = false;
        placementDone = false;
    }

    // Puts the token back on the first tile (used when a new game starts)
    public void ResetToStart()
    {
        StopAllCoroutines();
        CancelPlacement();
        isMoving = false;
        currentTileIndex = 0;
        transform.position = GetStandPosition(boardPath.GetTile(0));
    }


        // Slides the token straight to a tile (used by the trap penalty)
    public void MoveToTile(int index)
    {
        if (isMoving)
            return;

        StartCoroutine(SlideToTile(index));
    }

    private IEnumerator SlideToTile(int index)
    {
        isMoving = true;

        Vector3 targetPosition = GetStandPosition(boardPath.GetTile(index));
        float speed = moveSpeed * boardPath.transform.lossyScale.x;

        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

            yield return null;
        }

        currentTileIndex = index;
        isMoving = false;
    }

    // ---------- Automatic movement (fallback) ----------

    public void MoveSteps(int steps)
    {
        if (isMoving || steps <= 0)
            return;

        StartCoroutine(MoveAlongPath(steps));
    }

    // Point where the token rests on top of a tile.
    // Uses the real sizes of tile and token, so it works at any board scale.
    private Vector3 GetStandPosition(Transform tile)
    {
        Renderer tileRenderer = tile.GetComponent<Renderer>();

        float tileTop = tileRenderer != null
            ? tileRenderer.bounds.max.y
            : tile.position.y;

        float tokenHalfHeight = tokenRenderer != null
            ? tokenRenderer.bounds.extents.y
            : 0f;

        return new Vector3(
            tile.position.x,
            tileTop + tokenHalfHeight,
            tile.position.z
        );
    }

    private IEnumerator MoveAlongPath(int steps)
    {
        isMoving = true;

        int targetIndex = Mathf.Min(
            currentTileIndex + steps,
            boardPath.TotalTiles - 1
        );

        // The board is scaled down to sit on the table, so the speed scales with it.
        float speed = moveSpeed * boardPath.transform.lossyScale.x;

        while (currentTileIndex < targetIndex)
        {
            currentTileIndex++;

            Vector3 targetPosition =
                GetStandPosition(boardPath.GetTile(currentTileIndex));

            while (transform.position != targetPosition)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    speed * Time.deltaTime
                );

                yield return null;
            }
        }

        isMoving = false;
    }
}