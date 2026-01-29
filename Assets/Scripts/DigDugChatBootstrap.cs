using LiveChat.Commands;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DigDugChatBootstrap
{
    private const string SceneName = "DigDugChat";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || scene.name != SceneName)
            return;

        if (Object.FindObjectOfType<DigDugChatGame>() != null)
            return;

        GameObject root = new GameObject("DigDugChatRoot");
        root.AddComponent<DigDugChatGame>();
        root.AddComponent<LocalDebugLiveChatClient>();
        ChatCommandRouter router = root.AddComponent<ChatCommandRouter>();
        root.AddComponent<DigDugMoveCommand>();
        root.AddComponent<DigDugPumpCommand>();
        root.AddComponent<DigDugResetCommand>();
        root.AddComponent<DigDugHelpCommand>();

        router.RefreshHandlers();
    }
}
