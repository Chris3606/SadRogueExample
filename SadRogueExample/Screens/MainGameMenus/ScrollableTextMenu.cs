using System;
using SadConsole;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;
using SadRogue.Primitives.GridViews;

namespace SadRogueExample.Screens.MainGameMenus;

class ScrollableTextMenu : MainGameMenu
{
    private readonly ScrollBar _scrollBar;
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
        _scrollBar = new ScrollBar(Orientation.Vertical, Height)
        {
            IsEnabled = false,
            Position = (Width - 1, 0)
        };
        _scrollBar.ValueChanged += (sender, e) => MessageBuffer.ViewPosition = (0, _scrollBar.Value);
        Controls.Add(_scrollBar);

        Children.Add(MessageBuffer);
    }

    public void Clear()
    {
        MessageBuffer.Clear();
        _scrollOffset = 0;
        Cursor.Position = (0, 0);
        _scrollBar.IsEnabled = false;
    }

    public override void Update(TimeSpan delta)
    {
        // If cursor has moved below the visible area, track the difference
        if (MessageBuffer.Cursor.Position.Y > _scrollOffset + MessageBuffer.ViewHeight - 1)
            _scrollOffset = MessageBuffer.Cursor.Position.Y - MessageBuffer.ViewHeight + 1;

        // Adjust the scroll bar
        _scrollBar.IsEnabled = _scrollOffset != 0;
        _scrollBar.Maximum = _scrollOffset;

        // If auto-scrolling is enabled, scroll
        if (_scrollBar.IsEnabled && AutomaticScroll && _lastCursorY != MessageBuffer.Cursor.Position.Y)
        {
            _scrollBar.Value = _scrollBar.Maximum;
            _lastCursorY = MessageBuffer.Cursor.Position.Y;
        }

        // Update the base class which includes the controls
        base.Update(delta);
    }
}
