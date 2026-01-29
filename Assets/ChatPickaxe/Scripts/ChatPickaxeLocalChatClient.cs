using System.Collections.Generic;
using LiveChat;
using UnityEngine;

namespace ChatPickaxe
{
    public class ChatPickaxeLocalChatClient : LiveChatClientBase
    {
        [SerializeField] private string _username = "Local";
        [SerializeField] private string _displayName = "Local";
        [SerializeField] private string _channelName = "local";
        [SerializeField] private int _maxLogEntries = 8;

        private readonly List<string> _log = new List<string>();
        private string _currentInput = string.Empty;
        private bool _focusInput = true;

        private GUIStyle _logStyle;
        private GUIStyle _inputStyle;
        private GUIStyle _boxStyle;

        private const string InputControlName = "ChatPickaxeInput";

        private void Start()
        {
            if (!IsConnected)
                Connect(new LiveChatConnectConfig { ChannelName = _channelName });

            AddSystemMessage("Local chat ready. Type !left or !right.");
        }

        public override void Connect(LiveChatConnectConfig config)
        {
            DefaultChannel = string.IsNullOrEmpty(config?.ChannelName) ? _channelName : config.ChannelName;
            IsConnected = true;
            RaiseConnected();
            RaiseJoinedChannel(DefaultChannel);
        }

        public override void Disconnect()
        {
            if (!IsConnected)
                return;

            IsConnected = false;
            RaiseDisconnected();
        }

        public override void SendMessage(string channel, string message)
        {
            AddSystemMessage($"Bot: {message}");
        }

        public override void SendReply(string channel, string replyToMessageId, string message)
        {
            AddSystemMessage($"Bot: {message}");
        }

        private void OnGUI()
        {
            EnsureStyles();

            float width = Mathf.Min(360f, Screen.width - 16f);
            float logHeight = 140f;
            float inputHeight = 24f;
            float x = 8f;
            float y = Screen.height - logHeight - inputHeight - 16f;

            GUI.Box(new Rect(x, y, width, logHeight), string.Empty, _boxStyle);
            DrawLog(x + 8f, y + 6f, width - 16f, logHeight - 12f);

            GUI.SetNextControlName(InputControlName);
            _currentInput = GUI.TextField(new Rect(x, y + logHeight + 4f, width, inputHeight), _currentInput, _inputStyle);

            if (_focusInput)
            {
                GUI.FocusControl(InputControlName);
                _focusInput = false;
            }

            Event e = Event.current;
            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter))
            {
                if (GUI.GetNameOfFocusedControl() == InputControlName)
                {
                    SendLocalMessage(_currentInput);
                    _currentInput = string.Empty;
                    e.Use();
                    _focusInput = true;
                }
            }
        }

        private void DrawLog(float x, float y, float width, float height)
        {
            int linesToShow = Mathf.Min(_log.Count, _maxLogEntries);
            float lineHeight = 18f;
            float startY = y + height - (linesToShow * lineHeight);

            int startIndex = Mathf.Max(0, _log.Count - linesToShow);
            for (int i = 0; i < linesToShow; i++)
            {
                string line = _log[startIndex + i];
                GUI.Label(new Rect(x, startY + i * lineHeight, width, lineHeight), line, _logStyle);
            }
        }

        private void SendLocalMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            string trimmed = message.Trim();
            AddSystemMessage($"{_displayName}: {trimmed}");

            LiveChatMessage chatMessage = new LiveChatMessage
            {
                Channel = DefaultChannel,
                Username = _username,
                DisplayName = _displayName,
                RawMessage = trimmed,
                IsMe = true
            };

            RaiseMessageReceived(chatMessage);
        }

        private void AddSystemMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            _log.Add(message);
            if (_log.Count > _maxLogEntries * 2)
                _log.RemoveRange(0, _log.Count - _maxLogEntries * 2);
        }

        private void EnsureStyles()
        {
            if (_logStyle == null)
            {
                _logStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13
                };
            }

            if (_inputStyle == null)
            {
                _inputStyle = new GUIStyle(GUI.skin.textField)
                {
                    fontSize = 13
                };
            }

            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 12
                };
            }
        }
    }
}
