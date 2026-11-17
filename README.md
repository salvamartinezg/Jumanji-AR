# Jumanji XR – Survive the Adventure

A 3D board game in virtual reality for **Meta Quest 2**, inspired by the Jumanji universe. The player throws a virtual die with their hands (hand tracking), moves their token across a jungle board placed on a table in an attic, overcomes traps, collects treasures and must reach the final temple to win the game.

Single-player game, developed as a team.

## Team

- Salvador – [@salvamartinezg]
- Javier – [@javimartiin]

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

## Tech stack

| Tool | Use |
|------|-----|
| Unity (URP) | Game development |
| Meta XR SDK | VR features for Meta Quest |
| XR Interaction Toolkit | VR interactions |
| Hand Tracking | Interaction with the die and the token |
| ProBuilder | Building the board and the attic room |
| Blender | Modeling the player token and the die |
| Poly Pizza / Kenney / Sketchfab / Unity Asset Store | Rest of the low-poly models (temple, chests, crystals, decoration, furniture) |

## Game concept

1. The player starts the game seated in a fixed position, in front of a table with the jungle board in an attic.
2. They throw the die with their hand.
3. They pick up the token with their hand and place it on the tile that matches the number rolled.
4. Landing on special tiles triggers events:
   - **Trap:** spikes come out of the floor; the player loses a turn or goes back.
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


