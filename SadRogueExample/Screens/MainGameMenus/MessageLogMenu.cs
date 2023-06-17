using SadConsole;

namespace SadRogueExample.Screens.MainGameMenus
{
    internal class MessageLogMenu : ScrollableTextMenu
    {
        public MessageLogMenu(int width, int height, int bufferHeight)
            : base(width, height, bufferHeight)
        {
            Title = "Message Log";

            Cursor.AutomaticallyShiftRowsUp = true;
            ParentChanged += MessageLogMenu_ParentChanged;

            // Due to Show() being called in base class constructor, parent may already be set by the time this code runs
            if (Parent != null)
            {
                Engine.MessageLog.MessageAdded += MessageLog_MessageAdded;
                PrintMessages();
            }
        }

        private void MessageLogMenu_ParentChanged(object? sender, ValueChangedEventArgs<IScreenObject> e)
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
}
