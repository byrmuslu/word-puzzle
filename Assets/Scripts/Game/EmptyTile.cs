namespace WordPuzzle.Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EmptyTile : BaseObject
    {
        public void SetLocalPosition(Vector2 pos)
        {
            transform.localPosition = pos;
        }
    }
}