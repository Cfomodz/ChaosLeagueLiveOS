using System.Collections.Generic;
using UnityEngine;

namespace ChaosLeague.Games.DungeonRush
{
    /// <summary>
    /// Manages the grid-based dungeon for Chat Dungeon Rush.
    /// Handles tile spawning, room generation, and collision detection.
    /// </summary>
    public class DungeonGrid : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;
        [SerializeField] private float _tileSize = 1f;
        
        [Header("Tile Prefabs")]
        [SerializeField] private GameObject _floorTilePrefab;
        [SerializeField] private GameObject _wallTilePrefab;
        [SerializeField] private GameObject _doorPrefab;
        
        [Header("Entity Prefabs")]
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private GameObject _chestPrefab;
        [SerializeField] private GameObject _slimePrefab;
        [SerializeField] private GameObject _skeletonPrefab;
        [SerializeField] private GameObject _batPrefab;
        [SerializeField] private GameObject _bossPrefab;
        
        // Grid data
        private TileType[,] _grid;
        private List<DungeonEnemy> _activeEnemies = new List<DungeonEnemy>();
        private List<DungeonCollectible> _activeCollectibles = new List<DungeonCollectible>();
        private List<GameObject> _spawnedObjects = new List<GameObject>();
        
        public int Width => _width;
        public int Height => _height;
        public float TileSize => _tileSize;
        public Vector2Int HeroSpawnPoint { get; private set; }
        public Vector2Int ExitPoint { get; private set; }
        
        #region Room Generation
        
        /// <summary>
        /// Generate a new room based on progression
        /// </summary>
        public void GenerateRoom(int roomIndex, int totalRooms)
        {
            ClearRoom();
            
            _grid = new TileType[_width, _height];
            
            // Determine room type based on progression
            RoomType roomType = DetermineRoomType(roomIndex, totalRooms);
            
            // Generate base layout
            GenerateBaseLayout();
            
            // Add room-specific content
            switch (roomType)
            {
                case RoomType.Combat:
                    GenerateCombatRoom(roomIndex);
                    break;
                case RoomType.Treasure:
                    GenerateTreasureRoom();
                    break;
                case RoomType.Trap:
                    GenerateTrapRoom();
                    break;
                case RoomType.Boss:
                    GenerateBossRoom();
                    break;
            }
            
            // Spawn tiles and entities
            SpawnTiles();
            SpawnEntities();
        }
        
        private RoomType DetermineRoomType(int roomIndex, int totalRooms)
        {
            // Last room is always boss
            if (roomIndex == totalRooms - 1)
                return RoomType.Boss;
            
            // First room is easier combat
            if (roomIndex == 0)
                return RoomType.Combat;
            
            // Random distribution for middle rooms
            float roll = Random.value;
            if (roll < 0.5f)
                return RoomType.Combat;
            else if (roll < 0.75f)
                return RoomType.Treasure;
            else
                return RoomType.Trap;
        }
        
