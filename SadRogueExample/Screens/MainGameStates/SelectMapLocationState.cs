﻿using SadConsole;
using SadRogueExample.Screens.Components;
using System;
using SadConsole.Input;
using SadRogue.Primitives;
using SadRogue.Integration;

namespace SadRogueExample.Screens.MainGameStates
{
    /// <summary>
    /// The look marker's position, relative to the map and the surface.
    /// </summary>
    /// <param name="SurfacePosition">The look marker's position relative to the surface it's a child of.</param>
    /// <param name="MapPosition">The look marker's position on the map being rendered.</param>
    internal record struct LookMarkerPosition(Point SurfacePosition, Point MapPosition);

    /// <summary>
    /// State where the user is looking around and selecting a location on the map.
    /// </summary>
    internal class SelectMapLocationState : StateBase
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
        /// Called when the position of the look marker changes.
        /// </summary>
        private readonly Action<LookMarkerPosition>? _positionChanged;

        /// <summary>
        /// Called when the user selects a position by left-clicking or pressing enter.
        /// </summary>
        private readonly Action<LookMarkerPosition>? _positionSelected;

        // Keybindings component used to handle keyboard input controls for the look marker.
        private readonly SelectLocationKeybindingsComponent _keybindings;
        private readonly Func<Point>? _getLookMarkerSurfaceStartingLocation;
        private Point _lastMousePosition = Point.None;

        private readonly Color _lookMarkerColor = new(200, 0, 0, 170);

        public SelectMapLocationState(MainGame gameScreen,
            Action<LookMarkerPosition>? positionChanged = null,
            Action<LookMarkerPosition>? positionSelected = null,
            Func<Point>? getLookMarkerSurfaceStartingLocation = null)
            : base(gameScreen, false, false, true, false)
        {
            // Store callbacks
            _positionChanged = positionChanged;
            _positionSelected = positionSelected;
            _getLookMarkerSurfaceStartingLocation = getLookMarkerSurfaceStartingLocation;

            // Create the console we'll use as the marker to show where the user is looking.
            LookMarker = new(1, 1);
            LookMarker.Surface.Fill(Color.Transparent, _lookMarkerColor, 0);
            LookMarkerPosition = new(Point.Zero, Point.Zero);

            // Update the marker's position relative to the surface and map when the mouse moves
            LookMarker.PositionChanged += LookMarker_PositionChanged;

            // Create the keybindings component we'll use to handle input
            _keybindings = new(LookMarker);
            _keybindings.SetAction(Keys.Enter, OnPositionSelected);
            _keybindings.SetAction(Keys.Escape, () => GameScreen.CurrentState = new MainMapState(GameScreen));
        }

        public override void OnAdded(IScreenObject host)
        {
            base.OnAdded(host);
            host.Children.Add(LookMarker);
            host.SadComponents.Add(_keybindings);

            // Update the look marker's position to the starting location
            LookMarker.Position = _getLookMarkerSurfaceStartingLocation?.Invoke() ?? Engine.Player.Position - ((IScreenSurface)host).Surface.ViewPosition;
        }

        public override void OnRemoved(IScreenObject host)
        {
            base.OnRemoved(host);
            host.Children.Remove(LookMarker);
            host.SadComponents.Remove(_keybindings);
        }

        public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
        {
            handled = false;

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
                handled = true;
            }

            if (state.Mouse.LeftClicked)
            {
                OnPositionSelected();
                handled = true;
            }
        }

        /// <summary>
        /// Called when the user attempts to select a position by left-clicking or pressing enter.
        /// The function may return true to accept the current <see cref="LookMarkerPosition"/> value,
        /// or false to reject it.
        /// </summary>
        /// <returns>True if the selected position is acceptable; false otherwise.</returns>
        protected virtual bool ValidateSelectedPosition() => true;

        private void OnPositionSelected()
        {
            if (!ValidateSelectedPosition()) return;

            _positionSelected?.Invoke(LookMarkerPosition);
        }

        private void LookMarker_PositionChanged(object? sender, SadConsole.ValueChangedEventArgs<Point> e)
        {
            // The entity position is in surface position; so we calculate the map position based on that
            LookMarkerPosition = new(e.NewValue, e.NewValue + (Parent?.Surface.ViewPosition ?? Point.Zero));

            // Generate the text to display in the status panel.
            var entityName = "You see " + (GameScreen.Map.GetEntityAt<RogueLikeEntity>(LookMarkerPosition.MapPosition)?.Name ?? "nothing here.");
            GameScreen.StatusPanel.LookInfo.DisplayText = entityName;

            _positionChanged?.Invoke(LookMarkerPosition);
        }
    }
}
