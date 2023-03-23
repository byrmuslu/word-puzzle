namespace WordPuzzle.Manager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using WordPuzzle.Data;
    using WordPuzzle.Game;

    public class LevelManager : BaseObject
    {
        [SerializeField] private string _defaultLevelDataJSON;
        [SerializeField] private TextAsset _dictionaryText;

        private LevelData _levelData;

        private List<Tile> _tiles;

        private List<Tile> _formingTiles;

        protected override void Awake()
        {
            base.Awake();
            _levelData = JsonUtility.FromJson<LevelData>(_defaultLevelDataJSON);
            _tiles = new List<Tile>();
            _formingTiles = new List<Tile>();
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            foreach(var tileData in _levelData.tiles)
            {
                Tile newTile = resourcesManager.GetObject<Tile>();
                newTile.ID = tileData.id;
                newTile.DefaultPosition = tileData.position;
                newTile.Character = tileData.character;
                newTile.SortingOrder = 5;
                newTile.CanUse = true;
                _tiles.Add(newTile);
                _formingTiles.Add(newTile);
            }

            foreach(var tileData in _levelData.tiles)
            {
                foreach(int childId in tileData.children)
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

        }
    }
}