using SadConsole.Components;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;
using SadRogueExample.Themes;

namespace SadRogueExample.MapObjects.Components.Items;

internal class LightningDamageConsumable: RogueLikeComponentBase<RogueLikeEntity>, IConsumable
{
    public readonly int Damage;
    public readonly int MaxRange;

    public LightningDamageConsumable(int damage, int maxRange)
        : base(false, false, false, false)
    {
        Damage = damage;
        MaxRange = maxRange;
    }

    public IComponent? GetStateHandler(RogueLikeEntity consumer) => null;

    public bool Consume(RogueLikeEntity consumer)
    {
        if (consumer.CurrentMap == null) return false;

        // Find target
        var radius = (Radius)consumer.CurrentMap.DistanceMeasurement;
        Combatant? target = null;
        foreach (var pos in radius.PositionsInRadius(consumer.Position, MaxRange))
        {
            var entity = consumer.CurrentMap.GetEntityAt<RogueLikeEntity>(pos);
            if (entity != null && entity != consumer)
            {
                target = entity.AllComponents.GetFirstOrDefault<Combatant>();
                if (target != null) break;
            }
        }

        if (target == null)
        {
            Engine.GameScreen?.MessageLog.AddMessage(new("There is no enemy close enough to strike.", MessageColors.ImpossibleActionAppearance));
            return false;
        }

        // Lightning damage bypasses defense.
        Engine.GameScreen?.MessageLog.AddMessage(new($"A lightning bolt zaps the {target.Parent!.Name} with a loud thunder, for {Damage} damage!", MessageColors.PlayerAtkAppearance));
        target.HP -= Damage;
        
        return true;
    }
}

