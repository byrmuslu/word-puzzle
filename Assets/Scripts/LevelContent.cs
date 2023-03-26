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

    // ****** For case study *******
    [CreateAssetMenu(fileName = "level", menuName = "Level", order = 0)]
    public class LevelContent : ScriptableObject
    {
        public string dataJSON;
        public int levelNumber;

        private LevelData _data;

        public LevelData GetData()
        {
            if(_data == null)
                _data = JsonUtility.FromJson<LevelData>(dataJSON);
            return _data;
        }
    }
}