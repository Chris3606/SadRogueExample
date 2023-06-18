using GoRogue;
using SadConsole;

namespace SadRogueExample.Screens.MainGameMenus
{
    internal class MessageLogMenu : ScrollableTextMenu
    {
        private bool _firstMessage;
        private int _linesOfPreviousMessage;

        private ColoredGlyph[] _cachedTopRow;

        public MessageLogMenu(int width, int height, int bufferHeight)
            : base(width, height, bufferHeight)
        {
            Title = "Message Log";

            AutomaticScroll = true;
            _firstMessage = true;
            _linesOfPreviousMessage = 0;

            _cachedTopRow = new ColoredGlyph[MessageBuffer.Width];
            for (int i = 0; i < MessageBuffer.Width; i++)
                _cachedTopRow[i] = new ColoredGlyph();

            Cursor.AutomaticallyShiftRowsUp = true;
            ParentChanged += MessageLogMenu_ParentChanged;

            // Due to Show() being called in base class constructor, parent may already be set by the time this code runs
            if (Parent != null)
            {
                Engine.MessageLog.MessageAdded += MessageLog_MessageAdded;
                PrintMessages();
            }
        }

        public override void Clear()
        {
            _firstMessage = true;
            base.Clear();
        }

        private void MessageLogMenu_ParentChanged(object? sender, ValueChangedEventArgs<IScreenObject> e)
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

            MessageBuffer.TimesShiftedUp = 0;
            int oldY = Cursor.Position.Y;

            _firstMessage = false;

            // If the message we print is exactly the length of the console, SadConsole's cursor will automatically put in a new line.
            // We don't want to waste that line, so we'll cache what the top line is so we can put it back if a new line we don't want
            // gets placed during the print.
            for (int x = 0; x < MessageBuffer.Width; x++)
                MessageBuffer[x, 0].CopyAppearanceTo(_cachedTopRow[x], false);

            var text = message.Count == 1 ? message.Text : message.Text + " (x" + message.Count.ToString() + ")";
            Cursor.Print(text);

            // We wrote to the last character of the console and shifted things down, but didn't write anything on the new line;
            // so we'll put the original top row back.
            if (Cursor.Position.X == 0)
            {
                if (MessageBuffer.TimesShiftedUp > 0)
                {
                    MessageBuffer.TimesShiftedUp--;
                    MessageBuffer.ShiftDown(1);
                    for (int x = 0; x < MessageBuffer.Width; x++)
                        _cachedTopRow[x].CopyAppearanceTo(MessageBuffer[x, 0], false);
                }
                else
                    Cursor.Position = Cursor.Position.Translate(0, -1);
            }

            _linesOfPreviousMessage = Cursor.Position.Y - oldY + 1 + MessageBuffer.TimesShiftedUp;
        }
    }
}
