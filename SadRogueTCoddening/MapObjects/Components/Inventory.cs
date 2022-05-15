using System;
using System.Collections.Generic;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogueTCoddening.MapObjects.Components.Items;
using SadRogueTCoddening.Themes;

namespace SadRogueTCoddening.MapObjects.Components;

/// <summary>
/// Component representing an inventory which can hold a given number of items.
/// </summary>
internal class Inventory : RogueLikeComponentBase<RogueLikeEntity>
{
    public int Capacity { get; }
    
    public readonly List<RogueLikeEntity> Items;

    public Inventory(int capacity)
        : base(false, false, false, false)
    {
        Capacity = capacity;
        Items = new List<RogueLikeEntity>(capacity);
    }

    /// <summary>
    /// Drops the given item from this inventory.
    /// </summary>
    public void Drop(RogueLikeEntity item)
    {
        if (Parent == null)
            throw new InvalidOperationException(
                "Can't drop an entity from an inventory that's not connected to an object.");
        if (Parent.CurrentMap == null)
            throw new InvalidOperationException(
                "Objects are not allowed to drop items from their inventory when they're not part of a map.");

        if (!Items.Remove(item))
            throw new ArgumentException("Tried to drop an item from an inventory it was not a part of.", nameof(item));
        
        item.Position = Parent.Position;
        Parent.CurrentMap.AddEntity(item);

        if (Parent == Engine.Player)
            Engine.GameScreen?.MessageLog.AddMessage(new($"You dropped the {item.Name}.", MessageColors.ItemDroppedAppearance));
    }

    /// <summary>
    /// Tries to pick up the first item found at the Parent's location.
    /// </summary>
    /// <returns>True if an item was picked up; false otherwise.</returns>
    public bool PickUp()
    {
        if (Parent == null)
            throw new InvalidOperationException(
                "Can't pick up an item into an inventory that's not connected to an object.");

        if (Parent.CurrentMap == null)
            throw new InvalidOperationException("Entity must be part of a map to pick up items.");

        var isPlayer = Parent == Engine.Player;

        var inventory = Parent.AllComponents.GetFirst<Inventory>();
        foreach (var item in Parent.CurrentMap.GetEntitiesAt<RogueLikeEntity>(Parent.Position))
        {
            if (!item.AllComponents.Contains<ICarryable>()) continue;

            if (inventory.Items.Count >= inventory.Capacity)
            {
                if (isPlayer)
                    Engine.GameScreen?.MessageLog.AddMessage(new("Your inventory is full.",
                        MessageColors.ImpossibleActionAppearance));
                return false;
            }

            item.CurrentMap!.RemoveEntity(item);
            inventory.Items.Add(item);

            if (isPlayer)
                Engine.GameScreen?.MessageLog.AddMessage(new($"You picked up the {item.Name}.",
                    MessageColors.ItemPickedUpAppearance));

            // TODO: Not great place for this
            //TakeEnemyTurns(Engine.Player.CurrentMap!);
            return true;
        }
        if (isPlayer)
            Engine.GameScreen?.MessageLog.AddMessage(new("There is nothing here to pick up.", MessageColors.ImpossibleActionAppearance));
        return false;
    }

    /// <summary>
    /// Causes the parent to consume the given consumable item.  The given entity must have some component implementing IConsumable.
    /// </summary>
    public bool Consume(RogueLikeEntity item)
    {
        if (Parent == null)
            throw new InvalidOperationException("Cannot consume item from an inventory not attached to an object.");
        var consumable = item.AllComponents.GetFirst<IConsumable>();
        
        int idx = Items.FindIndex(i => i == item);
        if (idx == -1)
            throw new ArgumentException("Tried to consume a consumable that was not in the inventory.");

        bool result = consumable.Consume(Parent);
        if (!result) return false;
        
        Items.RemoveAt(idx);
        return true;
    }
}