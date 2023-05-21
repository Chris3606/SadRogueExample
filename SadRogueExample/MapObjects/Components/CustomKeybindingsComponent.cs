using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;
using SadRogueExample.Maps;

namespace SadRogueExample.MapObjects.Components;

/// <summary>
/// Subclass of the integration library's keybindings component that ensures the player's movements count as the player's turn when (and only when) successful.
/// </summary>
internal class CustomKeybindingsComponent : PlayerKeybindingsComponent
{

    /// <summary>
    /// Creates a new component that maps keybindings to various forms of actions.
    /// </summary>
    /// <param name="motionHandler">
    /// The function to use for handling any keybindings in <see cref="Motions"/>.  If set to null, a default handler
    /// will be used that simply sets the parent object's Position if it can move in the selected direction.
    /// </param>
    /// <param name="sortOrder">Sort order for the component.</param>
    public PlayerKeybindingsComponent(Action<Direction>? motionHandler = null, uint sortOrder = 5U)
        : base(motionHandler, sortOrder)
    {
        
    }

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
/// Subclass of the integration library's keybindings component that ensures the player's movements count as the player's turn when (and only when) successful.
/// </summary>
internal class CustomPlayerKeybindingsComponent : PlayerKeybindingsComponent
{
    protected override void MotionHandler(Direction direction)
    {
        // If we're waiting a turn, there's nothing to do; it's always a valid turn to wait
        if (direction == Direction.None)
            PlayerActionHelper.PlayerTakeAction(_ => true);
        else
            PlayerActionHelper.PlayerTakeAction(GameMap.MoveOrBump, direction);
    }
}