using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogueTCoddening.Themes;

namespace SadRogueTCoddening.MapObjects.Components.Items;

internal class HealingConsumable : RogueLikeComponentBase<RogueLikeEntity>, IConsumable
{
    public int Amount { get; }

    public HealingConsumable(int amount)
        : base(false, false, false, false)
    {
        Amount = amount;
    }

    public bool Consume(RogueLikeEntity consumer)
    {
        var combatant = consumer.AllComponents.GetFirst<Combatant>();
        var amountRecovered = combatant.Heal(Amount);
        if (amountRecovered > 0)
        {
            Engine.GameScreen?.MessageLog.AddMessage(new(
                $"You consume the {Parent!.Name}, and recover {amountRecovered} HP!",
                MessageColors.HealthRecoveredAppearance));
            return true;
        }
        
        Engine.GameScreen?.MessageLog.AddMessage(new("Your health is already full.", MessageColors.ImpossibleActionAppearance));
        return true;
    }
}