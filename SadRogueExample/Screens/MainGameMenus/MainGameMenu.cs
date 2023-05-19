using System;
using SadConsole.UI;
using SadRogue.Primitives;

namespace SadRogueExample.Screens.MainGameMenus;

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

    /// <summary>
    /// Prints the given text centered in the window.  You may optionally specify an X or a Y value, in which case
    /// the text will be centered only along the other axis.
    /// </summary>
    protected void PrintTextAtCenter(string text, int? x = null, int? y = null)
    {
        if (x.HasValue && y.HasValue)
            throw new ArgumentException($"{nameof(PrintTextAtCenter)} should not be called with both an X and a Y value.");

        int effectiveWidth = Width - 2;
        if (text.Length > effectiveWidth)
            throw new ArgumentException("Message too long to print.");

        int printX = x ?? effectiveWidth / 2 - text.Length / 2 + 1;
        int printY = y ?? (Height - 2) / 2 + 1;
        var pos = new Point(printX, printY);
        Cursor.Move(pos);
        Cursor.Print(text);
    }
}