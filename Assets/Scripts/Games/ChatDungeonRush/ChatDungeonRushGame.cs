using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ChaosLeague.Games.DungeonRush
{
    /// <summary>
    /// Chat Dungeon Rush - A simple chat-controlled dungeon crawler mini-game.
    /// Chat votes on hero movement/actions each turn. Most popular vote wins.
    /// </summary>
    public class ChatDungeonRushGame : Game
    {
        [Header("Game Settings")]
        [SerializeField] private float _turnDuration = 4f;
        [SerializeField] private int _gridWidth = 8;
        [SerializeField] private int _gridHeight = 8;
        [SerializeField] private int _maxRooms = 5;
        [SerializeField] private int _heroMaxHealth = 5;
        
        [Header("Scoring")]
        [SerializeField] private int _pointsPerKill = 10;
        [SerializeField] private int _pointsPerTreasure = 25;
        [SerializeField] private int _pointsPerRoomClear = 100;
        
        [Header("References")]
        [SerializeField] private DungeonGrid _dungeonGrid;
        [SerializeField] private DungeonHero _hero;
        [SerializeField] private Transform _gameArea;
        
        [Header("Events")]
        public UnityEvent<VoteCommand> OnVoteExecuted;
        public UnityEvent<int> OnRoomCleared;
        public UnityEvent OnGameWon;
        public UnityEvent OnGameLost;
        
        // Vote tracking
        private Dictionary<VoteCommand, List<VoteEntry>> _currentVotes = new Dictionary<VoteCommand, List<VoteEntry>>();
        private float _turnTimer;
        private bool _isTurnActive;
        private int _currentRoom;
        private int _totalScore;
        
        // Participant tracking for point distribution
        private HashSet<string> _participantsThisTurn = new HashSet<string>();
        private Dictionary<string, int> _participantTotalVotes = new Dictionary<string, int>();
        
        public float TurnProgress => _turnTimer / _turnDuration;
        public int CurrentRoom => _currentRoom;
        public int TotalRooms => _maxRooms;
        public int TotalScore => _totalScore;
        public IReadOnlyDictionary<VoteCommand, List<VoteEntry>> CurrentVotes => _currentVotes;
        
        #region Game Lifecycle
        
        public override void OnTilePreInit()
        {
            // Initialize game state
            _currentRoom = 0;
            _totalScore = 0;
            _currentVotes.Clear();
            _participantTotalVotes.Clear();
            
            InitializeVoteDictionary();
        }
        
        public override void StartGame()
        {
            IsGameStarted = true;
            
            // Generate first dungeon room
            if (_dungeonGrid != null)
            {
                _dungeonGrid.GenerateRoom(_currentRoom, _maxRooms);
            }
            
            // Spawn hero at entrance
            if (_hero != null)
            {
                _hero.Initialize(_heroMaxHealth);
                _hero.OnDeath += HandleHeroDeath;
                _hero.OnEnemyKilled += HandleEnemyKilled;
                _hero.OnTreasureCollected += HandleTreasureCollected;
            }
            
            StartCoroutine(GameLoop());
        }
        
        public override void CleanUpGame()
        {
            IsGameStarted = false;
            StopAllCoroutines();
            
            if (_hero != null)
            {
                _hero.OnDeath -= HandleHeroDeath;
                _hero.OnEnemyKilled -= HandleEnemyKilled;
                _hero.OnTreasureCollected -= HandleTreasureCollected;
            }
            
            DistributeFinalPoints();
        }
        
        #endregion
        
        #region Game Loop
        
        private IEnumerator GameLoop()
        {
            while (IsGameStarted && !DoneWithGameplay)
            {
                // Start new turn
                _isTurnActive = true;
                _turnTimer = 0f;
                ClearVotes();
                _participantsThisTurn.Clear();
                
                // Wait for votes
                while (_turnTimer < _turnDuration && _isTurnActive)
                {
                    _turnTimer += Time.deltaTime;
                    yield return null;
                }
                
                // Execute winning vote
                if (_isTurnActive)
                {
                    ExecuteWinningVote();
                }
                
                // Small delay between turns for visual feedback
                yield return new WaitForSeconds(0.5f);
                
                // Check room completion
                if (_dungeonGrid != null && _dungeonGrid.IsRoomCleared())
                {
                    yield return HandleRoomCleared();
                }
            }
        }
        
        private void ExecuteWinningVote()
        {
            VoteCommand winner = GetWinningCommand();
            
            if (_hero != null)
            {
                switch (winner)
                {
                    case VoteCommand.Up:
                        _hero.TryMove(Vector2Int.up);
                        break;
                    case VoteCommand.Down:
                        _hero.TryMove(Vector2Int.down);
                        break;
                    case VoteCommand.Left:
                        _hero.TryMove(Vector2Int.left);
                        break;
                    case VoteCommand.Right:
                        _hero.TryMove(Vector2Int.right);
                        break;
                    case VoteCommand.Attack:
                        _hero.PerformAttack();
                        break;
                    case VoteCommand.Wait:
                        // Do nothing
                        break;
                }
            }
            
            OnVoteExecuted?.Invoke(winner);
        }
        
        private VoteCommand GetWinningCommand()
        {
            VoteCommand winner = VoteCommand.Wait;
            int maxVoteWeight = 0;
            List<VoteCommand> ties = new List<VoteCommand>();
            
            foreach (var kvp in _currentVotes)
            {
                int totalWeight = kvp.Value.Sum(v => v.Weight);
                
                if (totalWeight > maxVoteWeight)
                {
                    maxVoteWeight = totalWeight;
                    winner = kvp.Key;
                    ties.Clear();
                    ties.Add(kvp.Key);
                }
                else if (totalWeight == maxVoteWeight && totalWeight > 0)
                {
                    ties.Add(kvp.Key);
                }
            }
            
            // Handle tie-breaker
            if (ties.Count > 1)
            {
                winner = ties[UnityEngine.Random.Range(0, ties.Count)];
            }
            
            return winner;
        }
        
        private IEnumerator HandleRoomCleared()
        {
            _isTurnActive = false;
            
            AddScore(_pointsPerRoomClear);
            OnRoomCleared?.Invoke(_currentRoom);
            
            _currentRoom++;
            
            if (_currentRoom >= _maxRooms)
            {
                // Game won!
                DoneWithGameplay = true;
                OnGameWon?.Invoke();
            }
            else
            {
                // Transition to next room
                yield return new WaitForSeconds(1f);
                
                if (_dungeonGrid != null)
                {
                    _dungeonGrid.GenerateRoom(_currentRoom, _maxRooms);
                }
                
                if (_hero != null)
                {
                    _hero.ResetPosition();
                }
            }
        }
        
        #endregion
        
        #region Vote Processing
        
        public override void ProcessGameplayCommand(string messageId, TwitchClient twitchClient, PlayerHandler ph, string msg, string rawEmotesRemoved)
        {
            if (!_isTurnActive || ph == null)
                return;
            
            string command = msg.ToLower().Trim();
            VoteCommand? vote = ParseVoteCommand(command);
            
            if (vote.HasValue)
            {
                RegisterVote(ph, vote.Value);
            }
        }
        
        private VoteCommand? ParseVoteCommand(string command)
        {
            // Remove ! prefix if present
            if (command.StartsWith("!"))
                command = command.Substring(1);
            
            switch (command)
            {
                case "up":
                case "u":
                case "w":
                case "north":
                case "n":
                    return VoteCommand.Up;
                    
                case "down":
                case "d":
                case "s":
                case "south":
                    return VoteCommand.Down;
                    
                case "left":
                case "l":
                case "a":
                case "west":
                    return VoteCommand.Left;
                    
                case "right":
                case "r":
                case "east":
                case "e":
                    return VoteCommand.Right;
                    
                case "attack":
                case "atk":
                case "hit":
                case "x":
                    return VoteCommand.Attack;
                    
                case "wait":
                case "stay":
                case "skip":
                    return VoteCommand.Wait;
                    
                default:
                    return null;
            }
        }
        
        private void RegisterVote(PlayerHandler ph, VoteCommand command)
        {
            string username = ph.pp?.TwitchUsername ?? "unknown";
            
            // Prevent duplicate votes from same user this turn
            if (_participantsThisTurn.Contains(username))
                return;
            
            _participantsThisTurn.Add(username);
            
            // Calculate vote weight based on user status
            int weight = 1;
            // Note: In full implementation, check subscriber/VIP status
            // if (ph.IsSubscriber) weight = 2;
            // if (ph.IsVip) weight = 3;
            
            var entry = new VoteEntry
            {
                Username = username,
                Weight = weight,
                Timestamp = Time.time
            };
            
            _currentVotes[command].Add(entry);
            
            // Track total participation
            if (!_participantTotalVotes.ContainsKey(username))
                _participantTotalVotes[username] = 0;
            _participantTotalVotes[username]++;
        }
        
        private void InitializeVoteDictionary()
        {
            _currentVotes.Clear();
            foreach (VoteCommand cmd in Enum.GetValues(typeof(VoteCommand)))
            {
                _currentVotes[cmd] = new List<VoteEntry>();
            }
        }
        
        private void ClearVotes()
        {
            foreach (var list in _currentVotes.Values)
            {
                list.Clear();
            }
        }
        
        #endregion
        
        #region Scoring & Events
        
        private void AddScore(int points)
        {
            _totalScore += points;
        }
        
        private void HandleHeroDeath()
        {
            DoneWithGameplay = true;
            OnGameLost?.Invoke();
        }
        
        private void HandleEnemyKilled()
        {
            AddScore(_pointsPerKill);
        }
        
        private void HandleTreasureCollected(int value)
        {
            AddScore(_pointsPerTreasure * value);
        }
        
        private void DistributeFinalPoints()
        {
            // Distribute points proportionally to participation
            if (_participantTotalVotes.Count == 0 || _totalScore <= 0)
                return;
            
            int totalVotesCast = _participantTotalVotes.Values.Sum();
            
            // In full implementation, distribute points via PlayerHandler
            // foreach (var kvp in _participantTotalVotes)
            // {
            //     float proportion = (float)kvp.Value / totalVotesCast;
            //     int points = Mathf.RoundToInt(_totalScore * proportion);
            //     // Find PlayerHandler and add points
            // }
        }
        
        #endregion
        
        #region Public Getters for UI
        
        public int GetVoteCount(VoteCommand command)
        {
            if (_currentVotes.TryGetValue(command, out var list))
                return list.Sum(v => v.Weight);
            return 0;
        }
        
        public int GetTotalVotes()
        {
            return _currentVotes.Values.Sum(list => list.Sum(v => v.Weight));
        }
        
        public float GetVotePercentage(VoteCommand command)
        {
            int total = GetTotalVotes();
            if (total == 0) return 0f;
            return (float)GetVoteCount(command) / total;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Available commands that chat can vote on
    /// </summary>
    public enum VoteCommand
    {
        Up,
        Down,
        Left,
        Right,
        Attack,
        Wait
    }
    
    /// <summary>
    /// Represents a single vote entry
    /// </summary>
    public struct VoteEntry
    {
        public string Username;
        public int Weight;
        public float Timestamp;
    }
}
