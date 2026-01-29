# Dig Dug Chat (KISS)

This is a lightweight, chat-controlled Dig Dug inspired mini-game built on top
of the LiveChat command system. The playable scene is:

- `Assets/Scenes/DigDugChat.unity`

## How to Play

- Start Play Mode with the DigDugChat scene open.
- Use chat commands (or the local debug chat input at the bottom of the screen).
- Clear all enemies to win; touching an enemy ends the run.

### Chat Commands

- `!dig up|down|left|right` (aliases: `!move`, `!u`, `!d`, `!l`, `!r`)
- `!pump` (aliases: `!fire`)
- `!start` (aliases: `!reset`, `!restart`)
- `!dighelp`

## Required Pixel Art Assets (Low-Res)

These are the minimal assets needed for a 16x16 pixel art presentation. If
these sprites exist under `Assets/Resources/DigDugChat/`, the game will use
them. Otherwise it falls back to procedural placeholder sprites.

1. `digdug_dirt.png`
   - 16x16 dirt tile with a subtle dither pattern for the tilemap.
2. `digdug_player.png`
   - 16x16 player sprite (simple helmet/body silhouette).
3. `digdug_enemy.png`
   - 16x16 enemy sprite (round blob with eyes).
4. `digdug_pump.png`
   - 16x16 pump bubble/segment sprite for the pump VFX.

Optional polish assets (not required for the KISS build):

- `digdug_ui_panel.png` (frame for HUD text)
- `digdug_win.png` / `digdug_lose.png` (small pixel banners)

## Notes

- The game is built to be simple and readable: grid-based digging, a short
  pump range, and simple enemy AI that uses line-of-sight when possible.
- Replace the `LocalDebugLiveChatClient` with a real chat client to go live.
