using SadConsole.Components;
using SadRogue.Integration;

namespace SadRogueExample.MapObjects.Components.Items;

/// <summary>
/// Implemented by things that can be consumed.
/// </summary>
internal interface IConsumable : ICarryable
{
    /// <summary>
    /// Returns a state object, if the item needs to get a target or otherwise
    /// change the state of the MainGame screen to gain information before its Consume
    /// function is called.
    /// </summary>
    /// <param name="consumer">The entity consuming the consumable.</param>
    /// <returns>A state object, if one is needed; null otherwise.</returns>
    IComponent? GetStateHandler(RogueLikeEntity consumer);

    // TODO: Rename to Activate?
    /// <summary>
    /// Performs the actual effect of the item.
    /// </summary>
    /// <param name="consumer">The entity consuming the consumable.</param>
    /// <returns>True if the consumption was successful; false otherwise.</returns>
    bool Consume(RogueLikeEntity consumer);
}