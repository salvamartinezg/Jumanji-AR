using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Board Reference")]
    [SerializeField] private BoardPathManager boardPath;

    [Header("Movement Settings")]
    [Tooltip("Speed in board units per second (scaled with the board).")]
    [SerializeField] private float moveSpeed = 3f;

    private int currentTileIndex = 0;
    private bool isMoving = false;
    private Renderer tokenRenderer;

    // Allows TurnManager to check if the player is moving
    public bool IsMoving
    {
        get { return isMoving; }
    }

    private void Start()
    {
        tokenRenderer = GetComponent<Renderer>();
        transform.position = GetStandPosition(boardPath.GetTile(0));
    }

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
