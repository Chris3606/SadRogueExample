using SadConsole;
using SadRogue.Primitives;

namespace SadRogueExample.Screens.Surfaces;

/// <summary>
/// A very basic SadConsole Console subclass that acts as a game message log panel which displays the most recent messages.
/// </summary>
public class MessageLogPanel : Console
{
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
        Cursor.AutomaticallyShiftRowsUp = true;
        ParentChanged += MessageLogPanel_ParentChanged;
    }

    private void MessageLogPanel_ParentChanged(object? sender, SadConsole.ValueChangedEventArgs<IScreenObject> e)
    {
        if (e.NewValue == null)
            Engine.MessageLog.MessageAdded -= MessageLog_MessageAdded;
        else
        {
            Engine.MessageLog.MessageAdded += MessageLog_MessageAdded;
            PrintMessages();
        }
    }

    private void MessageLog_MessageAdded(object? sender, MessageAddedEventArgs e) => PrintMessage(e.Message);
    
    private void PrintMessages()
    {
        Cursor.Position = (0, 0);

        foreach (var message in Engine.MessageLog.Messages)
            PrintMessage(message);
    }

    private void PrintMessage(Message message)
    {
        if (message.Count > 1)
        {
            Cursor.Position = Cursor.Position.Translate(0, -1);
            Cursor.Print(message.Text + " (x" + message.Count.ToString() + ")");
        }
        else
            Cursor.Print(message.Text);
        Cursor.NewLine();
    }
}