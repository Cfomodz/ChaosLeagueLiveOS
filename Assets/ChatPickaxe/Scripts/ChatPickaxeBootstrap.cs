using LiveChat.Commands;
using UnityEngine;

namespace ChatPickaxe
{
    public class ChatPickaxeBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            if (FindObjectsOfType<ChatPickaxeBootstrap>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            BuildGame();
        }

        private void BuildGame()
        {
            GameObject root = new GameObject("ChatPickaxeRoot");
            root.transform.SetParent(transform);

            root.AddComponent<ChatPickaxeRenderer>();
            ChatPickaxeGame game = root.AddComponent<ChatPickaxeGame>();
            root.AddComponent<ChatPickaxeHud>();

            GameObject chatRoot = new GameObject("ChatPickaxeChat");
            chatRoot.transform.SetParent(transform);

            ChatPickaxeLocalChatClient chatClient = chatRoot.AddComponent<ChatPickaxeLocalChatClient>();
            ChatCommandRouter router = chatRoot.AddComponent<ChatCommandRouter>();

            ChatPickaxeCommandHandler left = chatRoot.AddComponent<ChatPickaxeCommandHandler>();
            left.Configure(game, ChatPickaxeCommandType.Left);

            ChatPickaxeCommandHandler right = chatRoot.AddComponent<ChatPickaxeCommandHandler>();
            right.Configure(game, ChatPickaxeCommandType.Right);

            ChatPickaxeCommandHandler drop = chatRoot.AddComponent<ChatPickaxeCommandHandler>();
            drop.Configure(game, ChatPickaxeCommandType.Drop);

            ChatPickaxeCommandHandler restart = chatRoot.AddComponent<ChatPickaxeCommandHandler>();
            restart.Configure(game, ChatPickaxeCommandType.Restart);

            ChatPickaxeCommandHandler help = chatRoot.AddComponent<ChatPickaxeCommandHandler>();
            help.Configure(game, ChatPickaxeCommandType.Help);

            if (router != null)
                router.RefreshHandlers();

            if (chatClient != null)
                chatClient.Connect(new LiveChat.LiveChatConnectConfig { ChannelName = "local" });
        }
    }
}
