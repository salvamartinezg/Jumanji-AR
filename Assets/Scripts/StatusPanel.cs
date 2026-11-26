using TMPro;
using UnityEngine;

// Floating panel that shows the game status to the player.
public class StatusPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;

    private string info = "";
    private string message = "";

    public void SetInfo(int turn, int tile, int totalTiles, int treasures)
    {
        info = "Turn " + turn + "   |   Tile " + tile + "/" + totalTiles + "   |   Treasures " + treasures;
        Refresh();
    }

    public void SetMessage(string text)
    {
        message = text;
        Refresh();
    }

    private void Refresh()
    {
        if (statusText == null)
            return;

        statusText.text =
            "<b>" + info + "</b>\n" +
            "<size=80%><color=#F2B632>" + message + "</color></size>";
    }
}