using UnityEngine;

namespace ChatPickaxe
{
    public class ChatPickaxeHud : MonoBehaviour
    {
        [SerializeField] private ChatPickaxeGame _game;
        [SerializeField] private int _padding = 12;
        [SerializeField] private float _lineHeight = 20f;

        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;

        private void Awake()
        {
            if (_game == null)
                _game = GetComponent<ChatPickaxeGame>();
        }

        private void OnGUI()
        {
            if (_game == null)
                return;

            EnsureStyles();

            float x = _padding;
            float y = _padding;

            GUI.Label(new Rect(x, y, 400f, _lineHeight), "Chat Pickaxe", _titleStyle);
            y += _lineHeight;

            GUI.Label(new Rect(x, y, 400f, _lineHeight), $"Score: {_game.Score}", _labelStyle);
            y += _lineHeight;
            GUI.Label(new Rect(x, y, 400f, _lineHeight), $"Mined: {_game.Mined}", _labelStyle);
            y += _lineHeight;
            GUI.Label(new Rect(x, y, 400f, _lineHeight), $"Misses: {_game.Misses}", _labelStyle);
            y += _lineHeight;

            if (!_game.PickaxeActive)
            {
                GUI.Label(new Rect(x, y, 400f, _lineHeight), "Spawning pickaxe...", _labelStyle);
                y += _lineHeight;
            }

            y += _lineHeight * 0.5f;
            GUI.Label(new Rect(x, y, 500f, _lineHeight), "Commands: !left !right !drop !restart", _labelStyle);
        }

        private void EnsureStyles()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold
                };
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 14
                };
            }
        }
    }
}
