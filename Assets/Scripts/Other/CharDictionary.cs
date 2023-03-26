namespace WordPuzzle.Other
{
    public interface ICharDictionary<T>
    {
        T this[char key] { get;set; }

        ICharDictionary<T> Copy();
    }

    public class CharDictionary<T> : ICharDictionary<T>
    {
        private static int HashFunction(char key)
            => key - 'a';

        private T[] _innerArray = new T[26];

        public T this[char key]
        {
            get => _innerArray[HashFunction(key)];
            set => _innerArray[HashFunction(key)] = value;
        }

        public void AddKeysWithValue(T value, params char[] keys)
        {
            foreach (var key in keys)
                _innerArray[HashFunction(key)] = value;
        }

        public ICharDictionary<T> Copy()
        {
            var copyArray = new T[26];
            _innerArray.CopyTo(copyArray, 0);

            return new CharDictionary<T>
            {
                _innerArray = copyArray
            };
        }
    }
}