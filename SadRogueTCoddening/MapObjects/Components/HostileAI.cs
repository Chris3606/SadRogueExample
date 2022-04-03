using System.Linq;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;

namespace SadRogueTCoddening.MapObjects.Components;

/// <summary>
/// Simple component that moves its parent toward the player if the player is visible. It demonstrates the basic
/// usage of the integration library's component system, as well as basic AStar pathfinding.
/// </summary>
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
        Actions.Bump(Parent, Direction.GetDirection(firstPoint - Parent.Position));
    }
}