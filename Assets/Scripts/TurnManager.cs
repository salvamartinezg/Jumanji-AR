
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private PlayerMovement player1;
    [SerializeField] private PlayerMovement player2;

    [Header("Dice")]
    [SerializeField] private DiceController diceController;

    private int currentPlayerIndex = 0;
    private bool turnInProgress = false;

    private void Start()
    {
        Debug.Log("Game started. Player 1's turn!");
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

        PlayerMovement activePlayer;

        if (currentPlayerIndex == 0)
        {
            activePlayer = player1;
            Debug.Log("Player 1 (RED) is playing!");
        }
        else
        {
            activePlayer = player2;
            Debug.Log("Player 2 (BLUE) is playing!");
        }

        int diceResult = diceController.RollDice();

        activePlayer.MoveSteps(diceResult);

        while (activePlayer.IsMoving)
        {
            yield return null;
        }

        Debug.Log("Turn finished!");

        currentPlayerIndex = 1 - currentPlayerIndex;

        if (currentPlayerIndex == 0)
            Debug.Log("Now it's Player 1's (RED) turn!");
        else
            Debug.Log("Now it's Player 2's (BLUE) turn!");

        turnInProgress = false;
    }
}
