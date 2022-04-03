using SadRogue.Integration;
using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;

namespace SadRogueTCoddening.MapObjects.Components
{
    /// <summary>
    /// Subclass of the integration library's keybindings component that moves enemies as appropriate when the player
    /// moves.
    /// </summary>
    internal class CustomPlayerKeybindingsComponent : PlayerKeybindingsComponent
    {
        protected override void MotionHandler(Direction direction)
        {
            if (Parent == null) return;

            // If we're waiting a turn, no need to bump anything.
            if (direction != Direction.None)
            {
                var result = SadRogueTCoddening.Actions.Bump((RogueLikeEntity)Parent, direction);
                if (!result) return; // If we didn't do anything, we won't count this as an action.
            }
            
            // Otherwise, we took an action, so end turn
            SadRogueTCoddening.Actions.TakeEnemyTurns(Parent.CurrentMap!);
        }
    }
}
