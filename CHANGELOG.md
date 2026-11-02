# Changelog - PKOpenPaths

## [2026-11-02]

### Added
- **Forest Village Armor Shop Early Door Unlock (`ve02`)**:
  - Implemented [ForestVillageArmorShopPatch.cs](Patches/ForestVillageArmorShopPatch.cs) hooking `MACHICON.EventInit` (`[HarmonyPrefix]`) on map `ve02`.
  - Neutralizes `ev[4]` (MapInit blocker `obj[40]` spawn check: `[4, 114, 4, 255, 254]` -> `[0, 0, 0, 0, 254]`) and `ev[5]` (Talk "Closed." trigger: `[4, 114, 4, 5, 40, 255, 254]` -> `[0, 0, 0, 0, 0, 0, 254]`) in `mapeventdat[0]`.
  - Strictly preserves 100% exact bytecode array lengths (5 bytes for `ev[4]`, 7 bytes for `ev[5]`) to prevent parser desyncs.
  - Allows door `ev[6]` (`X: 54..58, Y: 128..129`) leading to Armor Shop interior `(46, 53)` (Shop 11) to open at any time regardless of Story Flag 114 Bit 4 (late-game Greenhill/Rockaxe council meeting).
  - Configurable via `ModConfiguration.cs` under `[Suikoden 2 Forest Village]` -> `EnableForestVillageArmorShopUnlocked` (default: `true`).
