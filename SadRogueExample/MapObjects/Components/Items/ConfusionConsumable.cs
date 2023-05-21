using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;
using SadRogueExample.MapObjects.Components.AI;
using SadRogueExample.Screens;
using SadRogueExample.Themes;

namespace SadRogueExample.MapObjects.Components.Items;
internal class ConfusionConsumable : SingleTargetConsumable
{
    public readonly int NumberOfTurns;

    public ConfusionConsumable(int numberOfTurns)
        : base(false, false)
    {
        NumberOfTurns = numberOfTurns;
    }

    protected override bool OnUse(RogueLikeEntity target)
    {
        var currentAI = target.AllComponents.GetFirstOrDefault<AIBase>();
        if (currentAI == null)
        {
            Engine.GameScreen!.MessageLog.AddMessage(
                                       new("You cannot confuse an inanimate object.",
                                                                      MessageColors.ImpossibleActionAppearance));
            return false;
        }
        target.AllComponents.Remove(currentAI);
        // TODO: Color
        Engine.GameScreen!.MessageLog.AddMessage(new($"The eyes of the {target.Name} look vacant, as it starts to stumble around!", MessageColors.EnemyAtkAppearance));
        target.AllComponents.Add(new ConfusedAI(NumberOfTurns, currentAI));

        return true;
    }
}
