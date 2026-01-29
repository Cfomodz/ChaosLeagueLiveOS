using System.Collections.Generic;
using UnityEngine;

namespace ChatPickaxe
{
    public class ChatPickaxeRenderer : MonoBehaviour
    {
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private int _pixelsPerUnit = 16;
        [SerializeField] private Color _backgroundColor = new Color(0.08f, 0.08f, 0.1f, 1f);

        private int _width;
        private int _height;
        private Vector2 _origin;

        private Sprite _oreSprite;
        private Sprite _pickaxeSprite;

        private SpriteRenderer[,] _oreRenderers;
        private SpriteRenderer _pickaxeRenderer;
        private Transform _boardRoot;

        private static readonly string[] PickaxeArt =
        {
            "................",
            ".....bbbb.......",
            "....bbbbbb......",
            "...bbb..bbb.....",
            "..bbb....bbb....",
            ".bbb.....bbb....",
            "bbb......bbb....",
            "bb.......bbb....",
            "b........bbb.hh.",
            ".........bbb.hh.",
            ".........bbb.hh.",
            ".........bbb.hh.",
            ".........bbb.hh.",
            ".........bbb.hh.",
            ".........bbb.hh.",
            ".........bbb.hh."
        };

        private static readonly string[] OreArt =
        {
            "################",
            "##o######o######",
            "#####o######o###",
            "###o####o#######",
            "################",
            "#o######o#######",
            "######o#####o###",
            "################",
            "###o######o#####",
            "######o#######o#",
            "################",
            "##o#######o#####",
            "#####o######o###",
            "################",
            "###o#######o####",
            "################"
        };

        public void Initialize(int width, int height)
        {
            _width = width;
            _height = height;

            BuildSpritesIfNeeded();
            BuildBoard();
            ApplyCameraSettings();
        }

        public void Render(bool[,] ore, Vector2Int pickaxeCell, bool pickaxeActive)
        {
            if (_oreRenderers == null || ore == null)
                return;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    SpriteRenderer renderer = _oreRenderers[x, y];
                    if (renderer != null)
                        renderer.enabled = ore[x, y];
                }
            }

            if (_pickaxeRenderer != null)
            {
                _pickaxeRenderer.enabled = pickaxeActive;
                if (pickaxeActive)
                    _pickaxeRenderer.transform.position = CellToWorld(pickaxeCell);
            }
        }

        private void BuildSpritesIfNeeded()
        {
            if (_oreSprite == null)
                _oreSprite = CreateSprite(OreArt, CreateOrePalette(), _pixelsPerUnit);

            if (_pickaxeSprite == null)
                _pickaxeSprite = CreateSprite(PickaxeArt, CreatePickaxePalette(), _pixelsPerUnit);
        }

        private void BuildBoard()
        {
            if (_boardRoot != null)
                Destroy(_boardRoot.gameObject);

            _boardRoot = new GameObject("Board").transform;
            _boardRoot.SetParent(transform);

            _origin = new Vector2(-_width * _cellSize * 0.5f, -_height * _cellSize * 0.5f);

            _oreRenderers = new SpriteRenderer[_width, _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    GameObject tile = new GameObject($"Ore_{x}_{y}");
                    tile.transform.SetParent(_boardRoot);
                    tile.transform.position = CellToWorld(new Vector2Int(x, y));

                    SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
                    renderer.sprite = _oreSprite;
                    renderer.enabled = false;
                    _oreRenderers[x, y] = renderer;
                }
            }

            if (_pickaxeRenderer == null)
            {
                GameObject pickaxe = new GameObject("Pickaxe");
                pickaxe.transform.SetParent(_boardRoot);
                _pickaxeRenderer = pickaxe.AddComponent<SpriteRenderer>();
            }

            _pickaxeRenderer.sprite = _pickaxeSprite;
            _pickaxeRenderer.sortingOrder = 5;
        }

        private void ApplyCameraSettings()
        {
            Camera cam = Camera.main;
            if (cam == null)
                cam = FindObjectOfType<Camera>();

            if (cam == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cam = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
                cam.tag = "MainCamera";
            }

            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = _backgroundColor;
            cam.transform.position = new Vector3(0f, 0f, -10f);

            float worldHeight = _height * _cellSize;
            cam.orthographicSize = worldHeight * 0.5f + _cellSize;
        }

        private Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(
                _origin.x + (cell.x + 0.5f) * _cellSize,
                _origin.y + (cell.y + 0.5f) * _cellSize,
                0f);
        }

        private static Sprite CreateSprite(string[] rows, Dictionary<char, Color> palette, int pixelsPerUnit)
        {
            if (rows == null || rows.Length == 0)
                return null;

            int height = rows.Length;
            int width = rows[0].Length;

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                string row = rows[height - 1 - y];
                for (int x = 0; x < width; x++)
                {
                    Color color = Color.clear;
                    if (x < row.Length)
                    {
                        char key = row[x];
                        if (key != '.' && palette.TryGetValue(key, out Color paletteColor))
                            color = paletteColor;
                    }

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }

        private static Dictionary<char, Color> CreatePickaxePalette()
        {
            return new Dictionary<char, Color>
            {
                { 'b', new Color(0.78f, 0.8f, 0.86f, 1f) },
                { 'h', new Color(0.55f, 0.35f, 0.18f, 1f) }
            };
        }

        private static Dictionary<char, Color> CreateOrePalette()
        {
            return new Dictionary<char, Color>
            {
                { '#', new Color(0.32f, 0.34f, 0.38f, 1f) },
                { 'o', new Color(0.44f, 0.46f, 0.5f, 1f) }
            };
        }
    }
}
