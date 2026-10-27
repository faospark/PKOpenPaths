extern alias GSD2;

using System;
using HarmonyLib;

namespace PKOpenPaths.Patches;

/// <summary>
/// Pre-positions the puzzle boulders in Cave of the Wind (vd06) into their wind-blocking coordinates.
/// 
/// Grounding & Mechanics:
/// Map asset vd06 contains subterranean rooms 8, 9, 10, and 11 which have wind gusts
/// obstructing progress until boulders (ano=6, otyp=3) are pushed into the blocking paths.
/// By intercepting MACHICON.EventInit, this patch sets the boulders to their solved positions
/// in memory upon map load, removing the need for manual puzzle solving while preserving 
/// all event triggers, flags, and game updates.
/// </summary>
public static class CaveOfTheWindPatch
{
    public static void Initialize(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(EventInitPatch));
            Plugin.Log.LogInfo("[CaveOfTheWindPatch] Registered MACHICON.EventInit hook successfully.");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"[CaveOfTheWindPatch] Failed to register hook: {ex}");
        }
    }

    [HarmonyPatch(typeof(GSD2::MACHICON), "EventInit")]
    public static class EventInitPatch
    {
        [HarmonyPostfix]
        public static void Postfix(GSD2::MACHICON __instance, GSD2::EVENTCON __result)
        {
            try
            {
                if (Plugin.Config == null || !Plugin.Config.EnableCaveOfTheWindSolved.Value)
                    return;

                if (__result == null || __result.mdat == null)
                    return;

                string mapId = __result.mdat.name;
                if (string.IsNullOrEmpty(mapId) && __instance != null)
                {
                    string raw = __instance.GetMapName();
                    if (!string.IsNullOrEmpty(raw))
                    {
                        mapId = raw.Split(' ')[0];
                    }
                }

                if (string.IsNullOrEmpty(mapId) || !mapId.StartsWith("vd06", StringComparison.OrdinalIgnoreCase))
                    return;

                var mapEvents = __result.mdat.eventdata?.mapeventdat;
                if (mapEvents == null)
                    return;

                int modifiedCount = 0;

                // Submap vd06_08 (Room 8)
                if (mapEvents.Count > 8 && mapEvents[8]?.eventobj != null)
                {
                    var objs = mapEvents[8].eventobj;
                    if (objs.Length > 5)
                    {
                        SetBoulderPosition(objs[1], 26, 23, ref modifiedCount);
                        SetBoulderPosition(objs[2], 14, 23, ref modifiedCount);
                        SetBoulderPosition(objs[3], 26, 34, ref modifiedCount);
                        SetBoulderPosition(objs[4], 20, 34, ref modifiedCount);
                        SetBoulderPosition(objs[5], 14, 34, ref modifiedCount);
                    }
                }

                // Submap vd06_09 (Room 9)
                if (mapEvents.Count > 9 && mapEvents[9]?.eventobj != null)
                {
                    var objs = mapEvents[9].eventobj;
                    if (objs.Length > 1)
                    {
                        SetBoulderPosition(objs[1], 16, 22, ref modifiedCount);
                    }
                }

                // Submap vd06_10 (Room 10)
                if (mapEvents.Count > 10 && mapEvents[10]?.eventobj != null)
                {
                    var objs = mapEvents[10].eventobj;
                    if (objs.Length > 2)
                    {
                        SetBoulderPosition(objs[1], 20, 22, ref modifiedCount);
                        SetBoulderPosition(objs[2], 15, 22, ref modifiedCount);
                    }
                }

                // Submap vd06_11 (Room 11)
                if (mapEvents.Count > 11 && mapEvents[11]?.eventobj != null)
                {
                    var objs = mapEvents[11].eventobj;
                    if (objs.Length > 4)
                    {
                        SetBoulderPosition(objs[1], 72, 22, ref modifiedCount);
                        SetBoulderPosition(objs[2], 64, 22, ref modifiedCount);
                        SetBoulderPosition(objs[3], 68, 22, ref modifiedCount);
                        SetBoulderPosition(objs[4], 60, 22, ref modifiedCount);
                    }
                }

                if (modifiedCount > 0)
                {
                    Plugin.Log.LogInfo($"[CaveOfTheWindPatch] Applied Cave of the Wind solved positions to {modifiedCount} boulders on map '{mapId}'.");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[CaveOfTheWindPatch] Error applying boulder positions: {ex}");
            }
        }

        private static void SetBoulderPosition(GSD2::EVENT_OBJ obj, int x, int y, ref int count)
        {
            if (obj != null)
            {
                obj.x = x;
                obj.y = y;
                count++;
            }
        }
    }
}

