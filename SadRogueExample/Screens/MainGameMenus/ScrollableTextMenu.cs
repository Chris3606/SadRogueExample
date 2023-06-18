using System;
using SadConsole;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;
using SadRogue.Primitives.GridViews;

namespace SadRogueExample.Screens.MainGameMenus;

class ScrollableTextMenu : MainGameMenu
{
    protected readonly ScrollBar ScrollBar;
    private int _scrollOffset;
    private int _lastCursorY;
    private bool _allowInput;

    /// <summary>
    /// The child console that displays the text written by the cursor.
    /// </summary>
    public Console MessageBuffer { get; }

    /// <summary>
    /// When <see langword="true"/>, the console scrolls to the end when the cursor writes a new line.
    /// </summary>
    public bool AutomaticScroll { get; set; }

    /// <summary>
    /// When <see langword="true"/>, allow keyboard input to the cursor.
    /// </summary>
    public bool AllowInput
    {
        get => _allowInput;
        set
        {
            _allowInput = value;
            MessageBuffer.UseKeyboard = value;
        }
    }

    public ScrollableTextMenu(int width, int height, int bufferHeight) : base(width, height)
    {
        // Input settings
        UseMouse = true;
        FocusOnMouseClick = false;

        // Create the message buffer console
        var writableSurfaceBounds = this.Bounds().Expand(-1, -1);
        MessageBuffer = new Console(writableSurfaceBounds.Width, writableSurfaceBounds.Height, writableSurfaceBounds.Width, bufferHeight)
        {
            UseMouse = false,
            UseKeyboard = false,
            Position = writableSurfaceBounds.Position,
        };

        // Reassign the message buffer cursor to this object
        SadComponents.Remove(Cursor);
        Cursor = MessageBuffer.Cursor;
        Cursor.IsEnabled = true;

        // Handle the scroll bar control
        ScrollBar = new ScrollBar(Orientation.Vertical, Height)
        {
            IsEnabled = false,
            Position = (Width - 1, 0)
        };
        ScrollBar.ValueChanged += (sender, e) => MessageBuffer.ViewPosition = (0, ScrollBar.Value);
        Controls.Add(ScrollBar);

        Children.Add(MessageBuffer);
    }

    public virtual void Clear()
    {
        MessageBuffer.Clear();
        _scrollOffset = 0;
        Cursor.Position = (0, 0);
        ScrollBar.IsEnabled = false;
    }

    public override void Update(TimeSpan delta)
    {
        // If cursor has moved below the visible area, track the difference
        if (MessageBuffer.Cursor.Position.Y > _scrollOffset + MessageBuffer.ViewHeight - 1)
            _scrollOffset = MessageBuffer.Cursor.Position.Y - MessageBuffer.ViewHeight + 1;

        // Adjust the scroll bar
        ScrollBar.IsEnabled = _scrollOffset != 0;
        ScrollBar.Maximum = _scrollOffset;

        // If auto-scrolling is enabled, scroll
        if (ScrollBar.IsEnabled && AutomaticScroll && _lastCursorY != MessageBuffer.Cursor.Position.Y)
        {
            ScrollBar.Value = ScrollBar.Maximum;
            _lastCursorY = MessageBuffer.Cursor.Position.Y;
        }

        // Update the base class which includes the controls
        base.Update(delta);
    }
}
