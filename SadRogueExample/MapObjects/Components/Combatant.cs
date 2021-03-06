using System;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogueExample.Themes;

namespace SadRogueExample.MapObjects.Components;

/// <summary>
/// Component for entities that allows them to have health and attack.
/// </summary>
internal class Combatant : RogueLikeComponentBase<RogueLikeEntity>, IBumpable
{
    private int _hp;

    public int HP
    {
        get => _hp;
        private set
        {
            if (_hp == value) return;

            _hp = Math.Min(Math.Max(0, value), MaxHP);
            HPChanged?.Invoke(this, EventArgs.Empty);

            if (_hp == 0)
                Died?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? HPChanged;

    public event EventHandler? Died;

    public int MaxHP { get; }
    public int Defense { get; }
    public int Power { get; }

    public Combatant(int hp, int defense, int power)
        : base(false, false, false, false)
    {
        HP = MaxHP = hp;
        Defense = defense;
        Power = power;
    }

    public int Heal(int amount)
    {
        amount = Math.Min(amount, MaxHP - HP);
        HP += amount;

        return amount;
    }

    public void Attack(Combatant target)
    {
        int damage = Power - target.Defense;
        string attackDesc = $"{Parent!.Name} attacks {target.Parent!.Name}";

        var atkTextColor = Parent == Engine.Player ? MessageColors.PlayerAtkAppearance : MessageColors.EnemyAtkAtkAppearance;
        if (damage > 0)
        {
            Engine.GameScreen?.MessageLog.AddMessage(new($"{attackDesc} for {damage} damage.", atkTextColor));
            target.HP -= damage;
        }
        else
            Engine.GameScreen?.MessageLog.AddMessage(new($"{attackDesc} but does no damage.", atkTextColor));
    }

    public bool OnBumped(RogueLikeEntity source)
    {
        var combatant = source.AllComponents.GetFirstOrDefault<Combatant>();
        if (combatant == null) return false;

        combatant.Attack(this);
        return true;
    }
}