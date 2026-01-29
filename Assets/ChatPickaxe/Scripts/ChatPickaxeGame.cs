using UnityEngine;

namespace ChatPickaxe
{
    [RequireComponent(typeof(ChatPickaxeRenderer))]
    public class ChatPickaxeGame : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private int _gridWidth = 7;
        [SerializeField] private int _gridHeight = 12;
        [SerializeField] private int _startingOreRows = 4;

        [Header("Timing")]
        [SerializeField] private float _fallInterval = 0.6f;
        [SerializeField] private float _spawnDelay = 0.2f;

        [Header("Scoring")]
        [SerializeField] private int _scorePerOre = 10;
        [SerializeField] private int _missPenalty = 0;

        private bool[,] _ore;
        private Vector2Int _pickaxeCell;
        private bool _pickaxeActive;
        private float _fallTimer;
        private float _spawnTimer;

        private int _score;
        private int _mined;
        private int _misses;

        private ChatPickaxeRenderer _renderer;

        public int Score => _score;
        public int Mined => _mined;
        public int Misses => _misses;
        public bool PickaxeActive => _pickaxeActive;
        public Vector2Int PickaxeCell => _pickaxeCell;
        public int GridWidth => _gridWidth;
        public int GridHeight => _gridHeight;

        private void Awake()
        {
            _renderer = GetComponent<ChatPickaxeRenderer>();
        }

        private void Start()
        {
            InitializeGrid();
            SpawnPickaxe();
            UpdateView();
        }

        private void Update()
        {
            if (!_pickaxeActive)
            {
                if (_spawnTimer > 0f)
                {
                    _spawnTimer -= Time.deltaTime;
                    if (_spawnTimer <= 0f)
                    {
                        SpawnPickaxe();
                        UpdateView();
                    }
                }

                return;
            }

            _fallTimer += Time.deltaTime;
            if (_fallTimer >= _fallInterval)
            {
                _fallTimer = 0f;
                StepDown();
            }
        }

        public void CommandLeft()
        {
            TryMove(-1);
        }

        public void CommandRight()
        {
            TryMove(1);
        }

        public void CommandDrop()
        {
            StepDown();
        }

        public void CommandRestart()
        {
            ResetGame();
        }

        private void ResetGame()
        {
            _score = 0;
            _mined = 0;
            _misses = 0;
            InitializeGrid();
            SpawnPickaxe();
            UpdateView();
        }

        private void InitializeGrid()
        {
            if (_gridWidth < 3)
                _gridWidth = 3;
            if (_gridHeight < 6)
                _gridHeight = 6;
            if (_startingOreRows < 1)
                _startingOreRows = 1;
            if (_startingOreRows >= _gridHeight)
                _startingOreRows = _gridHeight - 1;

            _ore = new bool[_gridWidth, _gridHeight];
            FillOreRows(_startingOreRows);

            _renderer.Initialize(_gridWidth, _gridHeight);
        }

        private void FillOreRows(int rows)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < _gridWidth; x++)
                    _ore[x, y] = true;
            }
        }

        private void SpawnPickaxe()
        {
            _pickaxeActive = false;
            _spawnTimer = 0f;
            _fallTimer = 0f;

            int startX = _gridWidth / 2;
            int foundX = -1;
            for (int offset = 0; offset < _gridWidth; offset++)
            {
                int x = (startX + offset) % _gridWidth;
                if (!IsOre(x, _gridHeight - 1))
                {
                    foundX = x;
                    break;
                }
            }

            if (foundX < 0)
            {
                _pickaxeActive = false;
                return;
            }

            _pickaxeCell = new Vector2Int(foundX, _gridHeight - 1);
            _pickaxeActive = true;
        }

        private void StepDown()
        {
            if (!_pickaxeActive)
                return;

            Vector2Int below = new Vector2Int(_pickaxeCell.x, _pickaxeCell.y - 1);
            if (below.y < 0)
            {
                MissPickaxe();
                return;
            }

            if (IsOre(below))
            {
                MineOre(below);
                return;
            }

            _pickaxeCell = below;
            UpdateView();
        }

        private void TryMove(int dx)
        {
            if (!_pickaxeActive)
                return;

            int targetX = _pickaxeCell.x + dx;
            if (targetX < 0 || targetX >= _gridWidth)
                return;

            if (IsOre(targetX, _pickaxeCell.y))
                return;

            _pickaxeCell = new Vector2Int(targetX, _pickaxeCell.y);
            UpdateView();
        }

        private bool IsOre(int x, int y)
        {
            if (_ore == null)
                return false;

            return _ore[x, y];
        }

        private bool IsOre(Vector2Int cell)
        {
            return IsOre(cell.x, cell.y);
        }

        private void MineOre(Vector2Int cell)
        {
            _ore[cell.x, cell.y] = false;
            _score += _scorePerOre;
            _mined++;
            ConsumePickaxe();

            if (!HasAnyOre())
                FillOreRows(_startingOreRows);
        }

        private bool HasAnyOre()
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                for (int x = 0; x < _gridWidth; x++)
                {
                    if (_ore[x, y])
                        return true;
                }
            }

            return false;
        }

        private void MissPickaxe()
        {
            if (_missPenalty != 0)
                _score = Mathf.Max(0, _score + _missPenalty);

            _misses++;
            ConsumePickaxe();
        }

        private void ConsumePickaxe()
        {
            _pickaxeActive = false;
            _spawnTimer = _spawnDelay;
            UpdateView();
        }

        private void UpdateView()
        {
            if (_renderer == null)
                return;

            _renderer.Render(_ore, _pickaxeCell, _pickaxeActive);
        }
    }
}
