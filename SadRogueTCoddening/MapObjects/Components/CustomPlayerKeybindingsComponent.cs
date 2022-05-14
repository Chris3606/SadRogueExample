using SadRogue.Integration;
using SadRogue.Integration.Keybindings;
using SadRogue.Primitives;
using SadRogueTCoddening.Themes;

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

        // If we're waiting a turn, no need to bump anything.
        if (direction != Direction.None)
        {
            var result = SadRogueTCoddening.Actions.MoveOrBump((RogueLikeEntity)Parent, direction);
            // If we didn't do anything, we won't count this as an action.
            if (!result)
            {
                Engine.GameScreen?.MessageLog.AddMessage(new ("That way is blocked.", MessageColors.ImpossibleActionAppearance));
                return; 
            }
        }

        // Otherwise, we took an action, so end turn and let the enemies take theirs (unless the player somehow died
        // on their turn in which case we'll return)
        if (Parent.GoRogueComponents.GetFirst<Combatant>().HP <= 0) return;
        
        SadRogueTCoddening.Actions.TakeEnemyTurns(Parent.CurrentMap!);
    }
}