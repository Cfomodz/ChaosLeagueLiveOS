using System;
using System.Collections;
using UnityEngine;

namespace ChaosLeague.Games.DungeonRush
{
    /// <summary>
    /// The hero character controlled by chat votes.
    /// Handles movement, combat, and health.
    /// </summary>
    public class DungeonHero : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int _maxHealth = 5;
        [SerializeField] private int _attackDamage = 1;
        [SerializeField] private int _attackRange = 1;
        
        [Header("Animation")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        
        [Header("References")]
        [SerializeField] private DungeonGrid _grid;
        
        // State
        private int _currentHealth;
        private Vector2Int _gridPosition;
        private bool _isMoving;
        private FacingDirection _facing = FacingDirection.Down;
        
        // Events
        public event Action OnDeath;
        public event Action OnEnemyKilled;
        public event Action<int> OnTreasureCollected;
        public event Action<int, int> OnHealthChanged;
        
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public Vector2Int GridPosition => _gridPosition;
        public bool IsDead => _currentHealth <= 0;
        public bool IsMoving => _isMoving;
        
        #region Initialization
        
        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
            
            if (_grid != null)
            {
                _gridPosition = _grid.HeroSpawnPoint;
                transform.position = _grid.GridToWorld(_gridPosition);
            }
            
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
        
        public void ResetPosition()
        {
            if (_grid != null)
            {
                _gridPosition = _grid.HeroSpawnPoint;
                transform.position = _grid.GridToWorld(_gridPosition);
            }
            _facing = FacingDirection.Down;
            UpdateSprite();
        }
        
        #endregion
        
        #region Movement
        
        /// <summary>
        /// Attempt to move the hero in a direction
        /// </summary>
        public bool TryMove(Vector2Int direction)
        {
            if (_isMoving || IsDead)
                return false;
            
            // Update facing direction
            UpdateFacing(direction);
            
            Vector2Int targetPos = _gridPosition + direction;
            
            // Check if walkable
            if (_grid == null || !_grid.IsWalkable(targetPos))
            {
                // Play bump animation/sound
                TriggerBumpAnimation();
                return false;
            }
            
            // Check for enemy at target
            var enemy = _grid.GetEnemyAt(targetPos);
            if (enemy != null)
            {
                // Attack instead of moving
                AttackEnemy(enemy);
                return true;
            }
            
            // Move to target
            StartCoroutine(MoveToPosition(targetPos));
            return true;
        }
        
        private IEnumerator MoveToPosition(Vector2Int targetPos)
        {
            _isMoving = true;
            _gridPosition = targetPos;
            
            Vector3 startPos = transform.position;
            Vector3 endPos = _grid.GridToWorld(targetPos);
            
            // Animate movement
            float elapsed = 0f;
            float duration = 1f / _moveSpeed;
            
            TriggerWalkAnimation();
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            
            transform.position = endPos;
            _isMoving = false;
            
            TriggerIdleAnimation();
            
            // Check for collectibles
            CheckCollectibles();
            
            // Check for traps
            CheckTraps();
            
            // Check for exit
            CheckExit();
        }
        
        private void UpdateFacing(Vector2Int direction)
        {
            if (direction.y > 0) _facing = FacingDirection.Up;
            else if (direction.y < 0) _facing = FacingDirection.Down;
            else if (direction.x < 0) _facing = FacingDirection.Left;
            else if (direction.x > 0) _facing = FacingDirection.Right;
            
            UpdateSprite();
        }
        
        private void UpdateSprite()
        {
            // Flip sprite for left/right
            if (_spriteRenderer != null)
            {
                _spriteRenderer.flipX = (_facing == FacingDirection.Left);
            }
            
            // Update animator parameters if available
            if (_animator != null)
            {
                _animator.SetInteger("FacingDirection", (int)_facing);
            }
        }
        
        #endregion
        
        #region Combat
        
        /// <summary>
        /// Perform a power attack in the facing direction
        /// </summary>
        public void PerformAttack()
        {
            if (_isMoving || IsDead)
                return;
            
            TriggerAttackAnimation();
            
            // Check for enemy in attack range
            Vector2Int attackDir = GetFacingVector();
            
            for (int i = 1; i <= _attackRange; i++)
            {
                Vector2Int checkPos = _gridPosition + (attackDir * i);
                
                if (_grid != null)
                {
                    var enemy = _grid.GetEnemyAt(checkPos);
                    if (enemy != null)
                    {
                        AttackEnemy(enemy);
                        break;
                    }
                }
            }
        }
        
        private void AttackEnemy(DungeonEnemy enemy)
        {
            TriggerAttackAnimation();
            
            enemy.TakeDamage(_attackDamage);
            
            if (enemy.IsDead)
            {
                OnEnemyKilled?.Invoke();
            }
        }
        
        private Vector2Int GetFacingVector()
        {
            switch (_facing)
            {
                case FacingDirection.Up: return Vector2Int.up;
                case FacingDirection.Down: return Vector2Int.down;
                case FacingDirection.Left: return Vector2Int.left;
                case FacingDirection.Right: return Vector2Int.right;
                default: return Vector2Int.down;
            }
        }
        
        public void TakeDamage(int damage)
        {
            if (IsDead) return;
            
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            TriggerHurtAnimation();
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Heal(int amount)
        {
            if (IsDead) return;
            
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            TriggerHealEffect();
        }
        
        private void Die()
        {
            TriggerDeathAnimation();
            OnDeath?.Invoke();
        }
        
        #endregion
        
        #region Interactions
        
        private void CheckCollectibles()
        {
            if (_grid == null) return;
            
            var collectible = _grid.GetCollectibleAt(_gridPosition);
            if (collectible != null)
            {
                int value = collectible.Collect();
                OnTreasureCollected?.Invoke(value);
                
                // Check if it's a health item
                if (collectible.IsHealth)
                {
                    Heal(value);
                }
            }
        }
        
        private void CheckTraps()
        {
            // Implemented via tile collision in DungeonGrid
            // For now, assume traps deal 1 damage
        }
        
        private void CheckExit()
        {
            if (_grid != null && _gridPosition == _grid.ExitPoint)
            {
                // Exit reached - handled by game controller
            }
        }
        
        #endregion
        
        #region Animation Triggers
        
        private void TriggerIdleAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Idle");
        }
        
        private void TriggerWalkAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Walk");
        }
        
