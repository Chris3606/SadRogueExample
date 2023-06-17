using System;
using System.Collections.Generic;
using SadConsole;

internal readonly record struct Message(ColoredString Text, int Count);

internal class MessageAddedEventArgs : EventArgs
{
    public Message Message { get; }

    public MessageAddedEventArgs(Message message)
    {
        Message = message;
    }
}

namespace SadRogueExample
{
    /// <summary>
    /// A class that keeps a log of messages meant to be displayed to the user.
    /// </summary>
    internal class MessageLog
    {
        private readonly List<Message> _messages;
        public IReadOnlyList<Message> Messages => _messages;

        public int MaxMessages { get; }

        public event EventHandler<MessageAddedEventArgs>? MessageAdded;

        public MessageLog(int maxMessages)
        {
            _messages = new List<Message>();
            MaxMessages = maxMessages;
        }

        public void Add(ColoredString message)
        {
            if (_messages.Count == 0)
            {
                _messages.Add(new(message, 1));
                
            }
            else
            {
                if (_messages.Count >= MaxMessages)
                    _messages.RemoveAt(0);

                var lastMessage = _messages[^1];

                // For now, we'll just blend messages with different colors but same content; but really we should take into account both
                if (lastMessage.Text.String == message.String)
                    _messages[^1] = new(lastMessage.Text, lastMessage.Count + 1);
                else
                    _messages.Add(new(message, 1));
            }
            
            MessageAdded?.Invoke(this, new MessageAddedEventArgs(_messages[^1]));
        }
    }
}
