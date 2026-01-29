using LiveChat.Commands;
using UnityEngine;

public class DigDugMoveCommand : ChatCommandHandler
{
    private DigDugChatGame _game;

    private void Awake()
    {
        SetDefaults(
            command: "dig",
            description: "Digs one tile in a direction.",
            usage: "!dig <up|down|left|right>",
            minimumPermission: ChatCommandPermission.Anyone,
            aliases: new[] { "move", "m", "u", "d", "l", "r", "up", "down", "left", "right" });
    }

    public override void Execute(ChatCommandContext context)
    {
        DigDugChatGame game = ResolveGame(context);
        if (game == null)
            return;

        if (!TryGetDirection(context, out Vector2Int direction))
        {
            context.Reply("Usage: !dig up|down|left|right");
            return;
        }

        game.EnqueueMove(direction, context.DisplayName);
    }

    private DigDugChatGame ResolveGame(ChatCommandContext context)
    {
        if (_game == null)
            _game = Object.FindObjectOfType<DigDugChatGame>();
        if (_game == null)
            context?.Reply("Dig Dug game not ready.");
        return _game;
    }

    private static bool TryGetDirection(ChatCommandContext context, out Vector2Int direction)
    {
        direction = Vector2Int.zero;
        if (context == null)
            return false;

        if (TryParseDirectionToken(context.Command, out direction))
            return true;

        if (context.Args.Length > 0 && TryParseDirectionToken(context.Args[0], out direction))
            return true;

        return false;
    }

    private static bool TryParseDirectionToken(string token, out Vector2Int direction)
    {
        direction = Vector2Int.zero;
        if (string.IsNullOrEmpty(token))
            return false;

        switch (token.ToLowerInvariant())
        {
            case "u":
            case "up":
            case "north":
                direction = Vector2Int.up;
                return true;
            case "d":
            case "down":
            case "south":
                direction = Vector2Int.down;
                return true;
            case "l":
            case "left":
            case "west":
                direction = Vector2Int.left;
                return true;
            case "r":
            case "right":
            case "east":
                direction = Vector2Int.right;
                return true;
        }

        return false;
    }
}

public class DigDugPumpCommand : ChatCommandHandler
{
    private DigDugChatGame _game;

    private void Awake()
    {
        SetDefaults(
            command: "pump",
            description: "Pumps in the last move direction.",
            usage: "!pump",
            minimumPermission: ChatCommandPermission.Anyone,
            aliases: new[] { "fire" });
    }

    public override void Execute(ChatCommandContext context)
    {
        DigDugChatGame game = ResolveGame(context);
        if (game == null)
            return;

        game.EnqueuePump(context.DisplayName);
    }

    private DigDugChatGame ResolveGame(ChatCommandContext context)
    {
        if (_game == null)
            _game = Object.FindObjectOfType<DigDugChatGame>();
        if (_game == null)
            context?.Reply("Dig Dug game not ready.");
        return _game;
    }
}

public class DigDugResetCommand : ChatCommandHandler
{
    private DigDugChatGame _game;

    private void Awake()
    {
        SetDefaults(
            command: "start",
            description: "Restarts the run after a win or loss.",
            usage: "!start",
            minimumPermission: ChatCommandPermission.Anyone,
            aliases: new[] { "reset", "restart" });
    }

    public override void Execute(ChatCommandContext context)
    {
        DigDugChatGame game = ResolveGame(context);
        if (game == null)
            return;

        if (!game.EnqueueReset(context.DisplayName))
            context.Reply("Finish the run before restarting.");
    }

    private DigDugChatGame ResolveGame(ChatCommandContext context)
    {
        if (_game == null)
            _game = Object.FindObjectOfType<DigDugChatGame>();
        if (_game == null)
            context?.Reply("Dig Dug game not ready.");
        return _game;
    }
}

public class DigDugHelpCommand : ChatCommandHandler
{
    private void Awake()
    {
        SetDefaults(
            command: "dighelp",
            description: "Shows Dig Dug chat commands.",
            usage: "!dighelp",
            minimumPermission: ChatCommandPermission.Anyone,
            aliases: new[] { "digdug", "ddhelp" });
    }

    public override void Execute(ChatCommandContext context)
    {
        if (context == null)
            return;

        context.Reply("Commands: !dig <up|down|left|right>, !pump, !start");
    }
}
