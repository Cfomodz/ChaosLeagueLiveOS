using LiveChat.Commands;
using UnityEngine;

namespace ChatPickaxe
{
    public enum ChatPickaxeCommandType
    {
        Left,
        Right,
        Drop,
        Restart,
        Help
    }

    public class ChatPickaxeCommandHandler : ChatCommandHandler
    {
        [SerializeField] private ChatPickaxeGame _game;
        [SerializeField] private ChatPickaxeCommandType _commandType = ChatPickaxeCommandType.Left;
        [SerializeField] private float _cooldownSeconds = 0.1f;

        private float _lastExecuteTime = -999f;

        public void Configure(ChatPickaxeGame game, ChatPickaxeCommandType commandType)
        {
            _game = game;
            _commandType = commandType;
            ApplyDefaults();
        }

        private void Awake()
        {
            if (_game == null)
                _game = FindObjectOfType<ChatPickaxeGame>();

            ApplyDefaults();
        }

        public override void Execute(ChatCommandContext context)
        {
            if (_game == null)
                return;

            float now = Time.unscaledTime;
            if (_cooldownSeconds > 0f && now - _lastExecuteTime < _cooldownSeconds)
                return;

            _lastExecuteTime = now;

            switch (_commandType)
            {
                case ChatPickaxeCommandType.Left:
                    _game.CommandLeft();
                    break;
                case ChatPickaxeCommandType.Right:
                    _game.CommandRight();
                    break;
                case ChatPickaxeCommandType.Drop:
                    _game.CommandDrop();
                    break;
                case ChatPickaxeCommandType.Restart:
                    _game.CommandRestart();
                    break;
                case ChatPickaxeCommandType.Help:
                    context?.Reply("Commands: !left !right !drop !restart");
                    break;
            }
        }

        private void ApplyDefaults()
        {
            switch (_commandType)
            {
                case ChatPickaxeCommandType.Left:
                    SetDefaults("left", "Slide the pickaxe left.", "!left", ChatCommandPermission.Anyone, "l");
                    break;
                case ChatPickaxeCommandType.Right:
                    SetDefaults("right", "Slide the pickaxe right.", "!right", ChatCommandPermission.Anyone, "r");
                    break;
                case ChatPickaxeCommandType.Drop:
                    SetDefaults("drop", "Drop the pickaxe one cell.", "!drop", ChatCommandPermission.Anyone, "d");
                    break;
                case ChatPickaxeCommandType.Restart:
                    SetDefaults("restart", "Restart the mine.", "!restart", ChatCommandPermission.Anyone, "reset");
                    break;
                case ChatPickaxeCommandType.Help:
                    SetDefaults("help", "Show pickaxe commands.", "!help", ChatCommandPermission.Anyone, "commands");
                    break;
            }
        }
    }
}
