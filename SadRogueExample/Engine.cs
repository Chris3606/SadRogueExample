using SadConsole;
using SadRogue.Integration;
using SadRogueExample.Screens;

namespace SadRogueExample;

/// <summary>
/// Main class containing the program's entry point, which runs the game's core loop.
/// </summary>
internal static class Engine
{
    // Window width/height
    public const int ScreenWidth = 80;
    public const int ScreenHeight = 50;

    private static MainGame? _gameScreen;

    public static MainGame? GameScreen
    {
        get => _gameScreen;
        set
        {
            if (_gameScreen == value) return;
            
            _gameScreen?.Uninitialize();
            _gameScreen = value;
        }
    }

    // Null override because it's initialized via Init
    public static MessageLog MessageLog = null!;

    // Null override because it's initialized via new-game/load game
    public static RogueLikeEntity Player = null!;

    private static void Main()
    {
        Game.Create(ScreenWidth, ScreenHeight, "Fonts/Cheepicus12.font");
        Game.Instance.OnStart = Init;
        Game.Instance.Run();
        Game.Instance.Dispose();
    }

    private static void Init()
    {
        MessageLog = new MessageLog(1000);

        // Main menu
        GameHost.Instance.Screen = new MainMenu();

        // Destroy the default starting console that SadConsole created automatically because we're not using it.
        GameHost.Instance.DestroyDefaultStartingConsole();
    }
}