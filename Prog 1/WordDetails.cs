using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prog_1
{
    public class WordDetails
    {
        public String Word { get; private set; }
        private Dictionary<char, int> charCounts;

        public WordDetails(String word)
        {
            this.Word = word;
            charCounts = new Dictionary<char, int>();

            var chars = word.ToCharArray();
            foreach (var currentChar in chars)
                IncrementCount(currentChar);
        }

        private void IncrementCount(char c)
        {
            if (!charCounts.ContainsKey(c))
                charCounts.Add(c, 0);
            charCounts[c] += 1; 
        }

        public int GetCount(char c)
        {
            return charCounts.ContainsKey(c) ? charCounts[c] : 0;
        }

        public override string ToString()
        {
            String charCountsDesc = String.Join(", ", charCounts.Keys.Select(k => String.Format("{0}:{1}", k, charCounts[k])));

            return String.Format("{0}:[{1}]", Word, charCountsDesc);
        }

        public bool EqualsCharCount(WordDetails other)
        {
            foreach (var charCountKey in charCounts.Keys)
            {
                if (other.GetCount(charCountKey) != GetCount(charCountKey))
                    return false;
            }
            return true;
        }
    }
}
