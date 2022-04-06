using SadConsole;
using SadConsole.Input;
using SadConsole.UI;

namespace SadRogueTCoddening.Screens.MainGameMenus;

/// <summary>
/// A ControlsConsole that can be used as a modal menu over the game screen by adding it as a child to the MainGameScreen.
/// </summary>
internal class MainGameMenu : ControlsConsole
{
    private IScreenObject _focused;
    
    public MainGameMenu(int width, int height)
        : base(width, height)
    {
        _focused = Game.Instance.FocusedScreenObjects.ScreenObject;
        IsFocused = true;
    }
    
    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.Escape))
        {
            CloseWindow();
            return true;
        }
        
        return base.ProcessKeyboard(keyboard);
    }

    private void CloseWindow()
    {
        _focused.IsFocused = true;
        Parent.Children.Remove(this);
    }
}