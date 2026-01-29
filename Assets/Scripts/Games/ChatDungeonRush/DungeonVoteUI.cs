using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ChaosLeague.Games.DungeonRush
{
    /// <summary>
    /// UI display for the voting system.
    /// Shows current votes, timer, and command indicators.
    /// </summary>
    public class DungeonVoteUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChatDungeonRushGame _game;
        
        [Header("Timer UI")]
        [SerializeField] private Image _timerFill;
        [SerializeField] private TextMeshProUGUI _timerText;
        
        [Header("Vote Bars")]
        [SerializeField] private VoteBarUI _upVoteBar;
        [SerializeField] private VoteBarUI _downVoteBar;
        [SerializeField] private VoteBarUI _leftVoteBar;
        [SerializeField] private VoteBarUI _rightVoteBar;
        [SerializeField] private VoteBarUI _attackVoteBar;
        [SerializeField] private VoteBarUI _waitVoteBar;
        
        [Header("Health UI")]
        [SerializeField] private Transform _heartsContainer;
        [SerializeField] private GameObject _heartFullPrefab;
        [SerializeField] private GameObject _heartEmptyPrefab;
        
        [Header("Score UI")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _roomText;
        
        [Header("Banner UI")]
        [SerializeField] private GameObject _victoryBanner;
        [SerializeField] private GameObject _defeatBanner;
        
        private List<GameObject> _heartIcons = new List<GameObject>();
        
        #region Unity Lifecycle
        
        private void Start()
        {
            if (_victoryBanner != null) _victoryBanner.SetActive(false);
            if (_defeatBanner != null) _defeatBanner.SetActive(false);
            
            // Subscribe to game events
            if (_game != null)
            {
                _game.OnVoteExecuted.AddListener(OnVoteExecuted);
                _game.OnRoomCleared.AddListener(OnRoomCleared);
                _game.OnGameWon.AddListener(OnGameWon);
                _game.OnGameLost.AddListener(OnGameLost);
            }
        }
        
        private void Update()
        {
            if (_game == null || !_game.IsGameStarted)
                return;
            
            UpdateTimer();
            UpdateVoteBars();
            UpdateScore();
        }
        
        private void OnDestroy()
        {
            if (_game != null)
            {
                _game.OnVoteExecuted.RemoveListener(OnVoteExecuted);
                _game.OnRoomCleared.RemoveListener(OnRoomCleared);
                _game.OnGameWon.RemoveListener(OnGameWon);
                _game.OnGameLost.RemoveListener(OnGameLost);
            }
        }
        
        #endregion
        
        #region UI Updates
        
        private void UpdateTimer()
        {
            float progress = _game.TurnProgress;
            
            if (_timerFill != null)
            {
                _timerFill.fillAmount = 1f - progress;
            }
            
            if (_timerText != null)
            {
                float remaining = (1f - progress) * 4f; // Assuming 4 second turns
                _timerText.text = remaining.ToString("F1");
            }
        }
        
        private void UpdateVoteBars()
        {
            int total = _game.GetTotalVotes();
            
            UpdateSingleVoteBar(_upVoteBar, VoteCommand.Up, total);
            UpdateSingleVoteBar(_downVoteBar, VoteCommand.Down, total);
            UpdateSingleVoteBar(_leftVoteBar, VoteCommand.Left, total);
            UpdateSingleVoteBar(_rightVoteBar, VoteCommand.Right, total);
            UpdateSingleVoteBar(_attackVoteBar, VoteCommand.Attack, total);
            UpdateSingleVoteBar(_waitVoteBar, VoteCommand.Wait, total);
        }
        
        private void UpdateSingleVoteBar(VoteBarUI bar, VoteCommand command, int totalVotes)
        {
            if (bar == null) return;
            
            int count = _game.GetVoteCount(command);
            float percentage = totalVotes > 0 ? (float)count / totalVotes : 0f;
            
            bar.SetValue(count, percentage);
        }
        
        private void UpdateScore()
        {
            if (_scoreText != null)
            {
                _scoreText.text = _game.TotalScore.ToString("N0");
            }
            
            if (_roomText != null)
            {
                _roomText.text = $"Room {_game.CurrentRoom + 1}/{_game.TotalRooms}";
            }
        }
        
        public void UpdateHealth(int current, int max)
        {
            // Clear existing hearts
            foreach (var heart in _heartIcons)
            {
                if (heart != null)
                    Destroy(heart);
            }
            _heartIcons.Clear();
            
            if (_heartsContainer == null)
                return;
            
            // Create new hearts
            for (int i = 0; i < max; i++)
            {
                GameObject prefab = i < current ? _heartFullPrefab : _heartEmptyPrefab;
                if (prefab != null)
                {
                    var heart = Instantiate(prefab, _heartsContainer);
                    _heartIcons.Add(heart);
                }
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnVoteExecuted(VoteCommand command)
        {
            // Flash the winning vote bar
            VoteBarUI bar = GetBarForCommand(command);
            if (bar != null)
            {
                bar.FlashWinner();
            }
        }
        
        private void OnRoomCleared(int roomIndex)
        {
            // Could show "Room Cleared!" animation
        }
        
        private void OnGameWon()
        {
            if (_victoryBanner != null)
                _victoryBanner.SetActive(true);
        }
        
        private void OnGameLost()
        {
            if (_defeatBanner != null)
                _defeatBanner.SetActive(true);
        }
        
        private VoteBarUI GetBarForCommand(VoteCommand command)
        {
            switch (command)
            {
                case VoteCommand.Up: return _upVoteBar;
                case VoteCommand.Down: return _downVoteBar;
                case VoteCommand.Left: return _leftVoteBar;
                case VoteCommand.Right: return _rightVoteBar;
                case VoteCommand.Attack: return _attackVoteBar;
                case VoteCommand.Wait: return _waitVoteBar;
                default: return null;
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// Individual vote bar component
    /// </summary>
    [System.Serializable]
    public class VoteBarUI : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private TextMeshProUGUI _percentText;
        [SerializeField] private Image _highlightImage;
        
        private Color _originalColor;
        private float _flashTimer;
        
        private void Awake()
        {
            if (_fillImage != null)
                _originalColor = _fillImage.color;
        }
        
        private void Update()
        {
            if (_flashTimer > 0)
            {
                _flashTimer -= Time.deltaTime;
                
                if (_highlightImage != null)
                {
                    float alpha = _flashTimer / 0.5f;
                    _highlightImage.color = new Color(1f, 1f, 1f, alpha);
                }
            }
        }
        
        public void SetValue(int count, float percentage)
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = percentage;
            }
            
            if (_countText != null)
            {
                _countText.text = count.ToString();
            }
            
            if (_percentText != null)
            {
                _percentText.text = $"{(percentage * 100):F0}%";
            }
        }
        
        public void FlashWinner()
        {
            _flashTimer = 0.5f;
            
            if (_highlightImage != null)
            {
                _highlightImage.color = Color.white;
            }
        }
    }
}
