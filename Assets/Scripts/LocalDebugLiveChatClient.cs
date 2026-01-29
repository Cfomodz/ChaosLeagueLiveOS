using LiveChat;
using UnityEngine;

public class LocalDebugLiveChatClient : LiveChatClientBase
{
    [SerializeField] private string _defaultChannel = "local";
    [SerializeField] private string _username = "LocalUser";
    [SerializeField] private bool _autoConnect = true;
    [SerializeField] private bool _showInput = true;
    [SerializeField] private string _input = "!dig right";

    private void Awake()
    {
        DefaultChannel = _defaultChannel;
        if (_autoConnect)
            Connect(new LiveChatConnectConfig { ChannelName = _defaultChannel });
    }

    public override void Connect(LiveChatConnectConfig config)
    {
        DefaultChannel = string.IsNullOrEmpty(config?.ChannelName) ? _defaultChannel : config.ChannelName;
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
        Debug.Log($"[LocalChat:{channel}] {message}");
    }

    public override void SendReply(string channel, string replyToMessageId, string message)
    {
        Debug.Log($"[LocalChat:{channel}] reply to {replyToMessageId}: {message}");
    }

    public void SimulateIncoming(string rawMessage)
    {
        if (string.IsNullOrWhiteSpace(rawMessage))
            return;

        if (!IsConnected)
            Connect(new LiveChatConnectConfig { ChannelName = _defaultChannel });

        LiveChatMessage message = new LiveChatMessage
        {
            MessageId = System.Guid.NewGuid().ToString("N"),
            Username = _username,
            DisplayName = _username,
            Channel = DefaultChannel,
            RawMessage = rawMessage,
            RawIrcMessage = rawMessage
        };

        RaiseMessageReceived(message);
    }

    private void OnGUI()
    {
        if (!_showInput)
            return;

        const float width = 520f;
        const float height = 64f;
        Rect area = new Rect(12f, Screen.height - height - 12f, width, height);

        GUILayout.BeginArea(area, GUI.skin.box);
        GUILayout.Label("Local chat (debug): type commands like !dig left, !pump, !start");
        GUI.SetNextControlName("LocalChatInput");
        _input = GUILayout.TextField(_input, 128);
        bool send = GUILayout.Button("Send");

        Event current = Event.current;
        if (current != null && current.type == EventType.KeyDown && current.keyCode == KeyCode.Return)
        {
            if (GUI.GetNameOfFocusedControl() == "LocalChatInput")
            {
                send = true;
                current.Use();
            }
        }

        if (send)
        {
            SimulateIncoming(_input);
            _input = string.Empty;
        }

        GUILayout.EndArea();
    }
}
