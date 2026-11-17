using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Single-player turn loop: roll the die, move the token, wait for the next roll.
public class TurnManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerMovement player;

    [Header("Dice")]
    [SerializeField] private DiceController diceController;

    private bool turnInProgress = false;

    private void Start()
    {
        Debug.Log("Game started. Press R to roll the die.");
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame &&
            !turnInProgress)
        {
            StartCoroutine(PlayTurn());
        }
    }

    private IEnumerator PlayTurn()
    {
        turnInProgress = true;

        int diceResult = diceController.RollDice();

        player.MoveSteps(diceResult);

        while (player.IsMoving)
        {
            yield return null;
        }

        Debug.Log("Turn finished. Press R to roll again.");

        turnInProgress = false;
    }
}
