namespace WordPuzzle.Game
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using WordPuzzle.Manager;

    public class WordArea : BaseObject
    {
        [SerializeField] private Transform _contentParent;
        [SerializeField] private float _offset;

        private LevelManager _levelManager;

        private List<EmptyTile> _emptyTiles;

        private int _indicator;

        public void SetLevelManager(LevelManager levelManager)
        {
            _levelManager = levelManager;
            _levelManager.TileSet += OnTileSet;
            _levelManager.TileSelected += OnTileSelected;
            _levelManager.TileMoveBack += OnTileMoveBack;
        }

        private void OnTileMoveBack(Tile tile)
        {
            _indicator--;
        }

        private void OnTileSet(List<Tile> tiles)
        {
            SetEmptyTiles(tiles.Count);
        }

        private void SetEmptyTiles(int count)
        {
            ClearEmptyTiles();
            _emptyTiles = new List<EmptyTile>();
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            float size = resourcesManager.GetObjectPrefab<EmptyTile>().transform.localScale.x;

            float originPoint = count * size * -1 / 2f;

            for (int i = 0; i < count; i++)
            {
                EmptyTile newEmptyTile = resourcesManager.GetObject<EmptyTile>(transform);
                Vector2 pos = default;
                pos.y = transform.position.y;
                pos.x = originPoint + (i * (size + _offset));
                newEmptyTile.SetLocalPosition(pos);
                _emptyTiles.Add(newEmptyTile);
            }
        }

        private void ClearEmptyTiles()
        {
            _emptyTiles?.ForEach(t => t.DeActivate());
        }

        private void OnTileSelected(Tile selectedTile)
        {
            if (_emptyTiles.Count <= _indicator)
                return;
            selectedTile.Move(_emptyTiles[_indicator++].transform.position);
        }
    }
}
