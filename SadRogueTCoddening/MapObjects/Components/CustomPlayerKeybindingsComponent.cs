using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;
using SadRogueTCoddening.Maps;

namespace SadRogueTCoddening.MapObjects.Components;

/// <summary>
/// Subclass of the integration library's keybindings component that moves enemies as appropriate when the player
/// moves.
/// </summary>
internal class CustomPlayerKeybindingsComponent : PlayerKeybindingsComponent
{
    protected override void MotionHandler(Direction direction)
    {
        if (Parent == null) return;

        // If we're waiting a turn, there's nothing to do; it's always a valid turn to wait
        if (direction == Direction.None)
            PlayerActionHelper.PlayerTakeAction(_ => true);
        else
            PlayerActionHelper.PlayerTakeAction(GameMap.MoveOrBump, direction);
    }
}