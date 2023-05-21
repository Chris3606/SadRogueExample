using SadRogue.Integration;
using SadRogue.Primitives;
using SadRogueExample.Maps;
using SadRogueExample.MapObjects.Components.Items;

namespace SadRogueExample.MapObjects.Items;

/// <summary>
/// Simple class with some static functions for creating items.
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

    public static RogueLikeEntity LightningScroll()
    {
        var scroll = new RogueLikeEntity(new Color(255, 255, 0), '~', layer: (int)GameMap.Layer.Items)
        {
            Name = "Lightning Scroll"
        };
        scroll.AllComponents.Add(new LightningDamageConsumable(20, 5));

        return scroll;
    }
}