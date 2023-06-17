using SadRogue.Integration;
using SadRogueExample.MapObjects.Components.AI;
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
        // TODO: Doing this here allows the targeter to have already accepted this position.  Ideally, this would be a targeter option instead
        if (currentAI == null)
        {
            Engine.MessageLog.Add(
                                       new("You cannot confuse an inanimate object.",
                                                                      MessageColors.ImpossibleActionAppearance));
            return false;
        }
        target.AllComponents.Remove(currentAI);
        Engine.MessageLog.Add(new($"The eyes of the {target.Name} look vacant, as it starts to stumble around!", MessageColors.StatusEffectAppliedAppearance));
        target.AllComponents.Add(new ConfusedAI(NumberOfTurns, currentAI));

        return true;
    }
}
