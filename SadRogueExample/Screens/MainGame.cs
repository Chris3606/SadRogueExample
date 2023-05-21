using System;
using System.Collections.Generic;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole.UI.Controls;
using SadRogue.Integration;
using SadRogue.Integration.Keybindings;
using SadRogue.Integration.Maps;
using SadRogue.Primitives;
using SadRogueExample.MapObjects.Components;
using SadRogueExample.Maps;
using SadRogueExample.Screens.Components;
using SadRogueExample.Screens.MainGameMenus;
using SadRogueExample.Screens.Surfaces;
using SadRogueExample.Themes;
using StatusPanel = SadRogueExample.Screens.Surfaces.StatusPanel;

namespace SadRogueExample.Screens;

internal class LookModeComponent : SelectMapLocation
{
    private readonly Label _lookLabel;
    private readonly RogueLikeMap _map;

    public LookModeComponent(Func<Point> getLookMarkerSurfaceStartingLocation, RogueLikeMap map, Label lookLabel)
        :base(getLookMarkerSurfaceStartingLocation)
    {
        _lookLabel = lookLabel;
        _map = map;

        PositionChanged += LookModeComponent_PositionChanged;
    }

    private void LookModeComponent_PositionChanged(object? sender, LookMarkerPositionEventArgs e)
    {
        // Generate the text to display in the status panel.
        var entityName = "You see " + (_map.GetEntityAt<RogueLikeEntity>(e.Position.MapPosition)?.Name ?? "nothing here.");
        _lookLabel.DisplayText = entityName;
    }

    public override void OnRemoved(IScreenObject host)
    {
        base.OnRemoved(host);
        _lookLabel.DisplayText = "";
    }
}

/// <summary>
/// Main game screen that shows map, message log, etc.  It has a number of states, each of which has its own
/// component that is added to the map's renderer when that state is active.  This component implements controls
/// and relevant logic for each state.
/// </summary>
internal class MainGame : ScreenObject
{
    public enum State
    {
        /// <summary>
        /// When this state is active, the map is displayed and the player can move around.
        /// </summary>
        MainMap,
        /// <summary>
        /// The player is in the process of looking around using the "look mode".
        /// </summary>
        LookMode
    }

    public readonly GameMap Map;
    public readonly MessageLogConsole MessageLog;
    public readonly StatusPanel StatusPanel;

    /// <summary>
    /// Component which locks the map's view onto an entity (usually the player).
    /// </summary>
    public readonly SurfaceComponentFollowTarget ViewLock;

    private State _currentState;
    public State CurrentState
    {
        get => _currentState;
        set
        {
            if (value == _currentState) return;

            // Remove the current state's component from the renderer
            Map.DefaultRenderer!.SadComponents.Remove(_stateComponents[_currentState]);

            // Set new state
            _currentState = value;

            // Add the new state's component to the renderer
            Map.DefaultRenderer!.SadComponents.Add(_stateComponents[_currentState]);
        }
    }

    private readonly Dictionary<State, IComponent> _stateComponents;

    private const int StatusBarWidth = 25;
    private const int BottomPanelHeight = 5;

    public MainGame(GameMap map)
    {
        // Record the map we're rendering
        Map = map;

        // Create a renderer for the map, specifying viewport size.
        Map.DefaultRenderer = Map.CreateRenderer((Engine.ScreenWidth, Engine.ScreenHeight - BottomPanelHeight));

        // Make the Map (which is also a screen object) a child of this screen, and ensure the default renderer receives input focus.
        Children.Add(Map);
        Map.DefaultRenderer.IsFocused = true;

        // Center view on player as they move (by default)
        ViewLock = new SurfaceComponentFollowTarget { Target = Engine.Player };
        Map.DefaultRenderer.SadComponents.Add(ViewLock);

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

        // Create components for various states that this screen controls (but do not attach any yet)
        var mainMapComponent = new MainMapKeybindingsComponent();
        SetKeybindings(mainMapComponent);

        var lookModeComponent = new LookModeComponent(
            () => Engine.Player.Position - Map.DefaultRenderer.Surface.ViewPosition, Map, StatusPanel.LookInfo);
        lookModeComponent.SelectionCancelled += (_, _) => CurrentState = State.MainMap;

        _stateComponents = new()
        {
            // A keybindings component which implements main actions/player movement.
            { State.MainMap, mainMapComponent },
            // A component which implements keyboard and mouse controls for a look marker.
            { State.LookMode, lookModeComponent }
        };

        // Switch to main map state
        _currentState = State.MainMap;
        Map.DefaultRenderer.SadComponents.Add(_stateComponents[State.MainMap]);

        // Add player death handler
        Engine.Player.AllComponents.GetFirst<Combatant>().Died += PlayerDeath;

        // Write welcome message
        MessageLog.AddMessage(new("Hello and welcome, adventurer, to yet another dungeon!", MessageColors.WelcomeTextAppearance));
    }

    private void SetKeybindings(KeybindingsComponent<IScreenObject> component)
    {
        // Add controls for picking up items and getting to inventory screen.
        component.SetAction(Keys.G, () => PlayerActionHelper.PlayerTakeAction(e => e.AllComponents.GetFirst<Inventory>().PickUp()));
        component.SetAction(Keys.C, () => Children.Add(new ConsumableSelect()));

        // "Look" functionality Keybindings
        component.SetAction(Keys.OemQuestion, () => CurrentState = State.LookMode);
        //component.SetAction(Keys.Escape, () => LookMode = false); // TODO: Remove this
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