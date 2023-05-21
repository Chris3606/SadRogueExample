using System;
using System.Diagnostics.CodeAnalysis;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Integration.Keybindings;
using SadRogue.Integration.Maps;
using SadRogue.Primitives;
using SadRogueExample.MapObjects.Components;
using SadRogueExample.Maps;
using SadRogueExample.Screens.MainGameMenus;
using SadRogueExample.Screens.Surfaces;
using SadRogueExample.Themes;
using StatusPanel = SadRogueExample.Screens.Surfaces.StatusPanel;

namespace SadRogueExample.Screens;

internal record struct LookMarkerPosition(Point SurfacePosition, Point MapPosition);

internal class LookMarkerPositionChangedEventArgs : EventArgs
{
    public readonly LookMarkerPosition NewPosition;

    public LookMarkerPositionChangedEventArgs(LookMarkerPosition newPosition)
    {
        NewPosition = newPosition;
    }
}

/// <summary>
/// Component which can be used to let the user select a location on the map by moving the mouse.
/// This component should be added to a map renderer directly.
/// </summary>
internal class SelectLocationComponent : RogueLikeComponentBase<IScreenSurface>
{
    public readonly ScreenSurface LookMarker;
    public LookMarkerPosition LookMarkerPosition { get; private set; }
    public event EventHandler<LookMarkerPositionChangedEventArgs>? LookMarkerPositionChanged;

    private readonly Color _lookMarkerColor = new(200, 0, 0, 170);


    public SelectLocationComponent()
        : base(false, false, false, true)
    {
        // Create the console we'll use as the marker to show where the user is looking.
        LookMarker = new(1, 1);
        LookMarker.Surface.Fill(Color.Transparent, _lookMarkerColor, 0);
        LookMarkerPosition = new(Point.Zero, Point.Zero);

        // Update the marker's position relative to the surface and map when the mouse moves
        LookMarker.PositionChanged += LookMarker_PositionChanged;
    }

    public override void OnAdded(IScreenObject host)
    {
        base.OnAdded(host);
        host.Children.Add(LookMarker);
    }

    public override void OnRemoved(IScreenObject host)
    {
        base.OnRemoved(host);
        host.Children.Remove(LookMarker);
    }

    public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, [UnscopedRef] out bool handled)
    {
        // Set the look marker's position to the mouse position.  We use SurfaceCellPosition instead of CellPosition
        // because we need the position relative to viewport/position, not position on the map.
        LookMarker.Position = state.SurfaceCellPosition;
        
        // Generate the text to display in the status panel.  Here, we need the CellPosition because we need to check the actual map position.
        //var entityName = "You see " + (_gameScreen.Map.GetEntityAt<RogueLikeEntity>(state.CellPosition)?.Name ?? "nothing here.");
        //_gameScreen.StatusPanel.LookInfo.DisplayText = entityName;

        handled = true;

        // TODO: Handle click
    }

    private void LookMarker_PositionChanged(object? sender, SadConsole.ValueChangedEventArgs<Point> e)
    {
        // The entity position is in surface position; so we calculate the map position based on that
        LookMarkerPosition = new(e.NewValue, e.NewValue + (Parent?.Surface.ViewPosition ?? Point.Zero));
        LookMarkerPositionChanged?.Invoke(this, new LookMarkerPositionChangedEventArgs(LookMarkerPosition));
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
    //public readonly ScreenSurface LookMarker;

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
    public readonly CustomKeybindingsComponent Keybindings;

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

        // Create the Keybindings component which implements main actions/player movement.
        Keybindings = new CustomKeybindingsComponent();
        Map.SadComponents.Add(Keybindings);

        // Set up Keybindings and motions
        SetKeybindings();

        // Add player death handler
        Engine.Player.AllComponents.GetFirst<Combatant>().Died += PlayerDeath;

        // Write welcome message
        MessageLog.AddMessage(new("Hello and welcome, adventurer, to yet another dungeon!", MessageColors.WelcomeTextAppearance));
    }

    private void SetKeybindings()
    {
        // Add Keybindings controlling player movement via keyboard.
        Keybindings.SetMotions(PlayerKeybindingsComponent.ArrowMotions);
        Keybindings.SetMotions(PlayerKeybindingsComponent.NumPadAllMotions);
        Keybindings.SetMotion(SadConsole.Input.Keys.NumPad5, Direction.None);
        Keybindings.SetMotion(SadConsole.Input.Keys.OemPeriod, Direction.None);

        // Add controls for picking up items and getting to inventory screen.
        Keybindings.SetAction(SadConsole.Input.Keys.G, () => PlayerActionHelper.PlayerTakeAction(e => e.AllComponents.GetFirst<Inventory>().PickUp()));
        Keybindings.SetAction(SadConsole.Input.Keys.C, () => Children.Add(new ConsumableSelect()));
            
        // "Look" functionality Keybindings
        Keybindings.SetAction(SadConsole.Input.Keys.L, () => LookMode = true);
        Keybindings.SetAction(SadConsole.Input.Keys.Escape, () => LookMode = false);
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