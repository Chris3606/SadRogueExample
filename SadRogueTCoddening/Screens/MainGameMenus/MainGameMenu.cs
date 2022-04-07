using System;
using SadConsole.UI;
using SadRogue.Primitives;

namespace SadRogueTCoddening.Screens.MainGameMenus;

/// <summary>
/// A Window that can be used as a modal menu over the game screen by adding it as a child to the MainGameScreen.
/// </summary>
internal class MainGameMenu : Window
{
    public MainGameMenu(int width, int height)
        : base(width, height)
    {
        CloseOnEscKey = true;
        IsModalDefault = true;
        
        Center();
        Show();
    }

    protected void PrintTextAtCenter(string text)
    {
        int effectiveWidth = Width - 2;
        if (text.Length > effectiveWidth)
            throw new ArgumentException("Message too long to print.");

        var pos = new Point(effectiveWidth / 2 - text.Length / 2 + 1, (Height - 2) / 2 + 1);
        Cursor.Move(pos);
        Cursor.Print(text);
    }
}