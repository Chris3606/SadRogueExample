﻿using SadConsole;
using SadConsole.UI;
using SadRogue.Primitives;
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
    public readonly ControlsConsole StatusPanel;
    
    public readonly SadConsole.Components.SurfaceComponentFollowTarget ViewLock;

    private const int StatusBarWidth = 20;
    const int BottomPanelHeight = 5;

    public MainGame(GameMap map)
    {
        // Record the map we're rendering
        Map = map;

        // Create a renderer for the map, specifying viewport size.
        Map.DefaultRenderer = Map.CreateRenderer((Constants.ScreenWidth, Constants.ScreenHeight - BottomPanelHeight));

        // Make the Map (which is also a screen object) a child of this screen, and ensure it receives focus.
        Map.Parent = this;
        Map.IsFocused = true;

        // Center view on player as they move (by default)
        ViewLock = new SadConsole.Components.SurfaceComponentFollowTarget { Target = Engine.Player };
        Map.DefaultRenderer?.SadComponents.Add(ViewLock);

        // Create message log
        MessageLog = new MessageLogConsole(Constants.ScreenWidth - StatusBarWidth - 1, BottomPanelHeight);
        MessageLog.Parent = this;
        MessageLog.Position = new(StatusBarWidth + 1, Constants.ScreenHeight - BottomPanelHeight);
        
        // Create status panel
        StatusPanel = new ControlsConsole(StatusBarWidth, BottomPanelHeight);
        StatusPanel.Parent = this;
        StatusPanel.Position = new(0, Constants.ScreenHeight - BottomPanelHeight);
        StatusPanel.FillWithRandomGarbage(255);
        
        // Write welcome message
        MessageLog.AddMessage(new("Hello and welcome, adventurer, to yet another dungeon!", Constants.WelcomeTextColor, Color.Transparent));
    }
}