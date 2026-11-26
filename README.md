# Jumanji XR – Survive the Adventure

A 3D board game in virtual reality for **Meta Quest 2**, inspired by the Jumanji universe. The player throws a virtual die with their hands (hand tracking), moves their token across a jungle board placed on a table in an attic, overcomes traps, collects treasures and must reach the final temple to win the game.

Single-player game, developed as a team.

## Team

- Salvador – [@salvamartinezg](https://github.com/salvamartinezg)
- Javier – [@javimartiin](https://github.com/javimartiin)

## Videos

All the videos requested for grading, in one place.

| Item | Content | Video |
|------|---------|-------|
| Homework 1 | Lab 01 – Vuforia AR project ([`Labs/Lab01`](Labs/Lab01)) | [Watch video](Lab1/Video/Lab1%20-%20SampleScene%20-%20Windows%2C%20Mac%2C%20Linux%20-%20Unity%206.6%20%286000.6.4f1%29%20_DX11_%202026-11-26%2018-16-30.mp4) |
| Homework 2 | Lab 02 – VR project with the XR Interaction Toolkit ([`Labs/Lab02`](Labs/Lab02)) | [Watch video](Labs/Lab02/Video/DemoVRLab2.mp4) |
| Homework 3 | Lab 03 – Hand model made in Blender and imported into a Unity XR project ([`Labs/Lab03`](Labs/Lab03)) | [Watch video](Labs/Lab03/Video/_%20Hand_Final%20%5BC__Users_Javi_Desktop_IntroductionMixedReality-2026_Labs_Lab03_Blender_Hand_Final.blend%5D%20-%20Blender%205.2.2%20LTS%202026-11-26%2018-36-09.mp4) |
| Project progress – this week | Whole game loop: menu, die throw, token placement, tile events and victory screen | _Link to be added_ |

## Documentation

- [State of the art - Homework 4](Docs/State-of-the-art.pdf)
- [Main Modules - Homework 5](Docs/Main_Modules_Jumanji_XR.pdf)

## Development Progress

| Week | Progress |
|------|----------|
| Week 1 · 19–25 October 2026 | **Lab 02 completed:** Unity VR project built with the XR Interaction Toolkit (`Labs/Lab02/VRLab02`) and its demo video (`Labs/Lab02/Video`). |
| Week 2 · 26 October–1 November 2026 | **State of the art – Homework 4 completed** ([PDF](Docs/State-of-the-art.pdf)). Lab 01 (Vuforia AR project) and Lab 03 (hand model made in Blender and imported into a Unity XR project) added to the repository. |
| Week 3 · 2–8 November 2026 | **Initial prototyping completed.** |
| Week 4 · 9–15 November 2026 | **Graybox completed:** attic room built with ProBuilder (floor, side walls, sloped roof, wooden beams and a window with glass), table  and chair at real scale, board placed on the table, primitive stand-ins for the temple, chest, crystal and spike trap, and attic props (shelf, trunk, boxes, rug and hanging lamp). The die and the token can be grabbed (XR Grab Interactable). Seated player with locomotion disabled, project set up for Meta Quest 2 (Android, OpenXR, hand tracking) and tested in the editor with the XR Interaction Simulator. Scale testing on the Quest 2 is still pending. |
| Week 5–6 · 16–29 November 2026 | **Whole game loop playable**: main menu, physical die, token placed by hand, trap / treasure / challenge events, progress panel and victory screen. Graybox given color and detail. Tested in the editor with the XR Interaction Simulator and keyboard fallbacks; testing of the hand interactions on the Quest 2 is still pending. |


### Game rules in the current build

- The board has 12 tiles in a snake path: **1** start, **4** treasure (chest), **6** trap, **7** challenge, **10** treasure (crystals) and **12** temple; the rest are normal tiles.
- **Trap:** spikes come out of the floor and the token goes back two tiles.
- **Treasure:** the chest opens or the crystals rise. Each treasure tile counts only once.
- **Challenge:** an obstacle blocks the way and the player throws the die again. A 4 or more overcomes it; otherwise the player loses the next turn.
- **Temple:** reaching tile 12 ends the game and shows the victory screen with the number of turns and treasures.

### How to play

In the headset (hand tracking):

1. Press **Play** on the floating menu with your index finger.
2. Throw the die with your hand. The number on the top face is read when it stops.
3. Pick up the token and put it on the tile that lights up. If you release it close enough, it snaps to the tile.
4. Resolve the tile event and keep playing until the token reaches the temple.

In the editor (keyboard fallbacks, used to test and record without the headset; the World Space menu cannot be clicked with the mouse):

| Key | When | Action |
|-----|------|--------|
| Enter | Menu or victory screen | Play / Play again |
| M | Victory screen | Back to the main menu |
| R | Waiting for a die roll | Random roll |
| 1–6 | Waiting for a die roll | Forced roll (makes the demo deterministic) |
| M | Placing the token | Move the token automatically |


## Tech stack

| Tool | Use |
|------|-----|
| Unity (URP) | Game development |
| Meta XR SDK | VR features for Meta Quest |
| XR Interaction Toolkit | VR interactions (grab, poke, UI) |
| Hand Tracking | Interaction with the die, the token and the menu |
| ProBuilder | Building the board and the attic room |
| TextMeshPro | Die faces, status panel and menus |
| Blender | Modeling the player token and the die |
| Poly Pizza / Kenney / Sketchfab / Unity Asset Store | Rest of the low-poly models (temple, chests, crystals, decoration, furniture) |

## Game concept

1. The player starts the game seated in a fixed position, in front of a table with the jungle board in an attic.
2. They throw the die with their hand.
3. They pick up the token with their hand and place it on the tile that matches the number rolled.
4. Landing on special tiles triggers events:
   - **Trap:** spikes come out of the floor and the token goes back.
   - **Treasure:** the player finds a chest or a crystal.
   - **Challenge:** an obstacle appears; the player throws the die again to overcome it, or loses the turn.
   - **Normal:** no event.
5. When the token reaches the final temple, the player wins.

Flow: Launch Game → Main Menu → Enter Game → Roll Die → Move Token → Board Event → Update Progress → Check Final Temple → Victory Screen.

## Art direction

**Stylized / low-poly** style. An attic with wooden beams, a window, shelves and old objects, where the jungle board (with miniature trees, ruins, traps, magic crystals and chests) is the colorful focus of the scene.

- Player in a fixed position, no locomotion.
- Flat shading, flat colors and a limited palette.


## References

- Jumanji: The Curse Returns
- Jumanji: The Video Game
- Demeo

These are sources of inspiration; we do not intend to copy them.
