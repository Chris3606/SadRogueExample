using System;
using System.Diagnostics.CodeAnalysis;
using SadConsole;
using SadConsole.Components;
using SadRogue.Primitives;
using SadRogueExample.MapObjects.Components;
using SadRogueExample.Maps;
using SadRogueExample.Screens.Surfaces;
using SadRogueExample.Themes;
using StatusPanel = SadRogueExample.Screens.Surfaces.StatusPanel;

namespace SadRogueExample.Screens;

/// <summary>
/// Main game screen that shows map, message log, etc.
/// </summary>
internal class MainGame : ScreenObject
{
    public GameMap Map { get; private set; }
    public readonly MessageLogConsole MessageLog;
    public readonly StatusPanel StatusPanel;

    /// <summary>
    /// Component which locks the map's view onto an entity (usually the player).
    /// </summary>
    public readonly SurfaceComponentFollowTarget ViewLock;

    private const int StatusBarWidth = 20;
    const int BottomPanelHeight = 5;

    public MainGame(GameMap map, Point playerSpawn)
    {
        // Center view on player as they move (by default)
        ViewLock = new SurfaceComponentFollowTarget { Target = Engine.Player };
        
        // Set up the map to render
        SetMap(map, playerSpawn);

        // Create message log
        MessageLog = new MessageLogConsole(Engine.ScreenWidth - StatusBarWidth - 1, BottomPanelHeight)
        {
            Parent = this,
            Position = new(StatusBarWidth + 1, Engine.ScreenHeight - BottomPanelHeight)
        };

        // Create status panel
        StatusPanel = new(StatusBarWidth, BottomPanelHeight)
        {
            Parent = this,
            Position = new(0, Engine.ScreenHeight - BottomPanelHeight)
        };

        // Add player death handler
        Engine.Player.AllComponents.GetFirst<Combatant>().Died += PlayerDeath;

        // Write welcome message
        MessageLog.AddMessage(new("Hello and welcome, adventurer, to yet another dungeon!", MessageColors.WelcomeTextAppearance));
    }

    [MemberNotNull(nameof(Map))]
    public void SetMap(GameMap map, Point playerSpawn)
    {
        // Remove player from its current map, if any
        Engine.Player.CurrentMap?.RemoveEntity(Engine.Player);

        if (Map != null)
        {
            // Remove view centering component from old map, if any
            Map.SadComponents.Remove(ViewLock);
            Map.Parent = null;
        }
        
        // Set new map to render
        Map = map;
        
        // Create a renderer for the map, specifying viewport size.
        Map.DefaultRenderer = Map.CreateRenderer((Engine.ScreenWidth, Engine.ScreenHeight - BottomPanelHeight));
        
        // Make the Map (which is also a screen object) a child of this screen, and ensure it receives focus.
        Map.Parent = this;
        Map.IsFocused = true;

        // Spawn the player on the new map
        Engine.Player.Position = playerSpawn;
        Map.AddEntity(Engine.Player);

        // Center view on player as they move (by default)
        Map.DefaultRenderer?.SadComponents.Add(ViewLock);
    }

    /// <summary>
    /// Called when the player dies.
    /// </summary>
    private static void PlayerDeath(object? s, EventArgs e)
    {
        Engine.Player.AllComponents.GetFirst<Combatant>().Died -= PlayerDeath;
        // Go back to main menu for now
        Game.Instance.Screen = new MainMenu();

    }
}