        private void TriggerAttackAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Attack");
        }
        
        private void TriggerHurtAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Hurt");
            
            StartCoroutine(FlashRed());
        }
        
        private void TriggerDeathAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Death");
        }
        
        private void TriggerBumpAnimation()
        {
            // Small shake to indicate can't move
            StartCoroutine(BumpShake());
        }
        
        private void TriggerHealEffect()
        {
            StartCoroutine(FlashGreen());
        }
        
        private IEnumerator FlashRed()
        {
            if (_spriteRenderer == null) yield break;
            
            Color original = _spriteRenderer.color;
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = original;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = original;
        }
        
        private IEnumerator FlashGreen()
        {
            if (_spriteRenderer == null) yield break;
            
            Color original = _spriteRenderer.color;
            _spriteRenderer.color = Color.green;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.color = original;
        }
        
        private IEnumerator BumpShake()
        {
            Vector3 originalPos = transform.position;
            Vector2Int dir = GetFacingVector();
            Vector3 bumpDir = new Vector3(dir.x, dir.y, 0) * 0.1f;
            
            transform.position = originalPos + bumpDir;
            yield return new WaitForSeconds(0.05f);
            transform.position = originalPos;
        }
        
        #endregion
    }
    
    public enum FacingDirection
    {
        Down = 0,
        Up = 1,
        Left = 2,
        Right = 3
    }
}
