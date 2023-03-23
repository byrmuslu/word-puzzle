namespace WordPuzzle.Data
{
    using System;
    using UnityEngine;

    [Serializable]
    public class TileData
    {
        public int id;
        public Vector3 position;
        public string character;
        public int[] children;
    }

    public class LevelData
    {
        public string title;
        public TileData[] tiles;
    }
}