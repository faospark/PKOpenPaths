# P.K. - Open Paths

**By [faospark](https://github.com/faospark)**

A **BepInEx 6 (IL2CPP)** mod for **Suikoden I & II HD Remaster** that opens blocked shortcuts and auto-solves environmental obstacles in **Suikoden II** using safe, in-memory map patching on load (`MACHICON.EventInit`).

## Features

- **Cave of the Wind Solved (`EnableCaveOfTheWindSolved`)**: Automatically places all pushable wind-puzzle boulders in `vd06` (rooms 8–11) into their wind-blocking positions while preserving story cutscenes, chests, and Kahn Marley's recruitment.
- **Tinto Mines Shortcuts (`EnableTintoMinesShortcuts`)**: Clears obstructing rock piles and barrier hitboxes in `vg15` (room 0) and `vg02` (room 2) for unobstructed traversal.
- **Forest Village Armor Shop Unlocked (`EnableForestVillageArmorShopUnlocked`)**: Unlocks the exterior Armor Shop door in Forest Village (`ve02`) early without waiting for late-game Greenhill liberation.

> For the dual-engine architecture and HD ghost-texture notes, see [Documentation/hd_texture_sync_and_ghost_object_resolution.md](Documentation/hd_texture_sync_and_ghost_object_resolution.md).

## Configuration (`BepInEx/config/faospark.pkopenpaths.cfg`)

```ini
[Suikoden 2 Cave of the Wind]
EnableCaveOfTheWindSolved = true             # Auto-place wind-blocking boulders in vd06

[Suikoden 2 Tinto Mines]
EnableTintoMinesShortcuts = true             # Clear rock blockages & barrier hitboxes in vg02/vg15

[Suikoden 2 Forest Village]
EnableForestVillageArmorShopUnlocked = true  # Unlock Forest Village (ve02) Armor Shop door early
```

## Requirements

- **BepInEx 6.0.0-pre.2 IL2CPP**
- **Suikoden I & II HD Remaster**
- **Recommended:** [Suikoden Fix](https://github.com/d3xMachina/SuikodenFix) by d3xMachina

## Installation

1. Install [BepInEx 6.0 (Unity IL2CPP x64)](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip) into your game root directory.
2. Place `PKOpenPaths.dll` into `BepInEx/plugins/`.
3. Launch the game once to generate `BepInEx/config/faospark.pkopenpaths.cfg`.

## Credits & License

- **[faospark](https://github.com/faospark)** — Primary Author & Lead Researcher
- **[d3xMachina](https://github.com/d3xMachina/Suikoden-Fix)** — Creator of Suikoden-Fix & BepInEx IL2CPP foundation
- **[asilverthorn](https://github.com/asilverthorn/suikoden_ref)** — Reference tables, internal IDs, and save structure research
- **Moogles & Mods Discord** — Community support & research ([Invite](https://discord.com/invite/bSnpVBV))
- **Framework:** [BepInEx](https://github.com/BepInEx/BepInEx) & [HarmonyX](https://github.com/BepInEx/HarmonyX)

Distributed under the [MIT License](LICENSE).
