using SadRogue.Integration.Components;
using SadRogue.Integration;

namespace SadRogueExample.MapObjects.Components.AI;

/// <summary>
/// Base component for all AIs.  Only one type of AI can be attached to a given entity at a time.
/// </summary>
/// <remarks>
/// Any entities with this component will take their "turn" via the TakeTurn function after the player takes their action for a turn.
/// </remarks>
internal abstract class AIBase : RogueLikeComponentBase<RogueLikeEntity>
{
    public AIBase()
        : base(false, false, false, false)
    {
        // Only allow one type of AI on an entity at a time
        Added += IncompatibleWith<AIBase>;
    }

    /// <summary>
    /// Causes its parent to take a turn.
    /// </summary>
    public abstract void TakeTurn();
}
