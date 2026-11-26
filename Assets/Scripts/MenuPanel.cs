using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Main menu and victory screen. Lives on MenuCanvas (always active);
// only the Panel child is shown or hidden.
public class MenuPanel : MonoBehaviour
{
    public enum Choice { None, Play, MainMenu }

    [Header("References")]
    [SerializeField] private GameObject content;      // the Panel
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;

    // What the player picked. TurnManager waits until this is not None.
    public Choice Chosen { get; private set; }
    public bool IsVisible => content != null && content.activeSelf;

    private RectTransform quitRect;
    private Vector2 quitPos;   // original position of the Quit button
    private Vector2 menuPos;   // original position of the Main menu button

    private void Awake()
    {
        quitRect = quitButton.GetComponent<RectTransform>();
        quitPos = quitRect.anchoredPosition;
        menuPos = menuButton.GetComponent<RectTransform>().anchoredPosition;

        playButton.onClick.AddListener(() => Pick(Choice.Play));
        menuButton.onClick.AddListener(() => Pick(Choice.MainMenu));
        quitButton.onClick.AddListener(QuitGame);
    }

     // Keyboard fallback: Enter = play, M = main menu.
    private void Update()
    {
        if (!IsVisible || Chosen != Choice.None) return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
            Pick(Choice.Play);
        else if (kb.mKey.wasPressedThisFrame && menuButton.gameObject.activeSelf)
            Pick(Choice.MainMenu);
    }

    public void ShowMainMenu()
    {
        titleText.text = "JUMANJI XR";
        subtitleText.text = "Survive the adventure";
        SetLabel(playButton, "Play");

        // No "Main menu" button here: move Quit up to close the gap.
        menuButton.gameObject.SetActive(false);
        quitRect.anchoredPosition = menuPos;
        Open();
    }

    public void ShowVictory(int turns, int treasures)
    {
        titleText.text = "JUMANJI!";
        subtitleText.text = $"Turns: {turns}   Treasures: {treasures}";
        SetLabel(playButton, "Play again");
        SetLabel(menuButton, "Main menu");

        menuButton.gameObject.SetActive(true);
        quitRect.anchoredPosition = quitPos;
        Open();
    }

    public void Hide()
    {
        content.SetActive(false);
    }

    private void Open()
    {
        Chosen = Choice.None;
        content.SetActive(true);
    }

    private void Pick(Choice choice)
    {
        if (!IsVisible || Chosen != Choice.None) return;
        Chosen = choice;
        Debug.Log("[Menu] Chosen: " + choice);
    }

    private static void SetLabel(Button button, string text)
    {
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.text = text;
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    [ContextMenu("Test main menu")]
    private void TestMainMenu() => ShowMainMenu();

    [ContextMenu("Test victory")]
    private void TestVictory() => ShowVictory(9, 2);
}