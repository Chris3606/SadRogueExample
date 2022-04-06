using SadConsole;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using SadRogueTCoddening.MapObjects.Components;
using SadRogueTCoddening.MapObjects.Components.Items;

namespace SadRogueTCoddening.Screens.MainGameMenus;

internal class ConsumableSelect : MainGameMenu
{
    private Inventory _playerInventory;
    
    public ConsumableSelect()
        : base(51, 15)
    {
        Position = new Point(Constants.ScreenWidth / 2, Constants.ScreenHeight / 2) - new Point(Width / 2, Height / 2);
        
        _playerInventory = Engine.Player.AllComponents.GetFirst<Inventory>();
        if (_playerInventory.Items.Count == 0)
        {
            Surface.Print(0, Height / 2, "There are no items in your inventory.");
            return;
        }
        
        Surface.Print(0, 0, "Select an item to consume:");
        bool foundItem = false;
        var list = new ListBox(Width, Height - 1)
        {
            Position = (0, 1)
        };
        
        foreach (var item in _playerInventory.Items)
        {
            var consumable = item.AllComponents.GetFirstOrDefault<IConsumable>();
            if (consumable == null) continue;
        
            foundItem = true;
            list.Items.Add(item.Name);
        }
        
        if (!foundItem)
            Surface.Print(0, Height / 2, "There are no consumable items in your inventory.");
        else
            Controls.Add(list);
    }
}