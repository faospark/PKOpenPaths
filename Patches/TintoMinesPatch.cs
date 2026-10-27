extern alias GSD2;

using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PKOpenPaths.Patches;

/// <summary>
/// Opens shortcuts in Tinto Mines and Tinto Mining City (vg02, vg15) by fully disabling
/// the blocking entities (disp: 0, ozok: 0) and force-loading 1x1 transparent textures/sprites
/// onto the HD rock props.
/// 
/// Grounding & Mechanics:
/// - vg15 (Tiger Cave / Mt. Senai / Tinto Mines): In room 0 (mapeventdat[0]), 
///   objects 3 through 12 form the rock blockage. Setting disp=0 and ozok=0
///   completely disables their 2D collision and sprites.
/// - vg02 (Tinto Mining City): In room 2 (mapeventdat[2]), objects 4 to 8 are the rock
///   cluster and objects 9 and 10 are the barrier triggers. Setting disp=0 and ozok=0
///   on 4 through 10 completely removes both the collision and 2D sprites.
/// - HD Visual Layer: In the HD Remaster, the rock blockage props are:
///   't_vg02_02_obj_rock01' (vg02) and 't_vg15_00_obj_rock01' (vg15).
///   We force their materials and SpriteRenderers to use a 1x1 transparent pixel texture,
///   zero their scale, move their transforms off-screen, and disable their GameObjects.
/// </summary>
public static class TintoMinesPatch
{
    public static string CurrentMapId = "";

