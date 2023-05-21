using SadConsole.Input;
using SadConsole;
using SadRogue.Integration.Components;
using System;
using SadRogue.Primitives;

namespace SadRogueExample.Screens.Components;

/// <summary>
/// The look marker's position, relative to the map and the surface.
/// </summary>
/// <param name="SurfacePosition">The look marker's position relative to the surface it's a child of.</param>
/// <param name="MapPosition">The look marker's position on the map being rendered.</param>
internal record struct LookMarkerPosition(Point SurfacePosition, Point MapPosition);

/// <summary>
/// Arguments for the <see cref="SelectMapLocation.PositionChanged"/> and <see cref="SelectMapLocation.PositionSelected"/> event.
/// </summary>
internal class LookMarkerPositionEventArgs : EventArgs
{
    public readonly LookMarkerPosition Position;

    public LookMarkerPositionEventArgs(LookMarkerPosition position)
    {
        Position = position;
    }
}

/// <summary>
/// Component which can be used to let the user select a location on the map by moving the mouse or using keyboard
/// motions.  This component should be added to a map renderer directly.
/// </summary>
internal class SelectMapLocation : RogueLikeComponentBase<IScreenSurface>
{
    /// <summary>
    /// The surface used to display where the user is looking.
    /// </summary>
    public readonly ScreenSurface LookMarker;

    /// <summary>
    /// The position of the look marker, both in terms of the map's coordinate space, and the parent surface's.
    /// </summary>
    public LookMarkerPosition LookMarkerPosition { get; private set; }

    /// <summary>
    /// Fired when the position of the look marker changes.
    /// </summary>
    public event EventHandler<LookMarkerPositionEventArgs>? PositionChanged;

    /// <summary>
    /// Fired when the user selects a position by left-clicking or pressing enter.
    /// </summary>
    public event EventHandler<LookMarkerPositionEventArgs>? PositionSelected;

    /// <summary>
    /// Fired when the user cancels the selection by pressing escape.
    /// </summary>
    public event EventHandler? SelectionCancelled;

    /// <summary>
    /// Keybindings component used to handle keyboard input controls for the look marker.
    /// </summary>
    public readonly SelectLocationKeybindingsComponent Keybindings;

    private readonly Func<Point> _getLookMarkerSurfaceStartingLocation;
    private Point _lastMousePosition = Point.None;

    private readonly Color _lookMarkerColor = new(200, 0, 0, 170);

    public SelectMapLocation(Func<Point> getLookMarkerSurfaceStartingLocation)
        : base(false, false, true, false)
    {
        // Create the console we'll use as the marker to show where the user is looking.
        LookMarker = new(1, 1);
        LookMarker.Surface.Fill(Color.Transparent, _lookMarkerColor, 0);
        LookMarkerPosition = new(Point.Zero, Point.Zero);

        // Update the marker's position relative to the surface and map when the mouse moves
        LookMarker.PositionChanged += LookMarker_PositionChanged;

        // Create the keybindings component we'll use to handle input
        Keybindings = new(LookMarker);
        Keybindings.SetAction(Keys.Enter, OnPositionSelected);
        Keybindings.SetAction(Keys.Escape, () => SelectionCancelled?.Invoke(this, EventArgs.Empty));

        // Record a function to use to get the look marker's starting location when this component is activated
        _getLookMarkerSurfaceStartingLocation = getLookMarkerSurfaceStartingLocation;
    }

    public override void OnAdded(IScreenObject host)
    {
        base.OnAdded(host);
        host.Children.Add(LookMarker);
        host.SadComponents.Add(Keybindings);

        // Update the look marker's position to the starting location
        LookMarker.Position = _getLookMarkerSurfaceStartingLocation();
    }

    public override void OnRemoved(IScreenObject host)
    {
        base.OnRemoved(host);
        host.Children.Remove(LookMarker);
        host.SadComponents.Remove(Keybindings);
    }

    public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
    {
        handled = true;

        // First frame, we will not actually change the look marker position; it will start on the player or some other target, and we want to keep
        // it that way until the user moves the mouse
        if (_lastMousePosition == Point.None)
        {
            _lastMousePosition = state.SurfaceCellPosition;
            return;
        }

        // If the user has clicked the mouse button or has moved the mouse, update the position of the look marker and trigger the selected event.
        // We want to ensure we update the position _always_ if the user has clicked the mouse, so that the look marker is correct before we select
        // a position.
        //
        // Note that this logic may very well break if the user moves the viewport during the selection; that is impossible for our current use cases,
        // however.
        if (state.SurfaceCellPosition != _lastMousePosition)
        {
            // Set the look marker's position to the mouse position.  We use SurfaceCellPosition instead of CellPosition
            // because we need the position relative to viewport/position, not position on the map.
            _lastMousePosition = state.SurfaceCellPosition;
            LookMarker.Position = state.SurfaceCellPosition;
        }

        // Trigger selection on left click
        if (state.Mouse.LeftClicked)
            OnPositionSelected();
    }

    private void OnPositionSelected()
    {
        PositionSelected?.Invoke(this, new(LookMarkerPosition));
    }

    private void LookMarker_PositionChanged(object? sender, SadConsole.ValueChangedEventArgs<Point> e)
    {
        // The entity position is in surface position; so we calculate the map position based on that
        LookMarkerPosition = new(e.NewValue, e.NewValue + (Parent?.Surface.ViewPosition ?? Point.Zero));
        PositionChanged?.Invoke(this, new(LookMarkerPosition));
    }
}

