namespace WordPuzzle.Other
{
    using System.Linq;

    public class WordAlgorithm
    {
        private string[] _words;
        private CharDictionary<int> _letterScores;

        public WordAlgorithm(string[] words)
        {
            _words = words;
            InitLetterScores();
        }

        private void InitLetterScores()
        {
            _letterScores = new CharDictionary<int>();
            _letterScores.AddKeysWithValue(1, 'e', 'a', 'i', 'o', 'n', 'r', 't', 'l', 's', 'u');
            _letterScores.AddKeysWithValue(2, 'd', 'g');
            _letterScores.AddKeysWithValue(3, 'b', 'c', 'm', 'p');
            _letterScores.AddKeysWithValue(4, 'f', 'h', 'v', 'w', 'y');
            _letterScores.AddKeysWithValue(5, 'k');
            _letterScores.AddKeysWithValue(8, 'j', 'x');
            _letterScores.AddKeysWithValue(10, 'q', 'z');
        }

        public bool HasPossibleWord(string word)
        {
            bool result = false;

            result = _words.Where(w => w.Equals(word)).Any();

            return result;
        }

        public int GetWordScore(string word)
        {
            return word.GetScore(GetAllowedLetters(word));
        }

        public int GetPossibleMaxLengthOfWord(string letters)
        {
            int result = 0;

            var allowedLetters = GetAllowedLetters(letters);

            var availableWords = _words.Where(w => w.CanBeComposedOf(allowedLetters));
            if(availableWords.Any())               
                result = availableWords.Max(w => w.Length);
            return result;
        }

        public string GetPossibleMaxScoreOfWord(string letters)
        {
            string result = "";

            var allowedLetters = GetAllowedLetters(letters);

            result = _words.Where(w => w.CanBeComposedOf(allowedLetters))
                           .MaxBy(w => w.GetScore(allowedLetters));
            return result;
        }

        public CharDictionary<int> GetAllowedLetters(string letters)
        {
            var allowedLetters = new CharDictionary<int>();

            for (int i = 0; i < letters.Length; i++)
                allowedLetters[letters[i]]++;

            return allowedLetters;
        }
    }
}