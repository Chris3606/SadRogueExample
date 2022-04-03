using SadRogue.Integration;
using SadRogue.Primitives;
using SadRogueTCoddening.MapObjects.Components.Items;
using SadRogueTCoddening.Maps;

namespace SadRogueTCoddening.MapObjects.Items;

/// <summary>
/// Factory for items.
/// </summary>
internal static class Factory
{
    public static RogueLikeEntity HealthPotion()
    {
        var potion = new RogueLikeEntity(new Color(127, 0, 255), '!', layer: (int)GameMap.Layer.Items)
        {
            Name = "Health Potion"
        };
        potion.AllComponents.Add(new HealingConsumable(4));

        return potion;
    }
}