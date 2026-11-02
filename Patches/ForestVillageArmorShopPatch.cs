extern alias GSD2;

using System;
using HarmonyLib;

namespace PKOpenPaths.Patches;

/// <summary>
/// Unlocks the exterior door to the Armor Shop in Forest Village (ve02) early.
/// 
/// Grounding & Mechanics:
/// In Forest Village exterior (ve02, mapeventdat[0]), the Armor Shop building adjacent to the
/// village blacksmith anvil (door ev[6] at X: 54..58, Y: 128..129) is blocked in early/mid game by:
/// - ev[4] (MapInit, Type 2): checks if Story Flag 114 Bit 4 is CLEAR ([4, 114, 4, 255, 254])
///   and executes [0, 40, 255, 254] which spawns blocker obstacle obj[40] over the doorway.
/// - ev[5] (Action/Talk, Type 1): checks if Story Flag 114 Bit 4 is CLEAR and player examines obj[40]
///   ([4, 114, 4, 5, 40, 255, 254]) and plays dialogue "Closed." ([23, 1, 3, 255, 254]),
///   intercepting the door trigger.
/// 
/// Inside the shop (interior at 46, 53):
/// - Counter obj[16] at (51, 34) runs Shop 11 (Armor Shop: Leather Armor, Chain Mail, Scale Mail).
/// - Anvil obj[27] is right outside at (60, 141).
/// - Connell is found in the corner at (41, 21).
/// 
/// Story Flag 114 Bit 4 is normally set late in the game after the Greenhill liberation council meeting
/// in Castle HQ (vk10). When exploring Forest Village earlier (e.g. via the Matilda border glitch/pass),
/// the shop door is permanently blocked with "Closed.".
/// 
/// By hooking MACHICON.EventInit (HarmonyPrefix) on map ve02, we neutralize ev[4] and ev[5] while
/// strictly preserving exact byte lengths matching vanilla JSON. This allows door ev[6] to warp directly
/// into the Armor Shop interior at (46, 53) at any point in the game.
/// </summary>
public static class ForestVillageArmorShopPatch
{
    public static void Initialize(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(EventInitPatch));
            Plugin.Log.LogInfo("[ForestVillageArmorShopPatch] Registered MACHICON.EventInit hook successfully.");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"[ForestVillageArmorShopPatch] Failed to register hook: {ex}");
        }
    }

    [HarmonyPatch(typeof(GSD2::MACHICON), "EventInit")]
    public static class EventInitPatch
    {
        [HarmonyPrefix]
        public static void Prefix(GSD2::MACHICON __instance)
        {
            try
            {
                if (Plugin.Config == null || !Plugin.Config.EnableForestVillageArmorShopUnlocked.Value)
                    return;

                var eventdata = __instance?.mdat?.eventdata;
                if (eventdata?.mapeventdat == null || eventdata.mapeventdat.Count == 0)
                    return;

                string asset = __instance.mdat.name ?? string.Empty;
                string raw = __instance.GetMapName() ?? string.Empty;

                bool isVe02 = asset.StartsWith("ve02", StringComparison.OrdinalIgnoreCase)
                    || raw.StartsWith("ve02", StringComparison.OrdinalIgnoreCase);

                if (!isVe02)
                    return;

                var map0 = eventdata.mapeventdat[0];
                if (map0?.eventdat == null || map0.eventdat.Length <= 5)
                    return;

                // Neutralize ev[4] (MapInit blocker spawn): exact 5 bytes preserved matching vanilla [4, 114, 4, 255, 254]
                if (map0.eventdat[4] != null)
                {
                    map0.eventdat[4].eventForm = new byte[] { 0, 0, 0, 0, 254 };
                }

                // Neutralize ev[5] (Action "Closed." trigger): exact 7 bytes preserved matching vanilla [4, 114, 4, 5, 40, 255, 254]
                if (map0.eventdat[5] != null)
                {
                    map0.eventdat[5].eventForm = new byte[] { 0, 0, 0, 0, 0, 0, 254 };
                }

                Plugin.Log.LogInfo("[ForestVillageArmorShopPatch] Forest Village Armor Shop door unlocked early in ve02 mapeventdat[0]!");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[ForestVillageArmorShopPatch] Error in MACHICON.EventInit Prefix: {ex}");
            }
        }
    }
}
