using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Prog_1
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<WordDetails> words = BuildWords("wordlist.txt");
            IEnumerable<WordDetails> scrambledWords = BuildWords("scrambledWords.txt");

            var unscrambledWords = UnscrambleWords(scrambledWords, words);
            File.WriteAllLines("result.txt", new[] { String.Join(",", unscrambledWords.ToArray()) });
        }

        private static IEnumerable<String> UnscrambleWords(IEnumerable<WordDetails> scrambledWords, IEnumerable<WordDetails> words)
        {
            List<String> unscrambledWords = new List<String>();

            foreach (var scrambledWord in scrambledWords)
            {
                String word = words.First(w => scrambledWord.EqualsCharCount(w)).Word;
                unscrambledWords.Add(word);
            }

            return unscrambledWords;
        }

        private static IEnumerable<WordDetails> BuildWords(String fileName)
        {
            List<WordDetails> words = new List<WordDetails>();

            foreach (String line in File.ReadAllLines(fileName))
            {
                var chars = line.ToCharArray();
                WordDetails details = new WordDetails(line);
                foreach (var currentChar in chars)
                    details.IncrementCount(currentChar);

                words.Add(details);
            }

            return words;
        }
    }
}
