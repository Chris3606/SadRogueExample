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

        Engine.GameScreen?.MessageLog.AddMessage(new($"You dropped the {item.Name}.", MessageColors.ItemDroppedAppearance));
    }

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