
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Board Reference")]
    [SerializeField] private BoardPathManager boardPath;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    private int currentTileIndex = 0;
    private bool isMoving = false;

    // Allows TurnManager to check if the player is moving
    public bool IsMoving
    {
        get { return isMoving; }
    }

    private void Start()
    {
        Transform firstTile = boardPath.GetTile(0);
        transform.position = firstTile.position + Vector3.up * 0.25f;
    }

    public void MoveSteps(int steps)
    {
        if (isMoving || steps <= 0)
            return;

        StartCoroutine(MoveAlongPath(steps));
    }

    private IEnumerator MoveAlongPath(int steps)
    {
        isMoving = true;

        int targetIndex = Mathf.Min(
            currentTileIndex + steps,
            boardPath.TotalTiles - 1
        );

        while (currentTileIndex < targetIndex)
        {
            currentTileIndex++;

            Vector3 targetPosition =
                boardPath.GetTile(currentTileIndex).position
                + Vector3.up * 0.25f;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }

            transform.position = targetPosition;
        }

        isMoving = false;
    }
}
