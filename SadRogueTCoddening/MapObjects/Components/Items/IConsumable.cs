using SadRogue.Integration;

namespace SadRogueTCoddening.MapObjects.Components.Items;

/// <summary>
/// Implemented by things that can be consumed.
/// </summary>
internal interface IConsumable : ICarryable
{
    bool Consume(RogueLikeEntity consumer);
}