    /// <summary>
    /// Exact object and texture names targeted for 1x1 transparent replacement and removal.
    /// Strictly contains only the two rock blockage objects provided.
    /// </summary>
    public static readonly HashSet<string> TargetExactObjectNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "t_vg02_02_obj_rock01",
        "t_vg15_00_obj_rock01"
    };

    private static Texture2D _transparent1x1;
    private static Sprite _transparent1x1Sprite;

    public static Texture2D GetTransparent1x1()
    {
        if (_transparent1x1 == null)
        {
            _transparent1x1 = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            _transparent1x1.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
            _transparent1x1.Apply(false, true);
            _transparent1x1.name = "Transparent1x1_OpenPaths";
            UnityEngine.Object.DontDestroyOnLoad(_transparent1x1);
        }
        return _transparent1x1;
    }

    public static Sprite GetTransparent1x1Sprite()
    {
        if (_transparent1x1Sprite == null)
        {
            var tex = GetTransparent1x1();
            _transparent1x1Sprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
            _transparent1x1Sprite.name = "Transparent1x1Sprite_OpenPaths";
            UnityEngine.Object.DontDestroyOnLoad(_transparent1x1Sprite);
        }
        return _transparent1x1Sprite;
    }

    public static void Initialize(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(EventInitPatch));
            harmony.PatchAll(typeof(OnMapInitPatch));
            harmony.PatchAll(typeof(SetMapPatch));
            harmony.PatchAll(typeof(MapSpriteHDAwakePatch));
            Plugin.Log.LogInfo("[TintoMinesPatch] Registered EventInit, OnMapInit, SetMap & MapSpriteHD hooks successfully.");
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"[TintoMinesPatch] Failed to register hook: {ex}");
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
                if (Plugin.Config == null || !Plugin.Config.EnableTintoMinesShortcuts.Value)
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

                CurrentMapId = mapId ?? "";

                if (string.IsNullOrEmpty(mapId))
                    return;

                var mapEvents = __result.mdat.eventdata?.mapeventdat;
                if (mapEvents == null)
                    return;

                // Handle vg15 (Tinto Mines tunnel rock blockage) - Full disable of objects 3 to 12
                if (mapId.StartsWith("vg15", StringComparison.OrdinalIgnoreCase))
                {
                    if (mapEvents.Count > 0 && mapEvents[0]?.eventobj != null)
                    {
                        var objs = mapEvents[0].eventobj;
                        int disabledCount = 0;
                        for (int i = 3; i <= 12 && i < objs.Length; i++)
                        {
                            if (objs[i] != null)
                            {
                                objs[i].disp = 0;
                                objs[i].ozok = 0;
                                disabledCount++;
                            }
                        }

                        Plugin.Log.LogInfo($"[TintoMinesPatch] Fully disabled {disabledCount} rock blockage objects (3-12) in map '{mapId}' (Room 0).");
                    }
                }
                // Handle vg02 (Tinto Mining City) - Full disable of rocks (4-8) AND barriers (9-10) in Room 2
                else if (mapId.StartsWith("vg02", StringComparison.OrdinalIgnoreCase))
                {
                    if (mapEvents.Count > 2 && mapEvents[2]?.eventobj != null)
                    {
                        var objs = mapEvents[2].eventobj;
                        int disabledCount = 0;
                        for (int i = 4; i <= 10 && i < objs.Length; i++)
                        {
                            if (objs[i] != null)
                            {
                                objs[i].disp = 0;
                                objs[i].ozok = 0;
                                disabledCount++;
                            }
                        }

                        Plugin.Log.LogInfo($"[TintoMinesPatch] Fully disabled {disabledCount} rock and barrier objects (4-10) in map '{mapId}' (Room 2).");
                    }
                }

                CleanHdRockVisuals();
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[TintoMinesPatch] Error applying Tinto Mines 2D shortcuts: {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(GSD2::MACHICON), "OnMapInit")]
    public static class OnMapInitPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            CleanHdRockVisuals();
        }
    }

    [HarmonyPatch(typeof(GSD2::MapBGManagerHD), "SetMap")]
    public static class SetMapPatch
    {
        [HarmonyPostfix]
        public static void Postfix(GSD2::MapBGManagerHD __instance, GSD2::MAP map, GameObject obj)
        {
            CleanHdRockVisuals();
        }
    }

    /// <summary>
    /// Intercepts Awake on MapSpriteHD components to force 1x1 transparent texture immediately.
    /// </summary>
    [HarmonyPatch(typeof(MapSpriteHD), "Awake")]
    public static class MapSpriteHDAwakePatch
    {
        [HarmonyPostfix]
        public static void Postfix(MapSpriteHD __instance)
        {
            try
            {
                if (Plugin.Config == null || !Plugin.Config.EnableTintoMinesShortcuts.Value)
                    return;

                if (__instance == null || __instance.gameObject == null)
                    return;

                ForceTransparent1x1(__instance.gameObject);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[TintoMinesPatch] Error in MapSpriteHDAwakePatch: {ex}");
            }
        }
    }

    /// <summary>
    /// Normalizes a GameObject or texture name by stripping Unity instance duplication suffixes
    /// such as '(1)', '(2)', '(Clone)' and surrounding whitespace.
    /// </summary>
    public static string NormalizeGameObjectName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";
        string clean = System.Text.RegularExpressions.Regex.Replace(name, @"\s*\(\d+\)", "");
        clean = clean.Replace("(Clone)", "").Trim();
        return clean;
    }

    /// <summary>
    /// Force-loads a 1x1 transparent pixel texture and sprite onto the target GameObject,
    /// scales it to zero, moves its coordinates off-screen, and disables it.
    /// </summary>
    public static bool ForceTransparent1x1(GameObject go)
    {
        if (go == null) return false;

        string cleanName = NormalizeGameObjectName(go.name);
        bool match = TargetExactObjectNames.Contains(cleanName);

        var renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (r == null) continue;

            if (!match && r.sharedMaterial != null && r.sharedMaterial.mainTexture != null)
            {
                string texClean = NormalizeGameObjectName(r.sharedMaterial.mainTexture.name);
                if (TargetExactObjectNames.Contains(texClean))
                    match = true;
            }

            if (!match && r is SpriteRenderer srCheck && srCheck.sprite != null)
            {
                string spriteClean = NormalizeGameObjectName(srCheck.sprite.name);
                string sprTexClean = srCheck.sprite.texture != null ? NormalizeGameObjectName(srCheck.sprite.texture.name) : "";
                if (TargetExactObjectNames.Contains(spriteClean) || (!string.IsNullOrEmpty(sprTexClean) && TargetExactObjectNames.Contains(sprTexClean)))
                {
                    match = true;
                }
            }

            if (match)
            {
                // Force material mainTexture to 1x1 transparent texture
                if (r.material != null)
                    r.material.mainTexture = GetTransparent1x1();
                if (r.sharedMaterial != null)
                    r.sharedMaterial.mainTexture = GetTransparent1x1();

                // Force SpriteRenderer to 1x1 transparent sprite
                if (r is SpriteRenderer sr)
                {
                    sr.sprite = GetTransparent1x1Sprite();
                    sr.color = Color.clear;
                }

                r.enabled = false;
            }
        }

        if (match)
        {
            go.transform.position = new Vector3(99999f, 99999f, 99999f);
            go.transform.localScale = Vector3.zero;
            go.SetActive(false);
            Plugin.Log.LogInfo($"[TintoMinesPatch] Force-loaded 1x1 transparent texture and disabled '{go.name}' (normalized: '{cleanName}')");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Scans active renderers in the map scene once upon loading to hide target rock visuals.
    /// </summary>
    public static void CleanHdRockVisuals()
    {
        try
        {
            if (Plugin.Config == null || !Plugin.Config.EnableTintoMinesShortcuts.Value)
                return;

            bool isTargetMap = CurrentMapId.StartsWith("vg15", StringComparison.OrdinalIgnoreCase) ||
                               CurrentMapId.StartsWith("vg02", StringComparison.OrdinalIgnoreCase);

            if (!isTargetMap)
                return;

            // Check all MapSpriteHD in the scene
            var allMapSprites = UnityEngine.Object.FindObjectsOfType<MapSpriteHD>();
            if (allMapSprites != null)
            {
                for (int i = 0; i < allMapSprites.Length; i++)
                {
                    if (allMapSprites[i] != null && allMapSprites[i].gameObject != null)
                    {
                        ForceTransparent1x1(allMapSprites[i].gameObject);
                    }
                }
            }

            // Check all Renderers in the active scene
            var renderers = UnityEngine.Object.FindObjectsOfType<Renderer>();
            if (renderers != null && renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] != null && renderers[i].gameObject != null)
                    {
                        ForceTransparent1x1(renderers[i].gameObject);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"[TintoMinesPatch] Error in CleanHdRockVisuals: {ex}");
        }
    }
}
