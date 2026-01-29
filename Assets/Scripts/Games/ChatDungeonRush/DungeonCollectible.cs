using System;
using System.Collections;
using UnityEngine;

namespace ChaosLeague.Games.DungeonRush
{
    /// <summary>
    /// Collectible items in the dungeon (coins, chests, hearts).
    /// </summary>
    public class DungeonCollectible : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private CollectibleType _type = CollectibleType.Coin;
        [SerializeField] private int _value = 1;
        
        [Header("Visuals")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        
        // State
        private Vector2Int _gridPosition;
        private bool _isCollected;
        
        public Vector2Int GridPosition => _gridPosition;
        public bool IsCollected => _isCollected;
        public CollectibleType Type => _type;
        public int Value => _value;
        
        /// <summary>
        /// Returns true if this collectible heals the player
        /// </summary>
        public bool IsHealth => _type == CollectibleType.Heart;
        
        #region Initialization
        
        public void Initialize(Vector2Int gridPos, int value)
        {
            _gridPosition = gridPos;
            _value = value;
            _isCollected = false;
        }
        
        #endregion
        
        #region Collection
        
        /// <summary>
        /// Collect this item and return its value
        /// </summary>
        public int Collect()
        {
            if (_isCollected)
                return 0;
            
            _isCollected = true;
            
            TriggerCollectAnimation();
            
            // Destroy after animation
            StartCoroutine(DestroyAfterDelay(0.3f));
            
            return _value;
        }
        
        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
        
        #endregion
        
        #region Animation
        
        private void Update()
        {
            // Simple floating animation for uncollected items
            if (!_isCollected && _spriteRenderer != null)
            {
                float bob = Mathf.Sin(Time.time * 3f) * 0.05f;
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    bob,
                    transform.localPosition.z
                );
            }
        }
        
        private void TriggerCollectAnimation()
        {
            if (_animator != null)
            {
                _animator.SetTrigger("Collect");
            }
            
            // Simple scale up and fade out
            StartCoroutine(CollectEffect());
        }
        
        private IEnumerator CollectEffect()
        {
            if (_spriteRenderer == null) yield break;
            
            Vector3 startScale = transform.localScale;
            Color startColor = _spriteRenderer.color;
            
            float duration = 0.25f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // Scale up
                transform.localScale = startScale * (1f + t * 0.5f);
                
                // Fade out
                _spriteRenderer.color = new Color(
                    startColor.r,
                    startColor.g,
                    startColor.b,
                    1f - t
                );
                
                // Float up
                transform.position += Vector3.up * Time.deltaTime * 2f;
                
                yield return null;
            }
        }
        
        #endregion
    }
    
    public enum CollectibleType
    {
        Coin,
        Gem,
        Chest,
        Heart,
        Key
    }
}
