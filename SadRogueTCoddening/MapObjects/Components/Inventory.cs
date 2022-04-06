using System;
using System.Collections.Generic;
using SadRogue.Integration;
using SadRogue.Integration.Components;

namespace SadRogueTCoddening.MapObjects.Components;

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

        // TODO: Pick a color, any color!
        Engine.GameScreen?.MessageLog.AddMessage(new($"You dropped the {item.Name}."));
    }
}