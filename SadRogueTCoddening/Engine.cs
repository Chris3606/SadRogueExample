using SadConsole;
using SadRogue.Integration;
using SadRogueTCoddening.Screens;

namespace SadRogueTCoddening;

/// <summary>
/// Main class containing the program's entry point, which runs the game's core loop.
/// </summary>
internal static class Engine
{
    public static MainGame? GameScreen;
        
    // Null override because it's initialized via new-game/load game
    public static RogueLikeEntity Player = null!;

    private static void Main()
    {
        Game.Create(Constants.ScreenWidth, Constants.ScreenHeight, "Fonts/Cheepicus12.font");
        Game.Instance.OnStart = Init;
        Game.Instance.Run();
        Game.Instance.Dispose();
    }

    private static void Init()
    {
        // Main menu
        GameHost.Instance.Screen = new MainMenu();

        // Destroy the default starting console that SadConsole created automatically because we're not using it.
        GameHost.Instance.DestroyDefaultStartingConsole();
    }
}