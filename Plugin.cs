using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using PKOpenPaths.Patches;

namespace PKOpenPaths;

[BepInPlugin("faospark.pkopenpaths", "PK Open Paths", "1.0.0")]
[BepInDependency("faospark.pkcore", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("d3xMachina.suikoden_fix", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BasePlugin
{
    public static Plugin Instance { get; private set; }
    public static new ManualLogSource Log;
    public static new ModConfiguration Config;

    public override void Load()
    {
        Instance = this;
        Log = base.Log;

        Log.LogInfo("Loading PKOpenPaths (Open Paths & Puzzle Solver Mod)...");

        Config = new ModConfiguration(base.Config);
        Config.Init();

        Harmony harmony = new Harmony("faospark.pkopenpaths");

        // Cave of the Wind Boulder Puzzle Solver
        if (Config.EnableCaveOfTheWindSolved.Value)
        {
            Log.LogInfo("Registering Cave of the Wind puzzle solver patch...");
            CaveOfTheWindPatch.Initialize(harmony);
        }

        // Tinto Mines Shortcuts
        if (Config.EnableTintoMinesShortcuts.Value)
        {
            Log.LogInfo("Registering Tinto Mines shortcuts patch...");
            TintoMinesPatch.Initialize(harmony);
        }

        Log.LogInfo("PKOpenPaths loaded successfully!");
    }
}

