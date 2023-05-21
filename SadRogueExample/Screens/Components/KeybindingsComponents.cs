using SadConsole;
using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;
using SadRogueExample.Maps;

namespace SadRogueExample.Screens.Components;

/// <summary>
/// Subclass of the integration library's keybindings component that can optionally configure default motions
/// which we use in the game.
/// </summary>
internal class CustomKeybindingsComponent<TParent> : KeybindingsComponent<TParent>
    where TParent : class, IScreenObject
{
    public CustomKeybindingsComponent(bool configureDefaultMotions = true, uint sortOrder = 5U)
        : base(sortOrder)
    {
        if (configureDefaultMotions)
        {
            // Add default 8-way motions
            SetMotions(KeybindingsComponent.ArrowMotions);
            SetMotions(KeybindingsComponent.NumPadAllMotions);
            SetMotion(SadConsole.Input.Keys.NumPad5, Direction.None);
            SetMotion(SadConsole.Input.Keys.OemPeriod, Direction.None);
        }
    }
}

/// <summary>
/// Custom keybindings component which uses the MotionHandler to drive player movement.
/// Motion will count as the player's turn if (and only if) the move was successful.
/// </summary>
internal class MainMapKeybindingsComponent : CustomKeybindingsComponent<IScreenObject>
{
    public MainMapKeybindingsComponent(uint sortOrder = 5U)
        : base(true, sortOrder)
    { }

    protected override void MotionHandler(Direction direction)
    {
        // If we're waiting a turn, there's nothing to do; it's always a valid turn to wait
        if (direction == Direction.None)
            PlayerActionHelper.PlayerTakeAction(_ => true);
        else
            PlayerActionHelper.PlayerTakeAction(GameMap.MoveOrBump, direction);
    }
}

/// <summary>
/// Custom keybindings component which uses the MotionHandler to move a look marker around
/// the screen.
/// </summary>
internal class SelectLocationKeybindingsComponent : CustomKeybindingsComponent<IScreenSurface>
{
    private readonly IScreenSurface _lookMarker;

    public SelectLocationKeybindingsComponent(IScreenSurface lookMarker, uint sortOrder = 5U)
        : base(true, sortOrder)
    {
        _lookMarker = lookMarker;
    }

    protected override void MotionHandler(Direction direction)
    {
        if (Parent == null) return;
        if (direction == Direction.None) return;

        var area = new Rectangle(Parent.Position.X, Parent.Position.Y, Parent.Surface.ViewWidth, Parent.Surface.ViewHeight);
        var newPos = _lookMarker.Position + direction;
        if (area.Contains(newPos))
            _lookMarker.Position = newPos;
    }
}