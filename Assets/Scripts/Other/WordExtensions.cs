namespace WordPuzzle.Other
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class WordExtensions
    {
        public static bool CanBeComposedOf(this string source, ICharDictionary<int> allowedLetters)
        {
            allowedLetters = allowedLetters.Copy();

            foreach (var letter in source)
            {
                allowedLetters[letter]--;
                if (allowedLetters[letter] < 0)
                    return false;
            }
           
            return true;
        }

        public static int GetScore(this string source, ICharDictionary<int> letterScores)
        {
            int score = 0;

            foreach (var letter in source)
                score += letterScores[letter];

            return score;
        }

        public static T MaxBy<T, TProp>(this IEnumerable<T> source, Func<T, TProp> predicate)
            where TProp : IComparable<TProp>
        {
            var max = source.FirstOrDefault();

            foreach (var item in source.Skip(1))
            {
                if (predicate(item).CompareTo(predicate(max)) > 0)
                    max = item;
            }

            return max;
        }
    }
}
