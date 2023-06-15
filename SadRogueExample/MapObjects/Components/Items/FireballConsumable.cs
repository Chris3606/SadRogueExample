using SadRogue.Primitives;
using SadRogue.Integration;

namespace SadRogueExample.MapObjects.Components.Items;

internal class FireballConsumable : AreaTargetConsumable
    {
        public readonly int Damage;

        public FireballConsumable(int damage, int radius)
            :base(radius, null, false)
        {
            Damage = damage;
        }

        protected override bool OnUse(Point target)
        {
            bool hitSomething = false;
            foreach (var pos in RadiusShape.PositionsInRadius(target, Radius,
                         Engine.GameScreen!.Map.DefaultRenderer!.Surface.View))
            {
                foreach (var entity in Engine.GameScreen.Map.GetEntitiesAt<RogueLikeEntity>(pos))
                {
                    var combatant = entity.AllComponents.GetFirstOrDefault<Combatant>();
                    if (combatant == null) continue;

                    Engine.GameScreen.MessageLog.AddMessage(
                        new($"The {entity.Name} is engulfed in a fiery explosion, taking {Damage} damage!"));
                    combatant.HP -= Damage;
                    hitSomething = true;
                }
            }
            
            if (!hitSomething)
                Engine.GameScreen.MessageLog.AddMessage(new("A fireball explodes but doesn't hit anything!"));

            return true;
        }
}
