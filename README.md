# P.K. - Open Paths

A BepInEx mod for **Suikoden I & II HD Remaster** that opens blocked shortcuts and auto-solves environmental obstacles (such as the Cave of the Wind boulder wind-puzzle and Tinto Mines shortcuts) using safe, non-destructive in-memory byte patching on map load (`MACHICON.EventInit`).

## Features

- **Cave of the Wind Solved (`vd06`)**:
  - Automatically positions all pushable boulders (`ano = 6`, `otyp = 3`) across rooms 8, 9, 10, and 11 into their wind-blocking positions upon map load.
  - Allows immediate traversal without manual boulder pushing while keeping all story cutscenes, chest triggers, and Kahn Marley recruitment sequences completely intact.

- **Tinto Mines Shortcuts (`vg02`, `vg15`)**:
  - **`vg15` (Room 0)**: Clears the large rock pile blocking the tunnel by repositioning boundary rocks and hiding the obstructing rocks (`disp = 0`, `ozok = 0`).
  - **`vg02` (Room 2)**: Adjusts the barrier hitboxes near the mine shafts to open unobstructed walking corridors.

- **Forest Village Armor Shop Door (`ve02`)**:
  - Unlocks the exterior door to the Armor Shop in Forest Village (`ve02`) early.
  - Neutralizes the late-game Greenhill liberation story flag requirement (Story Flag 114 Bit 4) that normally blocks the entrance with `"Closed."`, allowing immediate access to the Armor Shop (Shop 11) and Connell at any point in the game.

## Configuration

Configuration is located at `BepInEx/config/faospark.pkopenpaths.cfg` after the first launch:

```ini
[Suikoden 2 Cave of the Wind]

## Automatically places the puzzle boulders in Cave of the Wind (vd06) into their wind-blocking positions, allowing immediate passage without pushing boulders.
# Setting type: Boolean
# Default value: true
EnableCaveOfTheWindSolved = true

[Suikoden 2 Tinto Mines]

## Removes rock blockages and barrier triggers in Tinto Mines (vg02, vg15) to open convenient shortcuts.
# Setting type: Boolean
# Default value: true
EnableTintoMinesShortcuts = true

[Suikoden 2 Forest Village]

## Unlocks the exterior door to the Armor Shop in Forest Village (ve02) early, removing the late-game Greenhill liberation story flag requirement.
# Setting type: Boolean
# Default value: true
EnableForestVillageArmorShopUnlocked = true
```

## How It Works

Instead of distributing raw modified Unity `.bundle` files or JSON files that break whenever the remaster updates and conflict with other mods, **PKOpenPaths** hooks `MACHICON.EventInit` via Harmony:
1. Detects when the player transitions into a supported map (e.g. `vd06`).
2. Directly patches the deserialized map event data (`__result.mdat.eventdata.mapeventdat`) in memory before room rendering and actor spawning.
3. Completely update-proof, conflict-free, and instantly toggleable.

## Technical Architecture & HD Texture Research

For an in-depth breakdown of the dual-engine architecture (PS1 2D simulation vs. Unity HD visual layer) and how HD "ghost" textures are handled, see:
- [HD Visual Layer & Texture Sync Research](Documentation/hd_texture_sync_and_ghost_object_resolution.md)

---

## Installation

- **Prerequisite**: [BepInEx 6.0 (Unity IL2CPP x64)](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip) installed in your game root directory.
- **Install**: Extract the release ZIP directly into your game's root directory (or place `PKOpenPaths.dll` into `BepInEx/plugins/`).
- **Verify**: Launch the game; `BepInEx/LogOutput.log` will confirm `[PKOpenPaths] loaded successfully!`.

---

## Credits & Acknowledgments

- **[d3xMachina](https://github.com/d3xMachina/Suikoden-Fix)**: Creator of [Suikoden-Fix](https://github.com/d3xMachina/Suikoden-Fix). Bootstrapped the foundational BepInEx IL2CPP implementation and provided early inspiration for Remaster modding.
- **[asilverthorn](https://github.com/asilverthorn/suikoden_ref)**: Creator of [suikoden_ref](https://github.com/asilverthorn/suikoden_ref). Invaluable early reference tables, internal IDs, and save structure research.
- **[faospark](https://github.com/faospark)**: Primary Author & Lead Researcher.
- **Moogles & Mods Discord Community**: For ongoing support, research, collaboration, and keeping the Suikoden modding scene thriving. Join us on the [Moogles & Mods Discord](https://discord.com/invite/bSnpVBV).
- **Framework**: [BepInEx](https://github.com/BepInEx/BepInEx) & [HarmonyX](https://github.com/BepInEx/HarmonyX).

---

## License

Distributed under the [MIT License](LICENSE).
