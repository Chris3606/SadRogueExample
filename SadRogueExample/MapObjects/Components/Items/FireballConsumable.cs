using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Integration;
using SadRogue.Primitives.GridViews;
using SadRogueExample.Themes;

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

                    // TODO: Color
                    var color = entity == Engine.Player ? MessageColors.EnemyAtkAppearance : MessageColors.PlayerAtkAppearance;
                    Engine.GameScreen.MessageLog.AddMessage(
                        new($"The {entity.Name} is engulfed in a fiery explosion, taking {Damage} damage!", color));
                    combatant.HP -= Damage;
                    hitSomething = true;
                }
            }
            
            // TODO: Color
            if (!hitSomething)
                Engine.GameScreen.MessageLog.AddMessage(new("A fireball explodes but doesn't hit anything!", MessageColors.PlayerAtkAppearance));

            return true;
        }
}
