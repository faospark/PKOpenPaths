using BepInEx.Configuration;

namespace PKOpenPaths;

public sealed class ModConfiguration
{
    private readonly ConfigFile _config;

    // Suikoden 2 Cave of the Wind Puzzle Boulders
    public ConfigEntry<bool> EnableCaveOfTheWindSolved { get; private set; }

    // Suikoden 2 Tinto Mines Shortcuts
    public ConfigEntry<bool> EnableTintoMinesShortcuts { get; private set; }

    public ModConfiguration(ConfigFile config)
    {
        _config = config;
    }

    public void Init()
    {
        EnableCaveOfTheWindSolved = _config.Bind(
            "Suikoden 2 Cave of the Wind",
            "EnableCaveOfTheWindSolved",
            true,
            "Automatically places the puzzle boulders in Cave of the Wind (vd06) into their wind-blocking positions, allowing immediate passage without pushing boulders."
        );

        EnableTintoMinesShortcuts = _config.Bind(
            "Suikoden 2 Tinto Mines",
            "EnableTintoMinesShortcuts",
            true,
            "Removes rock blockages and barrier triggers in Tinto Mines (vg02, vg15) to open convenient shortcuts."
        );
    }
}

