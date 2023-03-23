namespace WordPuzzle.Command
{
    using UnityEngine;

    public interface ICommandMovement
    {
        public void Move(Vector2 target);
        public void MoveBack();
    }
}