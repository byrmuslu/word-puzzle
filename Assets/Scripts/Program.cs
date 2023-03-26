//using Scrabble.Collections;
//using Scrabble.Extensions;

//// Enable hackerman mode
//Console.ForegroundColor = ConsoleColor.Green;

//var letterScores = new CharDictionary<int>();
//letterScores.AddKeysWithValue(1, 'e', 'a', 'i', 'o', 'n', 'r', 't', 'l', 's', 'u');
//letterScores.AddKeysWithValue(2, 'd', 'g');
//letterScores.AddKeysWithValue(3, 'b', 'c', 'm', 'p');
//letterScores.AddKeysWithValue(4, 'f', 'h', 'v', 'w', 'y');
//letterScores.AddKeysWithValue(5, 'k');
//letterScores.AddKeysWithValue(8, 'j', 'x');
//letterScores.AddKeysWithValue(10, 'q', 'z');

//var wordNumber = int.Parse(Console.ReadLine() ?? "0");
//var wordList = new List<string>();

//for (int i = 0; i < wordNumber; i++)
//    wordList.Add(Console.ReadLine() ?? "");

//var drawnLetters = Console.ReadLine() ?? "aaaaaaa";
//var allowedLetters = new CharDictionary<int>();

//for (int i = 0; i < drawnLetters.Length; i++)
//    allowedLetters[drawnLetters[i]]++;

//var result = wordList.Where(x => x.CanBeComposedOf(allowedLetters))
//                     .MaxBy(x => x.GetScore(letterScores));

//Console.WriteLine(result);