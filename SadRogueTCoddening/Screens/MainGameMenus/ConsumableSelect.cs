using SadConsole.UI.Controls;
using SadRogue.Integration;
using SadRogueTCoddening.MapObjects.Components;
using SadRogueTCoddening.MapObjects.Components.Items;

namespace SadRogueTCoddening.Screens.MainGameMenus;

/// <summary>
/// A wrapper around a RogueLikeEntity, which ensures that when it is displayed in a menu, it is displayed as its Name field.
/// </summary>
internal class ListItem
{
    public RogueLikeEntity Item { get; init; } = null!;

    public override string ToString()
    {
        return Item.Name;
    }
}

/// <summary>
/// Menu used by the player to select a consumable from their inventory to use.
/// </summary>
internal class ConsumableSelect : MainGameMenu
{
    private readonly Inventory _playerInventory;
    
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
        
        // Find any consumable items and add them to a ListBox
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
        
        // Handle when an item is selected by using it.
        list.SelectedItemExecuted += OnItemSelected;
    }

    private void OnItemSelected(object? sender, ListBox.SelectedItemEventArgs e)
    {
        Hide();

        var item = ((ListItem)e.Item).Item;
        PlayerActionHelper.PlayerTakeAction(_ => _playerInventory.Consume(item));
    }
}