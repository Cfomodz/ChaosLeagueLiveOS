using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DigDugChatGame : MonoBehaviour
{
    private const int PixelsPerUnit = 16;
    private const int GridWidth = 20;
    private const int GridHeight = 15;
    private const int MaxQueuedActions = 30;
    private const int PumpRange = 3;
    private const int PumpHitsToDefeat = 3;
    private const float ActionInterval = 0.25f;
    private const float EnemyMoveInterval = 0.6f;
    private const float PumpDisplayDuration = 0.2f;

    private enum ActionType
    {
        Move,
        Pump,
        Reset
    }

    private struct QueuedAction
    {
        public ActionType Type;
        public Vector2Int Direction;
        public string Username;
        public string Description;
    }

    private class EnemyState
    {
        public Vector2Int Position;
        public GameObject GameObject;
        public SpriteRenderer Renderer;
        public int PumpLevel;
        public float StunnedUntil;
    }

    private class PumpSegment
    {
        public GameObject GameObject;
        public float HideAt;
    }

    [SerializeField] private bool _allowKeyboardInput = true;

    private Grid _grid;
    private Tilemap _dirtTilemap;
    private Tile _dirtTile;
    private Transform _pumpRoot;

    private Sprite _dirtSprite;
    private Sprite _playerSprite;
    private Sprite _enemySprite;
    private Sprite _pumpSprite;

    private bool[,] _dirt;
    private readonly Queue<QueuedAction> _actionQueue = new Queue<QueuedAction>();
    private readonly List<EnemyState> _enemies = new List<EnemyState>();
    private readonly List<PumpSegment> _pumpSegments = new List<PumpSegment>();

    private GameObject _player;
    private SpriteRenderer _playerRenderer;
    private Vector2Int _playerPos;
    private Vector2Int _playerDir = Vector2Int.right;

    private int _score;
    private bool _gameOver;
    private bool _gameWon;
    private string _lastActionUser = string.Empty;
    private string _lastActionDescription = string.Empty;
    private float _nextActionTime;
    private float _nextEnemyMoveTime;

    private GUIStyle _hudStyle;
    private GUIStyle _hudHeaderStyle;

    private void Awake()
    {
        EnsureSprites();
        EnsureWorld();
        ResetGameState();
        SetupCamera();
    }

    private void Update()
    {
        if (_allowKeyboardInput)
            HandleKeyboardInput();

        if (Time.time >= _nextActionTime && _actionQueue.Count > 0)
        {
            QueuedAction action = _actionQueue.Dequeue();
            ExecuteAction(action);
            _nextActionTime = Time.time + ActionInterval;
        }

        if (_gameOver || _gameWon)
        {
            DrainPumpVfx();
            return;
        }

        if (Time.time >= _nextEnemyMoveTime)
        {
            MoveEnemies();
            _nextEnemyMoveTime = Time.time + EnemyMoveInterval;
        }

        DrainPumpVfx();
    }

    public bool EnqueueMove(Vector2Int direction, string username)
    {
        if (_gameOver || _gameWon)
            return false;

        if (!IsCardinal(direction))
            return false;

        if (_actionQueue.Count >= MaxQueuedActions)
            return false;

        _actionQueue.Enqueue(new QueuedAction
        {
            Type = ActionType.Move,
            Direction = direction,
            Username = username,
            Description = $"dig {DirectionToLabel(direction)}"
        });

        return true;
    }

    public bool EnqueuePump(string username)
    {
        if (_gameOver || _gameWon)
            return false;

        if (_actionQueue.Count >= MaxQueuedActions)
            return false;

        _actionQueue.Enqueue(new QueuedAction
        {
            Type = ActionType.Pump,
            Direction = _playerDir,
            Username = username,
            Description = "pump"
        });

        return true;
    }

    public bool EnqueueReset(string username)
    {
        if (!_gameOver && !_gameWon)
            return false;

        _actionQueue.Clear();
        _actionQueue.Enqueue(new QueuedAction
        {
            Type = ActionType.Reset,
            Direction = Vector2Int.zero,
            Username = username,
            Description = "reset"
        });

        return true;
    }

    private void ExecuteAction(QueuedAction action)
    {
        _lastActionUser = string.IsNullOrEmpty(action.Username) ? "Anonymous" : action.Username;
        _lastActionDescription = action.Description;

        switch (action.Type)
        {
            case ActionType.Move:
                TryMovePlayer(action.Direction);
                break;
            case ActionType.Pump:
                TryPump(action.Direction);
                break;
            case ActionType.Reset:
                ResetGameState();
                break;
        }
    }

    private void EnsureSprites()
    {
        if (_playerSprite != null)
            return;

        _dirtSprite = LoadSpriteOrGenerate("DigDugChat/digdug_dirt", CreateDirtTexture);
        _playerSprite = LoadSpriteOrGenerate("DigDugChat/digdug_player", CreatePlayerTexture);
        _enemySprite = LoadSpriteOrGenerate("DigDugChat/digdug_enemy", CreateEnemyTexture);
        _pumpSprite = LoadSpriteOrGenerate("DigDugChat/digdug_pump", CreatePumpTexture);
    }

    private Sprite LoadSpriteOrGenerate(string resourcePath, Func<Texture2D> generator)
    {
        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
            return sprite;

        Texture2D texture = generator();
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
    }

    private void EnsureWorld()
    {
        if (_grid == null)
        {
            GameObject gridObject = new GameObject("DigDugGrid");
            gridObject.transform.SetParent(transform);
            _grid = gridObject.AddComponent<Grid>();
            _grid.cellSize = Vector3.one;

            GameObject dirtObject = new GameObject("Dirt");
            dirtObject.transform.SetParent(gridObject.transform);
            _dirtTilemap = dirtObject.AddComponent<Tilemap>();
            TilemapRenderer renderer = dirtObject.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = 0;
        }

        if (_pumpRoot == null)
        {
            GameObject pumpObject = new GameObject("PumpVfx");
            pumpObject.transform.SetParent(transform);
            _pumpRoot = pumpObject.transform;
        }

        if (_player == null)
        {
            _player = new GameObject("Player");
            _player.transform.SetParent(transform);
            _playerRenderer = _player.AddComponent<SpriteRenderer>();
            _playerRenderer.sortingOrder = 5;
        }

        if (_playerRenderer == null)
            _playerRenderer = _player.GetComponent<SpriteRenderer>();

        if (_playerRenderer != null)
            _playerRenderer.sprite = _playerSprite;

        if (_dirtTile == null)
        {
            _dirtTile = ScriptableObject.CreateInstance<Tile>();
            _dirtTile.sprite = _dirtSprite;
            _dirtTile.color = Color.white;
        }
    }

    private void ResetGameState()
    {
        _actionQueue.Clear();
        _score = 0;
        _gameOver = false;
        _gameWon = false;
        _lastActionUser = string.Empty;
        _lastActionDescription = string.Empty;
        _playerDir = Vector2Int.right;
        _nextActionTime = Time.time + ActionInterval;
        _nextEnemyMoveTime = Time.time + EnemyMoveInterval;
        if (_playerRenderer != null)
            _playerRenderer.color = Color.white;

        BuildDirtMap();
        SpawnPlayer();
        SpawnEnemies();
    }

    private void BuildDirtMap()
    {
        _dirt = new bool[GridWidth, GridHeight];
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
                _dirt[x, y] = true;
        }

        Vector2Int start = new Vector2Int(GridWidth / 2, GridHeight - 2);
        CarveRect(start.x - 1, start.y - 1, 3, 3);
        CarveVertical(start.x, 0, GridHeight - 1);

        int midY = GridHeight / 2;
        CarveHorizontal(1, GridWidth - 2, midY);

        CarveRect(start.x - 2, 1, 5, 3);

        if (_dirtTilemap != null)
        {
            _dirtTilemap.ClearAllTiles();
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    if (_dirt[x, y])
                        _dirtTilemap.SetTile(new Vector3Int(x, y, 0), _dirtTile);
                }
            }
        }
    }

    private void SpawnPlayer()
    {
        _playerPos = new Vector2Int(GridWidth / 2, GridHeight - 2);
        UpdatePlayerWorldPosition();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].GameObject != null)
                Destroy(_enemies[i].GameObject);
        }
        _enemies.Clear();

        Vector2Int[] spawnPositions =
        {
            new Vector2Int(GridWidth / 2 - 2, 1),
            new Vector2Int(GridWidth / 2, 1),
            new Vector2Int(GridWidth / 2 + 2, 1)
        };

        foreach (Vector2Int pos in spawnPositions)
        {
            if (InBounds(pos))
                CreateEnemy(pos);
        }
    }

    private void CreateEnemy(Vector2Int position)
    {
        GameObject enemyObject = new GameObject($"Enemy_{_enemies.Count + 1}");
        enemyObject.transform.SetParent(transform);
        SpriteRenderer renderer = enemyObject.AddComponent<SpriteRenderer>();
        renderer.sprite = _enemySprite;
        renderer.sortingOrder = 4;
        enemyObject.transform.position = CellToWorld(position);

        _enemies.Add(new EnemyState
        {
            Position = position,
            GameObject = enemyObject,
            Renderer = renderer,
            PumpLevel = 0,
            StunnedUntil = 0f
        });
    }

    private void TryMovePlayer(Vector2Int direction)
    {
        Vector2Int target = _playerPos + direction;
        if (!InBounds(target))
            return;

        EnemyState enemyAtTarget = FindEnemyAt(target);
        if (enemyAtTarget != null)
        {
            TriggerGameOver();
            return;
        }

        if (_dirt[target.x, target.y])
            DigTile(target);

        _playerPos = target;
        _playerDir = direction;
        UpdatePlayerWorldPosition();
    }

    private void TryPump(Vector2Int direction)
    {
        if (!IsCardinal(direction))
            direction = _playerDir;

        if (!IsCardinal(direction))
            return;

        float expiry = Time.time + PumpDisplayDuration;
        for (int step = 1; step <= PumpRange; step++)
        {
            Vector2Int pos = _playerPos + direction * step;
            if (!InBounds(pos))
                break;

            if (_dirt[pos.x, pos.y])
                break;

            ShowPumpSegment(pos, expiry);

            EnemyState enemy = FindEnemyAt(pos);
            if (enemy != null)
            {
                ApplyPump(enemy);
                break;
            }
        }
    }

    private void ApplyPump(EnemyState enemy)
    {
        enemy.PumpLevel++;
        enemy.StunnedUntil = Time.time + 0.5f;
        enemy.Renderer.color = GetPumpTint(enemy.PumpLevel);

        if (enemy.PumpLevel >= PumpHitsToDefeat)
        {
            _score += 100;
            if (enemy.GameObject != null)
                Destroy(enemy.GameObject);
            _enemies.Remove(enemy);
            if (_enemies.Count == 0)
                TriggerWin();
        }
    }

    private void MoveEnemies()
    {
        if (_enemies.Count == 0)
            return;

        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
        for (int i = 0; i < _enemies.Count; i++)
            occupied.Add(_enemies[i].Position);

        for (int i = 0; i < _enemies.Count; i++)
        {
            EnemyState enemy = _enemies[i];
            if (Time.time < enemy.StunnedUntil)
                continue;

            Vector2Int direction = ChooseEnemyDirection(enemy, occupied);
            if (direction == Vector2Int.zero)
                continue;

            Vector2Int target = enemy.Position + direction;
            if (target == _playerPos)
            {
                TriggerGameOver();
                return;
            }

            occupied.Remove(enemy.Position);
            enemy.Position = target;
            occupied.Add(enemy.Position);
            if (enemy.GameObject != null)
                enemy.GameObject.transform.position = CellToWorld(enemy.Position);
        }
    }

    private Vector2Int ChooseEnemyDirection(EnemyState enemy, HashSet<Vector2Int> occupied)
    {
        if (HasLineOfSight(enemy.Position, _playerPos))
        {
            Vector2Int desired = GetChaseDirection(enemy.Position, _playerPos);
            if (CanEnemyMoveTo(enemy.Position + desired, occupied))
                return desired;
        }

        List<Vector2Int> options = new List<Vector2Int>(4);
        AddIfAvailable(options, enemy.Position + Vector2Int.up, Vector2Int.up, occupied);
        AddIfAvailable(options, enemy.Position + Vector2Int.down, Vector2Int.down, occupied);
        AddIfAvailable(options, enemy.Position + Vector2Int.left, Vector2Int.left, occupied);
        AddIfAvailable(options, enemy.Position + Vector2Int.right, Vector2Int.right, occupied);

        if (options.Count == 0)
            return Vector2Int.zero;

        return options[UnityEngine.Random.Range(0, options.Count)];
    }

    private void AddIfAvailable(List<Vector2Int> options, Vector2Int target, Vector2Int direction, HashSet<Vector2Int> occupied)
    {
        if (CanEnemyMoveTo(target, occupied))
            options.Add(direction);
    }

    private bool CanEnemyMoveTo(Vector2Int target, HashSet<Vector2Int> occupied)
    {
        if (!InBounds(target))
            return false;

        if (_dirt[target.x, target.y])
            return false;

        if (occupied.Contains(target))
            return false;

        return true;
    }

    private bool HasLineOfSight(Vector2Int from, Vector2Int to)
    {
        if (from.x == to.x)
        {
            int start = Mathf.Min(from.y, to.y) + 1;
            int end = Mathf.Max(from.y, to.y);
            for (int y = start; y < end; y++)
            {
                if (_dirt[from.x, y])
                    return false;
            }
            return true;
        }

        if (from.y == to.y)
        {
            int start = Mathf.Min(from.x, to.x) + 1;
            int end = Mathf.Max(from.x, to.x);
            for (int x = start; x < end; x++)
            {
                if (_dirt[x, from.y])
                    return false;
            }
            return true;
        }

        return false;
    }

    private Vector2Int GetChaseDirection(Vector2Int from, Vector2Int to)
    {
        if (from.x == to.x)
            return to.y > from.y ? Vector2Int.up : Vector2Int.down;
        return to.x > from.x ? Vector2Int.right : Vector2Int.left;
    }

    private EnemyState FindEnemyAt(Vector2Int position)
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].Position == position)
                return _enemies[i];
        }
        return null;
    }

    private void DigTile(Vector2Int position)
    {
        if (!InBounds(position))
            return;

        if (!_dirt[position.x, position.y])
            return;

        _dirt[position.x, position.y] = false;
        if (_dirtTilemap != null)
            _dirtTilemap.SetTile(new Vector3Int(position.x, position.y, 0), null);
    }

    private void UpdatePlayerWorldPosition()
    {
        if (_player == null)
            return;

        _player.transform.position = CellToWorld(_playerPos);
    }

    private Vector3 CellToWorld(Vector2Int cell)
    {
        if (_dirtTilemap != null)
            return _dirtTilemap.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));

        return new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f);
    }

    private void ShowPumpSegment(Vector2Int position, float expiry)
    {
        PumpSegment segment = GetPumpSegment();
        segment.HideAt = expiry;
        segment.GameObject.transform.position = CellToWorld(position);
        segment.GameObject.SetActive(true);
    }

    private PumpSegment GetPumpSegment()
    {
        for (int i = 0; i < _pumpSegments.Count; i++)
        {
            if (!_pumpSegments[i].GameObject.activeSelf)
                return _pumpSegments[i];
        }

        GameObject segmentObject = new GameObject("PumpSegment");
        segmentObject.transform.SetParent(_pumpRoot);
        SpriteRenderer renderer = segmentObject.AddComponent<SpriteRenderer>();
        renderer.sprite = _pumpSprite;
        renderer.sortingOrder = 6;
        PumpSegment segment = new PumpSegment
        {
            GameObject = segmentObject,
            HideAt = 0f
        };
        _pumpSegments.Add(segment);
        return segment;
    }

    private void DrainPumpVfx()
    {
        for (int i = 0; i < _pumpSegments.Count; i++)
        {
            PumpSegment segment = _pumpSegments[i];
            if (segment.GameObject.activeSelf && Time.time >= segment.HideAt)
                segment.GameObject.SetActive(false);
        }
    }

    private void TriggerGameOver()
    {
        _gameOver = true;
        _actionQueue.Clear();
        if (_playerRenderer != null)
            _playerRenderer.color = new Color(0.6f, 0.6f, 0.6f);
    }

    private void TriggerWin()
    {
        _gameWon = true;
        _actionQueue.Clear();
        if (_playerRenderer != null)
            _playerRenderer.color = new Color(0.6f, 1f, 0.6f);
    }

    private bool InBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < GridWidth && position.y >= 0 && position.y < GridHeight;
    }

    private static bool IsCardinal(Vector2Int direction)
    {
        return direction == Vector2Int.up || direction == Vector2Int.down || direction == Vector2Int.left || direction == Vector2Int.right;
    }

    private static string DirectionToLabel(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            return "up";
        if (direction == Vector2Int.down)
            return "down";
        if (direction == Vector2Int.left)
            return "left";
        if (direction == Vector2Int.right)
            return "right";
        return "none";
    }

    private Color GetPumpTint(int pumpLevel)
    {
        if (pumpLevel <= 0)
            return Color.white;
        if (pumpLevel == 1)
            return new Color(1f, 0.9f, 0.6f);
        if (pumpLevel == 2)
            return new Color(1f, 0.7f, 0.4f);
        return new Color(1f, 0.4f, 0.4f);
    }

    private void CarveRect(int xMin, int yMin, int width, int height)
    {
        for (int x = xMin; x < xMin + width; x++)
        {
            for (int y = yMin; y < yMin + height; y++)
                CarveTile(new Vector2Int(x, y));
        }
    }

    private void CarveVertical(int x, int yMin, int yMax)
    {
        for (int y = yMin; y <= yMax; y++)
            CarveTile(new Vector2Int(x, y));
    }

    private void CarveHorizontal(int xMin, int xMax, int y)
    {
        for (int x = xMin; x <= xMax; x++)
            CarveTile(new Vector2Int(x, y));
    }

    private void CarveTile(Vector2Int position)
    {
        if (!InBounds(position))
            return;

        _dirt[position.x, position.y] = false;
    }

    private void SetupCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.08f, 0.1f);
        camera.transform.position = new Vector3(GridWidth / 2f, GridHeight / 2f, -10f);

        float verticalSize = GridHeight / 2f + 1f;
        float horizontalSize = GridWidth / 2f / camera.aspect + 1f;
        camera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            EnqueueMove(Vector2Int.up, "Keyboard");
        if (Input.GetKeyDown(KeyCode.DownArrow))
            EnqueueMove(Vector2Int.down, "Keyboard");
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            EnqueueMove(Vector2Int.left, "Keyboard");
        if (Input.GetKeyDown(KeyCode.RightArrow))
            EnqueueMove(Vector2Int.right, "Keyboard");
        if (Input.GetKeyDown(KeyCode.Space))
            EnqueuePump("Keyboard");
        if (Input.GetKeyDown(KeyCode.R))
            EnqueueReset("Keyboard");
    }

    private void EnsureHudStyles()
    {
        if (_hudStyle != null)
            return;

        _hudStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            normal = { textColor = Color.white }
        };

        _hudHeaderStyle = new GUIStyle(_hudStyle)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };
    }

    private void OnGUI()
    {
        EnsureHudStyles();

        GUILayout.BeginArea(new Rect(12f, 12f, 520f, 220f));
        GUILayout.Label("Dig Dug Chat (KISS)", _hudHeaderStyle);
        GUILayout.Label("Commands: !dig <up|down|left|right>, !pump, !start", _hudStyle);
        GUILayout.Label($"Score: {_score}  Enemies: {_enemies.Count}", _hudStyle);
        GUILayout.Label($"Queued actions: {_actionQueue.Count}", _hudStyle);

        if (!string.IsNullOrEmpty(_lastActionUser))
            GUILayout.Label($"Last action: {_lastActionUser} -> {_lastActionDescription}", _hudStyle);

        if (_gameOver)
            GUILayout.Label("Game Over! Use !start to play again.", _hudStyle);
        if (_gameWon)
            GUILayout.Label("You cleared the caves! Use !start to play again.", _hudStyle);

        GUILayout.EndArea();
    }

    private Texture2D CreateDirtTexture()
    {
        const int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color32 baseColor = new Color32(120, 80, 42, 255);
        Color32 dark = new Color32(95, 62, 30, 255);
        Color32 light = new Color32(150, 102, 58, 255);

        Color32[] pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Color32 color = baseColor;
                int noise = (x * 3 + y * 5) % 13;
                if (noise == 0 || noise == 1)
                    color = dark;
                else if (noise == 2)
                    color = light;
                pixels[y * size + x] = color;
            }
        }

        texture.SetPixels32(pixels);
        return texture;
    }

    private Texture2D CreatePlayerTexture()
    {
        const int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color32 transparent = new Color32(0, 0, 0, 0);
        Color32 face = new Color32(255, 214, 170, 255);
        Color32 body = new Color32(72, 140, 255, 255);
        Color32 eye = new Color32(30, 30, 30, 255);

        Color32[] pixels = new Color32[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = transparent;

        DrawRect(pixels, size, 5, 9, 6, 4, face);
        DrawRect(pixels, size, 6, 10, 1, 1, eye);
        DrawRect(pixels, size, 9, 10, 1, 1, eye);

        DrawRect(pixels, size, 6, 4, 4, 4, body);
        DrawRect(pixels, size, 5, 3, 2, 1, body);
        DrawRect(pixels, size, 9, 3, 2, 1, body);

        texture.SetPixels32(pixels);
        return texture;
    }

    private Texture2D CreateEnemyTexture()
    {
        const int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color32 transparent = new Color32(0, 0, 0, 0);
        Color32 body = new Color32(240, 92, 72, 255);
        Color32 eyeWhite = new Color32(255, 255, 255, 255);
        Color32 eye = new Color32(20, 20, 20, 255);

        Color32[] pixels = new Color32[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = transparent;

        Vector2 center = new Vector2(7.5f, 7.5f);
        float radius = 6.2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center.x;
                float dy = y - center.y;
                if (dx * dx + dy * dy <= radius * radius)
                    pixels[y * size + x] = body;
            }
        }

        DrawRect(pixels, size, 5, 9, 2, 2, eyeWhite);
        DrawRect(pixels, size, 9, 9, 2, 2, eyeWhite);
        DrawRect(pixels, size, 6, 9, 1, 1, eye);
        DrawRect(pixels, size, 10, 9, 1, 1, eye);

        texture.SetPixels32(pixels);
        return texture;
    }

    private Texture2D CreatePumpTexture()
    {
        const int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color32 transparent = new Color32(0, 0, 0, 0);
        Color32 body = new Color32(255, 224, 110, 255);
        Color32 highlight = new Color32(255, 244, 190, 255);

        Color32[] pixels = new Color32[size * size];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = transparent;

        Vector2 center = new Vector2(7.5f, 7.5f);
        float radius = 5.2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center.x;
                float dy = y - center.y;
                if (dx * dx + dy * dy <= radius * radius)
                    pixels[y * size + x] = body;
            }
        }

        DrawRect(pixels, size, 9, 11, 2, 2, highlight);

        texture.SetPixels32(pixels);
        return texture;
    }

    private void DrawRect(Color32[] pixels, int width, int x, int y, int rectWidth, int rectHeight, Color32 color)
    {
        for (int iy = y; iy < y + rectHeight; iy++)
        {
            for (int ix = x; ix < x + rectWidth; ix++)
            {
                if (ix < 0 || iy < 0 || ix >= width || iy >= width)
                    continue;
                pixels[iy * width + ix] = color;
            }
        }
    }
}
