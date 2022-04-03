﻿using System;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;

namespace SadRogueTCoddening.MapObjects.Components;

/// <summary>
/// Added to entities that have health and can attack.
/// </summary>
internal class Combatant : RogueLikeComponentBase<RogueLikeEntity>, IBumpable
{
    private int _hp;

    public int HP
    {
        get => _hp;
        private set
        {
            _hp = Math.Max(0, value);
            if (_hp == 0)
                Died?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? Died;

    public int MaxHP { get; }
    public int Defense { get;}
    public int Power { get; }

    public Combatant(int hp, int defense, int power)
        : base(false, false, false, false)
    {
        HP = MaxHP = hp;
        Defense = defense;
        Power = power;
    }

    public void Attack(Combatant target)
    {
        int damage = Power - target.Defense;
        string attackDesc = $"{Parent!.Name} attacks {target.Parent!.Name}";

        var atkTextColor = Parent == Engine.Player ? Constants.PlayerAtkTextColor : Constants.EnemyAtkTextColor;
        if (damage > 0)
        {
            Engine.GameScreen?.MessageLog.AddMessage(new($"{attackDesc} for {damage} damage.", atkTextColor, Color.Transparent));
            target.HP -= damage;
        }
        else
            Engine.GameScreen?.MessageLog.AddMessage(new($"{attackDesc} but does no damage.", atkTextColor, Color.Transparent));
    }

    public bool OnBumped(RogueLikeEntity source)
    {
        var combatant = source.AllComponents.GetFirstOrDefault<Combatant>();
        if (combatant == null) return false;
        
        combatant.Attack(this);
        return true;
    }
}