
using UnityEngine;

public class DiceController : MonoBehaviour
{
    public int RollDice()
    {
        int diceResult = Random.Range(1, 7);

        Debug.Log("Dice result: " + diceResult);

        return diceResult;
    }
}
