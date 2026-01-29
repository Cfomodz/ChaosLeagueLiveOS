using System;
using System.Collections;
using UnityEngine;

namespace ChaosLeague.Games.DungeonRush
{
    /// <summary>
    /// Base class for dungeon enemies.
    /// Enemies are stationary and attack the hero when adjacent.
    /// </summary>
    public class DungeonEnemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int _maxHealth = 2;
        [SerializeField] private int _attackDamage = 1;
        [SerializeField] private EnemyType _enemyType = EnemyType.Slime;
        
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        
        // State
        private int _currentHealth;
        private Vector2Int _gridPosition;
        private DungeonGrid _grid;
        
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public Vector2Int GridPosition => _gridPosition;
        public bool IsDead => _currentHealth <= 0;
        public EnemyType Type => _enemyType;
        
        public event Action<DungeonEnemy> OnDeath;
        
        #region Initialization
        
        public void Initialize(Vector2Int gridPos, DungeonGrid grid)
        {
            _gridPosition = gridPos;
            _grid = grid;
            _currentHealth = _maxHealth;
            
            // Adjust stats based on enemy type
            switch (_enemyType)
            {
                case EnemyType.Slime:
                    _maxHealth = 1;
                    _attackDamage = 1;
                    break;
                case EnemyType.Skeleton:
                    _maxHealth = 2;
                    _attackDamage = 1;
                    break;
                case EnemyType.Bat:
                    _maxHealth = 1;
                    _attackDamage = 1;
                    break;
                case EnemyType.Boss:
                    _maxHealth = 5;
                    _attackDamage = 2;
                    break;
            }
            
            _currentHealth = _maxHealth;
        }
        
        #endregion
        
        #region Combat
        
        public void TakeDamage(int damage)
        {
            if (IsDead) return;
            
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            
            TriggerHurtAnimation();
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Attack the hero if adjacent
        /// </summary>
        public void TryAttackHero(DungeonHero hero)
        {
            if (IsDead || hero == null || hero.IsDead)
                return;
            
            // Check if hero is adjacent
            Vector2Int diff = hero.GridPosition - _gridPosition;
            if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) == 1)
            {
                TriggerAttackAnimation();
                hero.TakeDamage(_attackDamage);
            }
        }
        
        private void Die()
        {
            TriggerDeathAnimation();
            
            // Notify grid
            if (_grid != null)
            {
                _grid.RemoveEnemy(this);
            }
            
            OnDeath?.Invoke(this);
            
            // Destroy after death animation
            StartCoroutine(DestroyAfterDelay(0.5f));
        }
        
        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
        
        #endregion
        
        #region Animation
        
        private void TriggerHurtAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Hurt");
            
            StartCoroutine(FlashWhite());
        }
        
        private void TriggerAttackAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Attack");
        }
        
        private void TriggerDeathAnimation()
        {
            if (_animator != null)
                _animator.SetTrigger("Death");
        }
        
        private IEnumerator FlashWhite()
        {
            if (_spriteRenderer == null) yield break;
            
            Color original = _spriteRenderer.color;
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = original;
        }
        
        #endregion
    }
    
    public enum EnemyType
    {
        Slime,
        Skeleton,
        Bat,
        Boss
    }
}
