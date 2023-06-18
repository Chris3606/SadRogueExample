using SadConsole;
using SadRogue.Primitives;

namespace SadRogueExample.Screens.Surfaces;

/// <summary>
/// A very basic SadConsole Console subclass that acts as a game message log panel which displays the most recent messages.
/// </summary>
public class MessageLogPanel : Console
{
    private bool _firstMessage;
    private int _linesOfPreviousMessage;

    public MessageLogPanel(int width, int height)
        : base(width, height)
    {
        Initialize();
    }

    public MessageLogPanel(int width, int height, int bufferWidth, int bufferHeight)
        : base(width, height, bufferWidth, bufferHeight)
    {
        Initialize();
    }

    public MessageLogPanel(ICellSurface surface, IFont? font = null, Point? fontSize = null)
        : base(surface, font, fontSize)
    {
        Initialize();
    }

    private void Initialize()
    {
        _firstMessage = true;
        _linesOfPreviousMessage = 0;

        Cursor.AutomaticallyShiftRowsUp = true;
        ParentChanged += MessageLogPanel_ParentChanged;
    }

    private void MessageLogPanel_ParentChanged(object? sender, SadConsole.ValueChangedEventArgs<IScreenObject> e)
    {
        if (e.NewValue == null)
        {
            Engine.MessageLog.MessageAdded -= MessageLog_MessageAdded;
            _firstMessage = true;
            Cursor.Position = (0, 0);
        }
        else
        {
            Engine.MessageLog.MessageAdded += MessageLog_MessageAdded;
            PrintMessages();
        }
    }

    private void MessageLog_MessageAdded(object? sender, MessageAddedEventArgs e)
    {
        // Overwrite previous message text with the count, if it's the same.
        // We make sure we shift back a number of rows equal to the number of lines in the previous message.
        // There is no need to clear the existing text, since the new message is guaranteed to be as long if not
        // longer than the previously written message (either it will have added a count, or the count will increase)
        var addNewline = true;
        if (e.Message.Count > 1)
        {
            // No new line is automatically added at the end of messages; so, the cursor is sitting at the end of the previous
            // message.  This means that to move it back to the beginning of the previous message, we need to move it back
            // one row less than the number of lines in the previous message.
            Cursor.Position = Cursor.Position.Translate(-Cursor.Position.X, -(_linesOfPreviousMessage - 1));
            addNewline = false;
        }

        PrintMessage(e.Message, addNewline);
    }

    private void PrintMessages()
    {
        foreach (var message in Engine.MessageLog.Messages)
            PrintMessage(message, true);
    }

    // Prints the given message at the current cursor position, with a new-line if needed.
    // Disable the new-line if you want are overwriting the previous message.
    private void PrintMessage(Message message, bool addNewline)
    {
        if (!_firstMessage && addNewline)
            Cursor.NewLine();

        TimesShiftedUp = 0;
        int oldY = Cursor.Position.Y;

        _firstMessage = false;

        var text = message.Count == 1 ? message.Text : message.Text + " (x" + message.Count.ToString() + ")";
        Cursor.Print(text);

        _linesOfPreviousMessage = Cursor.Position.Y - oldY + 1 + TimesShiftedUp;
    }
}