        private void GenerateBaseLayout()
        {
            // Fill with floor
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    // Walls on edges
                    if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1)
                    {
                        _grid[x, y] = TileType.Wall;
                    }
                    else
                    {
                        _grid[x, y] = TileType.Floor;
                    }
                }
            }
            
            // Hero spawn at bottom center
            HeroSpawnPoint = new Vector2Int(_width / 2, 1);
            _grid[HeroSpawnPoint.x, HeroSpawnPoint.y] = TileType.Floor;
            
            // Exit at top center
            ExitPoint = new Vector2Int(_width / 2, _height - 2);
            _grid[ExitPoint.x, ExitPoint.y] = TileType.Exit;
        }
        
        private void GenerateCombatRoom(int difficulty)
        {
            int enemyCount = Mathf.Min(2 + difficulty, 5);
            
            for (int i = 0; i < enemyCount; i++)
            {
                Vector2Int pos = GetRandomFloorPosition();
                _grid[pos.x, pos.y] = TileType.Enemy;
            }
            
            // Add a few coins
            int coinCount = Random.Range(2, 5);
            for (int i = 0; i < coinCount; i++)
            {
                Vector2Int pos = GetRandomFloorPosition();
                _grid[pos.x, pos.y] = TileType.Coin;
            }
        }
        
        private void GenerateTreasureRoom()
        {
            // Multiple coins
            int coinCount = Random.Range(6, 10);
            for (int i = 0; i < coinCount; i++)
            {
                Vector2Int pos = GetRandomFloorPosition();
                _grid[pos.x, pos.y] = TileType.Coin;
            }
            
            // A chest
            Vector2Int chestPos = GetRandomFloorPosition();
            _grid[chestPos.x, chestPos.y] = TileType.Chest;
            
            // Maybe one enemy guarding
            if (Random.value > 0.5f)
            {
                Vector2Int enemyPos = GetRandomFloorPosition();
                _grid[enemyPos.x, enemyPos.y] = TileType.Enemy;
            }
        }
        
        private void GenerateTrapRoom()
        {
            // Spike traps
            int trapCount = Random.Range(4, 8);
            for (int i = 0; i < trapCount; i++)
            {
                Vector2Int pos = GetRandomFloorPosition();
                _grid[pos.x, pos.y] = TileType.Trap;
            }
            
            // Some treasure as reward
            int coinCount = Random.Range(3, 6);
            for (int i = 0; i < coinCount; i++)
            {
                Vector2Int pos = GetRandomFloorPosition();
                _grid[pos.x, pos.y] = TileType.Coin;
            }
        }
        
        private void GenerateBossRoom()
        {
            // Boss in center-ish area
            Vector2Int bossPos = new Vector2Int(_width / 2, _height / 2);
            _grid[bossPos.x, bossPos.y] = TileType.Boss;
            
            // Clear area around boss
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int x = bossPos.x + dx;
                    int y = bossPos.y + dy;
                    if (x > 0 && x < _width - 1 && y > 0 && y < _height - 1)
                    {
                        if (_grid[x, y] != TileType.Boss)
                            _grid[x, y] = TileType.Floor;
                    }
                }
            }
        }
        
        private Vector2Int GetRandomFloorPosition()
        {
            int attempts = 100;
            while (attempts-- > 0)
            {
                int x = Random.Range(2, _width - 2);
                int y = Random.Range(2, _height - 2);
                
                if (_grid[x, y] == TileType.Floor)
                {
                    // Not too close to spawn
                    if (Vector2Int.Distance(new Vector2Int(x, y), HeroSpawnPoint) > 2)
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }
            return new Vector2Int(_width / 2, _height / 2);
        }
        
        #endregion
        
        #region Tile/Entity Management
        
        private void SpawnTiles()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector3 worldPos = GridToWorld(new Vector2Int(x, y));
                    GameObject prefab = null;
                    
                    switch (_grid[x, y])
                    {
                        case TileType.Wall:
                            prefab = _wallTilePrefab;
                            break;
                        case TileType.Exit:
                            prefab = _doorPrefab;
                            break;
                        default:
                            prefab = _floorTilePrefab;
                            break;
                    }
                    
                    if (prefab != null)
                    {
                        var tile = Instantiate(prefab, worldPos, Quaternion.identity, transform);
                        _spawnedObjects.Add(tile);
                    }
                }
            }
        }
        
        private void SpawnEntities()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector3 worldPos = GridToWorld(new Vector2Int(x, y));
                    
                    switch (_grid[x, y])
                    {
                        case TileType.Coin:
                            SpawnCollectible(_coinPrefab, new Vector2Int(x, y), 1);
                            break;
                        case TileType.Chest:
                            SpawnCollectible(_chestPrefab, new Vector2Int(x, y), 5);
                            break;
                        case TileType.Enemy:
                            SpawnEnemy(GetRandomEnemyPrefab(), new Vector2Int(x, y));
                            break;
                        case TileType.Boss:
                            SpawnEnemy(_bossPrefab, new Vector2Int(x, y));
                            break;
                    }
                }
            }
        }
        
        private GameObject GetRandomEnemyPrefab()
        {
            float roll = Random.value;
            if (roll < 0.5f && _slimePrefab != null)
                return _slimePrefab;
            else if (roll < 0.8f && _skeletonPrefab != null)
                return _skeletonPrefab;
            else if (_batPrefab != null)
                return _batPrefab;
            return _slimePrefab;
        }
        
        private void SpawnEnemy(GameObject prefab, Vector2Int gridPos)
        {
            if (prefab == null) return;
            
            Vector3 worldPos = GridToWorld(gridPos);
            var obj = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            _spawnedObjects.Add(obj);
            
            var enemy = obj.GetComponent<DungeonEnemy>();
            if (enemy != null)
            {
                enemy.Initialize(gridPos, this);
                _activeEnemies.Add(enemy);
            }
        }
        
        private void SpawnCollectible(GameObject prefab, Vector2Int gridPos, int value)
        {
            if (prefab == null) return;
            
            Vector3 worldPos = GridToWorld(gridPos);
            var obj = Instantiate(prefab, worldPos, Quaternion.identity, transform);
            _spawnedObjects.Add(obj);
            
            var collectible = obj.GetComponent<DungeonCollectible>();
            if (collectible != null)
            {
                collectible.Initialize(gridPos, value);
                _activeCollectibles.Add(collectible);
            }
        }
        
        private void ClearRoom()
        {
            foreach (var obj in _spawnedObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }
            _spawnedObjects.Clear();
            _activeEnemies.Clear();
            _activeCollectibles.Clear();
        }
        
        #endregion
        
        #region Grid Utilities
        
        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            float offsetX = -(_width * _tileSize) / 2f + _tileSize / 2f;
            float offsetY = -(_height * _tileSize) / 2f + _tileSize / 2f;
            
            return transform.position + new Vector3(
                gridPos.x * _tileSize + offsetX,
                gridPos.y * _tileSize + offsetY,
                0f
            );
        }
        
        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 localPos = worldPos - transform.position;
            float offsetX = -(_width * _tileSize) / 2f + _tileSize / 2f;
            float offsetY = -(_height * _tileSize) / 2f + _tileSize / 2f;
            
            int x = Mathf.RoundToInt((localPos.x - offsetX) / _tileSize);
            int y = Mathf.RoundToInt((localPos.y - offsetY) / _tileSize);
            
            return new Vector2Int(x, y);
        }
        
        public bool IsWalkable(Vector2Int gridPos)
        {
            if (gridPos.x < 0 || gridPos.x >= _width || gridPos.y < 0 || gridPos.y >= _height)
                return false;
            
            TileType tile = _grid[gridPos.x, gridPos.y];
            return tile != TileType.Wall;
        }
        
        public bool IsRoomCleared()
        {
            // Room is cleared when all enemies are dead
            _activeEnemies.RemoveAll(e => e == null || e.IsDead);
            return _activeEnemies.Count == 0;
        }
        
        public DungeonEnemy GetEnemyAt(Vector2Int gridPos)
        {
            foreach (var enemy in _activeEnemies)
            {
                if (enemy != null && !enemy.IsDead && enemy.GridPosition == gridPos)
                    return enemy;
            }
            return null;
        }
        
        public DungeonCollectible GetCollectibleAt(Vector2Int gridPos)
        {
            foreach (var collectible in _activeCollectibles)
            {
                if (collectible != null && !collectible.IsCollected && collectible.GridPosition == gridPos)
                    return collectible;
            }
            return null;
        }
        
        public void RemoveCollectible(DungeonCollectible collectible)
        {
            _activeCollectibles.Remove(collectible);
        }
        
        public void RemoveEnemy(DungeonEnemy enemy)
        {
            _activeEnemies.Remove(enemy);
        }
        
        #endregion
    }
    
    public enum TileType
    {
        Floor,
        Wall,
        Exit,
        Coin,
        Chest,
        Trap,
        Enemy,
        Boss
    }
    
    public enum RoomType
    {
        Combat,
        Treasure,
        Trap,
        Boss
    }
}
