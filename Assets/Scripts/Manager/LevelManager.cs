namespace WordPuzzle.Manager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using WordPuzzle.Command;
    using WordPuzzle.Data;
    using WordPuzzle.Game;

    public class LevelManager : BaseObject
    {
        [SerializeField] private string _defaultLevelDataJSON;
        [SerializeField] private TextAsset _dictionaryText;

        private LevelData _levelData;

        private List<Tile> _tiles;

        private List<Tile> _formingTiles;

        public event Action<Tile> TileSelected;
        public event Action<List<Tile>> TileSet;
        public event Action<Tile> TileMoveBack;

        private Stack<ICommandMovement> _movements;

        protected override void Awake()
        {
            base.Awake();
            _levelData = JsonUtility.FromJson<LevelData>(_defaultLevelDataJSON);
            _tiles = new List<Tile>();
            _movements = new Stack<ICommandMovement>();
            _formingTiles = new List<Tile>();
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            Debug.Log(HaveDictionary("cable"));

            foreach (var tileData in _levelData.tiles)
            {
                Tile newTile = resourcesManager.GetObject<Tile>();
                newTile.ID = tileData.id;
                newTile.DefaultPosition = tileData.position;
                newTile.Character = tileData.character;
                newTile.SortingOrder = 5;
                newTile.CanUse = true;
                newTile.PointerDown += OnTilePointerDown;
                newTile.Moved += OnTileMove;
                newTile.MoveBacked += OnTileMoveBacked;
                _tiles.Add(newTile);
                _formingTiles.Add(newTile);
            }

            foreach (var tileData in _levelData.tiles)
            {
                foreach (int childId in tileData.children)
                {
                    Tile child = _tiles.Find(t => t.ID == childId);
                    child.SortingOrder -= 1;
                    child.CanUse = false;
                    _formingTiles.Remove(child);
                }
            }

            float x = _levelData.tiles.Sum(t => t.position.x) / _levelData.tiles.Count();
            float y = _levelData.tiles.Sum(t => t.position.y) / _levelData.tiles.Count();

            Camera.main.transform.position = new Vector3(x, y, -10);

            InitWorkArea();

            TileSet?.Invoke(_formingTiles);

        }

        private void OnTileMoveBacked(Tile tile)
        {
            TileMoveBack?.Invoke(tile);
        }

        private void OnTileMove(Tile tile)
        {
            _movements.Push(tile);
        }

        private void InitWorkArea()
        {
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            WordArea wordArea = resourcesManager.GetObject<WordArea>(transform);

            wordArea.SetLevelManager(this);

            Vector2 pos = wordArea.transform.position;
            pos.x = Camera.main.transform.position.x;

            wordArea.transform.position = pos;
        }

        private void OnTilePointerDown(Tile tile)
        {
            TileSelected?.Invoke(tile);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Undo();
        }

        private void Undo()
        {
            if (_movements.Any())
                _movements.Pop().MoveBack();
        }

        private bool HaveDictionary(string answer)
        {
            string[] words = _dictionaryText.text.Split("\n");
            List<string> results = new List<string>();
            List<char> letters = answer.ToCharArray().ToList();
            foreach (string word in words)
            {
                if (word.Length != 6)
                    continue;
                bool allMatch = true;
                letters.ForEach(l =>
                {
                    if (!word.Contains(l))
                        allMatch = false;
                });
                if (allMatch)
                    return true;
            }

            return false;
        }
    }
}