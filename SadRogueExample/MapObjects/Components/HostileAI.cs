using System.Linq;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;
using SadRogueExample.Maps;

namespace SadRogueExample.MapObjects.Components;

/// <summary>
/// Simple component that moves its parent toward the player, if the player is visible to it.
/// </summary>
/// <remarks>
/// Any entities with this component will take their "turn" via the TakeTurn function after the player takes their action for a turn.
/// </remarks>
internal class HostileAI : RogueLikeComponentBase<RogueLikeEntity>
{
    public HostileAI()
        : base(false, false, false, false)
    {
    }

    public void TakeTurn()
    {
        if (Parent?.CurrentMap == null) return;
        if (!Parent.CurrentMap.PlayerFOV.CurrentFOV.Contains(Parent.Position)) return;
        if (Parent.AllComponents.GetFirst<Combatant>().HP <= 0) return;

        var path = Parent.CurrentMap.AStar.ShortestPath(Parent.Position, Engine.Player.Position);
        if (path == null) return;
        var firstPoint = path.GetStep(0);
        GameMap.MoveOrBump(Parent, Direction.GetDirection(Parent.Position, firstPoint));
    }
}