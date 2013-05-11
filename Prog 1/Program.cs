using HTSUtils;
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

            var username = args[0];
            var password = args[1];

            var page = new ChallengePage("https://www.hackthissite.org/missions/prog/1/", username, password);
            IEnumerable<WordDetails> scrambledWords = GetScrambledWords(page.GetChallengePage());

            var unscrambledWords = UnscrambleWords(scrambledWords, words);
            page.SubmitAnswer(String.Join(",", unscrambledWords.ToArray()));

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static IEnumerable<String> UnscrambleWords(IEnumerable<WordDetails> scrambledWords, IEnumerable<WordDetails> words)
        {
            List<String> unscrambledWords = new List<String>();

            foreach (var scrambledWord in scrambledWords)
            {
                String word = words.First(w => scrambledWord.EqualsCharCount(w)).Word;
                unscrambledWords.Add(word);
            }

            Console.WriteLine("Unscrambled words: {0}", String.Join(",", unscrambledWords.ToArray()));

            return unscrambledWords;
        }

        private static readonly Regex scrambledWordRegex = new Regex(@"<td><li>((:?[a-z]|[0-9]|:|\()+?)</li></td>", RegexOptions.Compiled);
        private static IEnumerable<WordDetails> GetScrambledWords(String html)
        {
            List<WordDetails> words = new List<WordDetails>();

            var matches = scrambledWordRegex.Matches(html);
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                var word = new WordDetails(match.Groups[1].Value);
                words.Add(word);
            }

            Console.WriteLine("Scrambled words: {0}", String.Join(",", words.Select(w => w.Word).ToArray()));

            return words;
        }

        private static IEnumerable<WordDetails> BuildWords(String fileName)
        {
            List<WordDetails> words = new List<WordDetails>();

            foreach (String line in File.ReadAllLines(fileName))
            {
                WordDetails details = new WordDetails(line);
                words.Add(details);
            }

            return words;
        }
    }
}
