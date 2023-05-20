using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadConsole.Components;
using SadConsole.Entities;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Integration;
using SadRogue.Primitives;
using SadRogueExample.MapObjects.Components;
using SadRogueExample.Maps;
using SadRogueExample.Screens.MainGameMenus;
using SadRogueExample.Screens.Surfaces;
using SadRogueExample.Themes;
using StatusPanel = SadRogueExample.Screens.Surfaces.StatusPanel;

namespace SadRogueExample.Screens;

/// <summary>
/// Component which causes the LookMarker in a GameScreen to follow the mouse and generates the appropriate display text.
/// </summary>
internal class LookModeFollowMouseComponent : MouseConsoleComponent
{
    private readonly MainGame _gameScreen;
    public LookModeFollowMouseComponent(MainGame gameScreen)
    {
        _gameScreen = gameScreen;
    }

    public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, [UnscopedRef] out bool handled)
    {
        // Set the look marker's position to the mouse position.  We use SurfaceCellPosition instead of CellPosition
        // because we need the position relative to viewport/position, not position on the map.
        _gameScreen.LookMarker.Position = state.SurfaceCellPosition;
        
        // Generate the text to display in the status panel.  Here, we need the CellPosition because we need to check the actual map position.
        var entityName = "You see " + (_gameScreen.Map.GetEntityAt<RogueLikeEntity>(state.CellPosition)?.Name ?? "nothing here.");
        _gameScreen.StatusPanel.LookInfo.DisplayText = entityName;

        handled = true;
    }
}

/// <summary>
/// Main game screen that shows map, message log, etc.
/// </summary>
internal class MainGame : ScreenObject
{
    public readonly GameMap Map;
    public readonly MessageLogConsole MessageLog;
    public readonly StatusPanel StatusPanel;
    public readonly ScreenSurface LookMarker;

    /// <summary>
    /// Component which locks the map's view onto an entity (usually the player).
    /// </summary>
    public readonly SurfaceComponentFollowTarget ViewLock;

    private bool _lookMode;
    public bool LookMode
    {
        get => _lookMode;
        set
        {
            if (_lookMode == value) return;

            _lookMode = value;
            if (_lookMode)
            {
                // Add the marker as a child, and add the follow component to the renderer
                Children.Add(LookMarker);
                Map.DefaultRenderer?.SadComponents.Add(_lookModeFollowComponent);
            }
            else
            {
                // Remove the marker as a child, remove the follow component from the renderer, and erase the status panel text
                Children.Remove(LookMarker);
                Map.DefaultRenderer?.SadComponents.Remove(_lookModeFollowComponent);
                StatusPanel.LookInfo.DisplayText = "";
            }
        }
    }

    private readonly LookModeFollowMouseComponent _lookModeFollowComponent;
    private readonly CustomPlayerKeybindingsComponent _keybindings;

    private const int StatusBarWidth = 25;
    private const int BottomPanelHeight = 5;
    private readonly Color _lookMarkerColor = new(200, 0, 0, 170);

    public MainGame(GameMap map)
    {
        // Record the map we're rendering
        Map = map;

        // Create a renderer for the map, specifying viewport size.
        Map.DefaultRenderer = Map.CreateRenderer((Engine.ScreenWidth, Engine.ScreenHeight - BottomPanelHeight));

        // Make the Map (which is also a screen object) a child of this screen, and ensure it receives focus.
        Map.Parent = this;
        Map.IsFocused = true;

        // Center view on player as they move (by default)
        ViewLock = new SurfaceComponentFollowTarget { Target = Engine.Player };
        Map.DefaultRenderer?.SadComponents.Add(ViewLock);

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

        // Create the look marker, but don't add it as a child yet so it doesn't display until we're in look mode
        LookMarker = new(1, 1)
        {
            Position = (5, 5),
        };
        LookMarker.Surface.Fill(Color.Transparent, _lookMarkerColor, 0);

        // Create component which will have the LookMarker follow the mouse in look mode (but do not attach it yet)
        _lookModeFollowComponent = new LookModeFollowMouseComponent(this);

        // Cache the keybindings component for convenience, since this screen will add some bindings and the player will not change during the lifetime
        // of this screen.
        _keybindings = Engine.Player.AllComponents.GetFirst<CustomPlayerKeybindingsComponent>();

        // Set up screen-specific keybindings
        SetKeybindings();

        // Add player death handler
        Engine.Player.AllComponents.GetFirst<Combatant>().Died += PlayerDeath;

        // Write welcome message
        MessageLog.AddMessage(new("Hello and welcome, adventurer, to yet another dungeon!", MessageColors.WelcomeTextAppearance));
    }

    /// <summary>
    /// Sets various keybindings that this screen adds to the player's keybindings component.
    /// </summary>
    public void SetKeybindings()
    {
        // "Look" functionality keybindings
        _keybindings.SetAction(SadConsole.Input.Keys.L, () => LookMode = true);
        _keybindings.SetAction(SadConsole.Input.Keys.Escape, () => LookMode = false);
    }

    /// <summary>
    /// Removes keybindings set when the screen was created from the player's keybindings component.
    /// </summary>
    public void RemoveKeybindings()
    {
        _keybindings.RemoveAction(SadConsole.Input.Keys.L);
        _keybindings.RemoveAction(SadConsole.Input.Keys.Escape);
    }

    /// <summary>
    /// Called when the player dies.
    /// </summary>
    private void PlayerDeath(object? s, EventArgs e)
    {
        MessageLog.AddMessage(new("You have died!", MessageColors.PlayerDiedAppearance));

        Engine.Player.AllComponents.GetFirst<Combatant>().Died -= PlayerDeath;

        // Switch to game over screen
        Children.Add(new GameOver());

    }
}