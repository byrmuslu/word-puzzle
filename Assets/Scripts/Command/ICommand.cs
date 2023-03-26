namespace WordPuzzle.Command
{
    using System;
    
    public interface ICommand
    {
        public void Execute();
        public void Undo();
    }
}