﻿using Microsoft.Toolkit.HighPerformance.Enumerables;
using SadConsole.UI.Controls;
using SadRogue.Integration;
using SadRogueTCoddening.MapObjects.Components;
using SadRogueTCoddening.MapObjects.Components.Items;

namespace SadRogueTCoddening.Screens.MainGameMenus;

internal class ListItem
{
    public RogueLikeEntity Item { get; init; }

    public override string ToString()
    {
        return Item.Name;
    }
}
internal class ConsumableSelect : MainGameMenu
{
    private Inventory _playerInventory;
    
    public ConsumableSelect()
        : base(51, 15)
    {
        Title = "Select an item to consume:";

        _playerInventory = Engine.Player.AllComponents.GetFirst<Inventory>();
        if (_playerInventory.Items.Count == 0)
        {
            PrintTextAtCenter("There are no items in your inventory.");
            return;
        }
        
        bool foundItem = false;
        var list = new ListBox(Width - 2, Height - 2) { Position = (1, 1), SingleClickItemExecute = true };
        
        foreach (var item in _playerInventory.Items)
        {
            var consumable = item.AllComponents.GetFirstOrDefault<IConsumable>();
            if (consumable == null) continue;
        
            foundItem = true;
            list.Items.Add(new ListItem{Item = item});
        }
        
        if (!foundItem)
            PrintTextAtCenter("There are no consumable items in your inventory.");
        else
            Controls.Add(list);
        
        list.SelectedItemExecuted += OnItemSelected;
    }

    private void OnItemSelected(object? sender, ListBox.SelectedItemEventArgs e)
    {
        var item = ((ListItem)e.Item).Item;

        var consumable = item.AllComponents.GetFirst<IConsumable>();
        consumable.Consume(Engine.Player);
        _playerInventory.Items.Remove(item);

        Hide();
        Actions.TakeEnemyTurns(Engine.Player.CurrentMap!);
    }
}