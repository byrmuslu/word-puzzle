namespace WordPuzzle.Manager
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;
    using WordPuzzle.Command;
    using WordPuzzle.Data;
    using WordPuzzle.Game;
    using WordPuzzle.Other;

    public class LevelManager : BaseObject
    {
        [SerializeField] private string _defaultLevelDataJSON;
        [SerializeField] private TextAsset _dictionaryText;

        private List<Tile> _tiles;

        public delegate void OnTitleChanged(string title);
        public event OnTitleChanged TitleChanged;
        public event Action<Tile> TilePointerDown;
        public delegate void OnPhaseChange(int maxWordLength, int totalScore);
        public event OnPhaseChange PhaseChanged;
        public delegate void OnPhaseDone(string word);
        public event OnPhaseDone PhaseDone;
        public delegate void OnWordCompletionStatusChange(bool hasWord, int wordScore);
        public event OnWordCompletionStatusChange WordCompletionStatusChanged;
        public delegate void OnActionAvailabilityChange(bool status);
        public event OnActionAvailabilityChange ActionAvailabilityChanged;
        public delegate void OnLevelComplete(LevelContent level, int levelScore);
        public event OnLevelComplete LevelCompleted;

        private Stack<ICommand> _actions;
        private List<Tile> _selectedTiles;

        private WordAlgorithm _wordAlgorithm;

        private LevelContent _currentLevelContent;
        private int _totalScore;
        private int _wordScore;

        private readonly List<string> _foundWords = new List<string>();
        private readonly List<Tile> _unUsedTiles = new List<Tile>();

        protected override void Awake()
        {
            base.Awake();
            _tiles = new List<Tile>();
            _actions = new Stack<ICommand>();
            _selectedTiles = new List<Tile>();
            _wordAlgorithm = new WordAlgorithm(_dictionaryText.text.Replace("\r", "").Split('\n'));
        }

        public void SetLevel(LevelContent level)
        {
            ClearLevel();
            _currentLevelContent = level;

            LevelData levelData = level.GetData();

            ResourcesManager resourcesManager = ResourcesManager.Instance;

            foreach (var tileData in levelData.tiles)
            {
                Tile newTile = resourcesManager.GetObject<Tile>();
                newTile.SetID(tileData.id);
                newTile.SetDefaultPosition(tileData.position);
                newTile.SetCharacter(tileData.character);
                newTile.SetSortingOrder(5);
                newTile.SetCanUse(true);

                newTile.Selected -= OnTilePointerDown;
                newTile.Executed -= OnTileExecuted;
                newTile.Withdrawn -= OnTileWithdrawn;
                newTile.ObjectDestroyed -= OnTileDestroyed;

                newTile.Selected += OnTilePointerDown;
                newTile.Executed += OnTileExecuted;
                newTile.Withdrawn += OnTileWithdrawn;
                newTile.ObjectDestroyed += OnTileDestroyed;

                _tiles.Add(newTile);
            }

            foreach (var tileData in levelData.tiles)
            {
                foreach (int childId in tileData.children)
                {
                    Tile child = _tiles.Find(t => t.ID == childId);
                    child.SetSortingOrder(child.SortingOrder - 1);
                    child.SetCanUse(false);
                }
            }

            float x = levelData.tiles.Sum(t => t.position.x) / levelData.tiles.Count();
            float y = levelData.tiles.Sum(t => t.position.y) / levelData.tiles.Count();

            Camera.main.transform.position = new Vector3(x, y + 17, -10);
            SetAvailableTiles();
            TitleChanged?.Invoke(levelData.title);
        }

        private void OnTileDestroyed(BaseObject destroyedObject)
        {
            _tiles.Remove(destroyedObject as Tile);
            _selectedTiles.Remove(destroyedObject as Tile);
            destroyedObject.ObjectDestroyed -= OnTileDestroyed;
            (destroyedObject as Tile).Executed -= OnTileExecuted;
            (destroyedObject as Tile).Withdrawn -= OnTileWithdrawn;
            (destroyedObject as Tile).Selected -= OnTilePointerDown;
        }

        private void OnTilePointerDown(Tile tile)
        {
            TilePointerDown?.Invoke(tile);
        }

        private void OnTileExecuted(Tile tile)
        {
            _selectedTiles.Add(tile);
            _actions.Push(tile);
            ActionAvailabilityChanged?.Invoke(true);
            CheckWordCompletionStatus();
        }

        private void OnTileWithdrawn(Tile tile)
        {
            _selectedTiles.Remove(tile);
            CheckWordCompletionStatus();
        }

        private void CheckWordCompletionStatus()
        {
            string word = GetTilesOfWord(_selectedTiles);
            bool wordCompletionStatus = _wordAlgorithm.HasPossibleWord(word);
            int wordScore = 0;
            if (wordCompletionStatus)
                wordScore = (_wordAlgorithm.GetWordScore(word) * 10 * word.Length);
            _wordScore = wordScore;
            WordCompletionStatusChanged?.Invoke(wordCompletionStatus, wordScore);
        }

        private string GetTilesOfWord(List<Tile> tiles)
        {
            StringBuilder stringBuilder = new StringBuilder();

            tiles.ForEach(t =>
            {
                stringBuilder.Append(t.Character);
            });

            return stringBuilder.ToString().ToLower().Replace('ý', 'i');
        }

        public void Done()
        {
            string word = GetTilesOfWord(_selectedTiles);
            if (_foundWords.Contains(word))
            {
                if(_selectedTiles.Count == _tiles.Count)
                {
                    _unUsedTiles.AddRange(_selectedTiles);
                    _tiles.Clear();
                    _selectedTiles.Clear();
                    _actions.Clear();
                    _wordScore = 0;
                    SetAvailableTiles();
                }
                return;
            }
            if(!string.IsNullOrEmpty(word))
                _foundWords.Add(word);
            
            List<Tile> tmpTiles = new List<Tile>();
            
            if (_selectedTiles.Count > 0)
                tmpTiles.AddRange(_selectedTiles);
            else
                tmpTiles.AddRange(_tiles.FindAll(t => t.CanUse));
            
            tmpTiles.ForEach(t =>
            {
                _tiles.Remove(t);
                t.Destroy();
            
            });

            _unUsedTiles.AddRange(_tiles.FindAll(t => t.CanUse));
            _selectedTiles.Clear();
            _actions.Clear();
            _totalScore += _wordScore;
            _wordScore = 0;

            PhaseDone?.Invoke(word);
            SetAvailableTiles();
        }

        public void Undo()
        {
            if (_actions.Any())
                _actions.Pop().Undo();
            if (_actions.Count == 0)
                ActionAvailabilityChanged?.Invoke(false);
        }

        public void AllUndo()
        {
            while (_actions.Any())
                _actions.Pop().Undo();
            ActionAvailabilityChanged?.Invoke(false);
        }

        private void ClearLevel()
        {
            _unUsedTiles.Clear();
            _foundWords.Clear();
            _totalScore = 0;
            _selectedTiles.Clear();
            _tiles.ForEach(t => t.Destroy());
            _tiles.Clear();
        }

        public void SetAvailableTiles()
        {
            if (_tiles.Count == 0)
            {
                int totalGameScore = _totalScore - (100 * _unUsedTiles.Count);
                PhaseChanged?.Invoke(0, totalGameScore);
                LevelCompleted?.Invoke(_currentLevelContent, totalGameScore);
                return;
            }

            int maxSortingOrder = _tiles.Max(t => t.SortingOrder);

            _tiles.ForEach(t =>
            {
                t.SetCanUse(t.SortingOrder == maxSortingOrder);
            });
            string word = GetTilesOfWord(_tiles.FindAll(t => t.CanUse));
            
            int possibleMaxLengthOfWord = _wordAlgorithm.GetPossibleMaxLengthOfWord(word.ToLower().Replace('ý', 'i'));
            
            if (possibleMaxLengthOfWord == 0)
                Done();
            else
            {
                int totalGameScore = _totalScore - (100 * _unUsedTiles.Count);
                PhaseChanged?.Invoke(possibleMaxLengthOfWord, totalGameScore);
            }
        }

        public void AutoSolve(Action onComplete)
        {
            StopAllCoroutines();
            StartCoroutine(AutoSolveAction(onComplete));
        }

        private IEnumerator AutoSolveAction(Action onComplete)
        {
            while (_tiles.Any())
            {
                List<Tile> availableTiles = _tiles.FindAll(t => t.CanUse);
                string word = GetTilesOfWord(availableTiles);
                string possibleMaxScoreText = _wordAlgorithm.GetPossibleMaxScoreOfWord(word.ToString().ToLower().Replace('ý', 'i'));
                Debug.Log("Highest Word : " + possibleMaxScoreText);
                for (int i = 0; i < possibleMaxScoreText.Length; i++)
                {
                    Tile tile = availableTiles.Find(t => t.Character.Equals(possibleMaxScoreText[i].ToString().Replace("i", "ý").ToUpper()));
                    availableTiles.Remove(tile);
                    tile.Select();
                }
                yield return new WaitForSeconds(1f);
                Done();
                yield return new WaitForSeconds(1f);
            }
            onComplete();
        }
    }
}