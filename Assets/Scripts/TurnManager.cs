using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Game loop: main menu -> throw the die -> place the token -> resolve the tile -> victory screen.
public class TurnManager : MonoBehaviour
{
    private enum GameState
    {
        Menu,
        WaitingForRoll,
        Moving,
        ResolvingTile,
        Victory
    }

    [Header("References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private DiceController diceController;
    [SerializeField] private BoardPathManager boardPath;
    [SerializeField] private StatusPanel panel;
    [SerializeField] private BoardEffects effects;
    [SerializeField] private MenuPanel menu;

    [Header("Rules")]
    [Tooltip("Tiles the token goes back when it lands on a trap.")]
    [SerializeField] private int trapStepsBack = 2;
    [Tooltip("Minimum roll needed to overcome a challenge.")]
    [SerializeField] private int challengeMinRoll = 4;

    private GameState state;
    private int turn;
    private int treasures;
    private int lastRoll;
    private bool skipNextTurn;

    // Treasure tiles already collected in this game
    private readonly HashSet<int> collectedTreasures = new HashSet<int>();

    private void Start()
    {
        // If they are not assigned in the Inspector, look for them in the scene
        if (boardPath == null)
            boardPath = FindFirstObjectByType<BoardPathManager>();

        if (panel == null)
            panel = FindFirstObjectByType<StatusPanel>();

        if (effects == null)
            effects = FindFirstObjectByType<BoardEffects>();

        if (menu == null)
            menu = FindFirstObjectByType<MenuPanel>();

        StartCoroutine(GameLoop());
    }

    // Shows a message to the player (panel) and in the Console
    private void Say(string text)
    {
        Debug.Log(text);

        if (panel != null)
            panel.SetMessage(text);
    }

    // Updates the first line of the panel: turn, tile and treasures
    private void UpdateInfo()
    {
        if (panel != null)
            panel.SetInfo(turn, player.CurrentTileIndex + 1, boardPath.TotalTiles, treasures);
    }

    // The status panel is hidden while a menu is on screen
    private void ShowStatusPanel(bool visible)
    {
        if (panel != null)
            panel.gameObject.SetActive(visible);
    }

    private IEnumerator GameLoop()
    {
        // Wait one frame so every other script has run its Start()
        yield return null;

        if (menu == null)
        {
            Debug.LogError("TurnManager: no MenuPanel found in the scene. Add it to MenuCanvas.");
            yield break;
        }

        while (true)
        {
            // ---- Main menu ----
            state = GameState.Menu;
            ResetBoard();
            ShowStatusPanel(false);
            menu.ShowMainMenu();
            yield return new WaitUntil(() => menu.Chosen != MenuPanel.Choice.None);
            menu.Hide();

            // ---- Games, until the player goes back to the main menu ----
            bool playing = true;
            while (playing)
            {
                StartNewGame();
                ShowStatusPanel(true);

                while (state != GameState.Victory)
                {
                    yield return StartCoroutine(PlayTurn());
                }

                // ---- Victory screen ----
                UpdateInfo();
                ShowStatusPanel(false);
                menu.ShowVictory(turn, treasures);
                yield return new WaitUntil(() => menu.Chosen != MenuPanel.Choice.None);

                playing = menu.Chosen == MenuPanel.Choice.Play;
                menu.Hide();
            }
        }
    }

    // Token back to the start and every prop in its initial state
    private void ResetBoard()
    {
        player.ResetToStart();

        if (effects != null)
            effects.ResetAll();
    }

    private void StartNewGame()
    {
        turn = 0;
        treasures = 0;
        skipNextTurn = false;
        collectedTreasures.Clear();

        ResetBoard();

        state = GameState.WaitingForRoll;
        Debug.Log("New game. Keyboard help: R = random roll, 1-6 = fixed roll, M = move the token automatically.");
    }

    private IEnumerator PlayTurn()
    {
        turn++;
        UpdateInfo();

        // The player failed a challenge: this turn is lost
        if (skipNextTurn)
        {
            skipNextTurn = false;
            Say("Turn lost because of the challenge!");
            yield return new WaitForSeconds(1.5f);
            yield break;
        }

        // 1. Roll the die
        state = GameState.WaitingForRoll;
        Say("Throw the die with your hand!");
        yield return StartCoroutine(ThrowDie());
        int roll = lastRoll;

        // 2. The target tile lights up and the player places the token on it
        state = GameState.Moving;
        int target = Mathf.Min(player.CurrentTileIndex + roll, boardPath.TotalTiles - 1);

        if (target > player.CurrentTileIndex)
        {
            Say("You rolled a " + roll + ". Put your token on the glowing tile.");
            player.BeginPlacement(target);

            while (!player.PlacementDone)
            {
                if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
                {
                    // Fallback: the token walks by itself
                    player.CancelPlacement();
                    player.MoveSteps(roll);

                    while (player.IsMoving)
                    {
                        yield return null;
                    }

                    break;
                }

                yield return null;
            }
        }

        UpdateInfo();

        // 3. Resolve the tile where the token landed
        state = GameState.ResolvingTile;
        yield return StartCoroutine(ResolveTile(player.CurrentTileIndex));
        UpdateInfo();

        // 4. Check victory (last tile of the path)
        if (player.CurrentTileIndex >= boardPath.TotalTiles - 1)
        {
            state = GameState.Victory;
        }
    }

    // Waits for a throw of the die (by hand, or keyboard as a fallback). Result in lastRoll.
    private IEnumerator ThrowDie()
    {
        diceController.ArmThrow();

        lastRoll = 0;
        while (lastRoll == 0)
        {
            lastRoll = diceController.ThrowResult;

            if (lastRoll == 0)
                lastRoll = ReadKeyboardRoll();

            yield return null;
        }

        diceController.Disarm();
    }

    // R = random roll, 1-6 = fixed value (handy to record the demo video)
    private int ReadKeyboardRoll()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return 0;

        if (kb.rKey.wasPressedThisFrame) return diceController.RollDice();
        if (kb.digit1Key.wasPressedThisFrame) return 1;
        if (kb.digit2Key.wasPressedThisFrame) return 2;
        if (kb.digit3Key.wasPressedThisFrame) return 3;
        if (kb.digit4Key.wasPressedThisFrame) return 4;
        if (kb.digit5Key.wasPressedThisFrame) return 5;
        if (kb.digit6Key.wasPressedThisFrame) return 6;

        return 0;
    }

    private IEnumerator ResolveTile(int tileIndex)
    {
        TileType type = boardPath.GetTileType(tileIndex);
        float pause = 0.8f;

        switch (type)
        {
            case TileType.Trap:
            {
                Say("TRAP! Spikes!");

                if (effects != null)
                    yield return StartCoroutine(effects.PlayTrap());

                // Penalty: the token goes back
                int backIndex = Mathf.Max(0, tileIndex - trapStepsBack);
                Say("You are pushed back " + (tileIndex - backIndex) + " tiles.");
                player.MoveToTile(backIndex);

                while (player.IsMoving)
                {
                    yield return null;
                }

                break;
            }

            case TileType.Treasure:
            {
                // Add returns false if this treasure was already collected
                if (collectedTreasures.Add(tileIndex))
                {
                    treasures++;
                    UpdateInfo();
                    Say("Treasure found!");

                    if (effects != null)
                    {
                        Vector3 tilePosition = boardPath.GetTile(tileIndex).position;
                        yield return StartCoroutine(effects.PlayTreasure(tilePosition));
                    }
                }
                else
                {
                    Say("You already took this treasure.");
                }

                break;
            }

            case TileType.Challenge:
            {
                Say("CHALLENGE! An obstacle blocks the way.");

                if (effects != null)
                    yield return StartCoroutine(effects.ShowObstacle(ObstaclePoint(tileIndex)));

                Say("Throw the die! You need " + challengeMinRoll + " or more.");
                yield return StartCoroutine(ThrowDie());

                if (lastRoll >= challengeMinRoll)
                {
                    Say("You rolled a " + lastRoll + ". Obstacle overcome!");
                }
                else
                {
                    Say("You rolled a " + lastRoll + ". You failed: you lose your next turn.");
                    skipNextTurn = true;
                }

                yield return new WaitForSeconds(1.5f);

                if (effects != null)
                    yield return StartCoroutine(effects.HideObstacle());

                break;
            }

            case TileType.Temple:
                Say("You reached the temple!");
                pause = 1.5f;
                break;

            default:
                Say("Nothing here. Keep going!");
                break;
        }

        // Time to read the message
        yield return new WaitForSeconds(pause);
    }

    // Point on the board surface between this tile and the next one
    private Vector3 ObstaclePoint(int tileIndex)
    {
        Transform tile = boardPath.GetTile(tileIndex);
        Transform next = boardPath.GetTile(Mathf.Min(tileIndex + 1, boardPath.TotalTiles - 1));

        Vector3 point = (tile.position + next.position) / 2f;

        Renderer tileRenderer = tile.GetComponent<Renderer>();
        point.y = tileRenderer != null ? tileRenderer.bounds.max.y : tile.position.y;

        return point;
    }
}