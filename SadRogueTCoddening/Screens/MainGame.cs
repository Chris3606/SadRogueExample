using SadConsole;
using SadRogueTCoddening.Maps;
using SadRogueTCoddening.Screens.Surfaces;

namespace SadRogueTCoddening.Screens;

/// <summary>
/// Main game screen that shows map, message log, etc.
/// </summary>
internal class MainGame : ScreenObject
{
    public readonly GameMap Map;
    public readonly MessageLogConsole MessageLog;
    public readonly SadConsole.Components.SurfaceComponentFollowTarget ViewLock;

    const int MessageLogHeight = 5;

    public MainGame(GameMap map)
    {
        // Record the map we're rendering
        Map = map;

        // Create a renderer for the map, specifying viewport size.
        Map.DefaultRenderer = Map.CreateRenderer((Constants.ScreenWidth, Constants.ScreenHeight - MessageLogHeight));

        // Make the Map (which is also a screen object) a child of this screen, and ensure it receives focus.
        Map.Parent = this;
        Map.IsFocused = true;

        // Center view on player as they move (by default)
        ViewLock = new SadConsole.Components.SurfaceComponentFollowTarget { Target = Engine.Player };
        Map.DefaultRenderer?.SadComponents.Add(ViewLock);

        // Create message log
        MessageLog = new MessageLogConsole(Constants.ScreenWidth, MessageLogHeight);
        MessageLog.Parent = this;
        MessageLog.Position = new(0, Constants.ScreenHeight - MessageLogHeight);
    }
}