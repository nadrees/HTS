using HTSUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prog_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = args[0];
            var password = args[1];

            var page = new ChallengePage("https://www.hackthissite.org/missions/prog/2/", username, password);

            // start challenge
            page.GetChallengePage();
            // get image for challenge
            var png = GetPngImage(page);

            var morse = ParseImage(png);
            Console.WriteLine("Morse encoded password: {0}", morse);

            var result = DecodeMorse(morse);
            Console.WriteLine("Decoded morse: {0}", result);

            page.SubmitAnswer(result);
        }

        private static String DecodeMorse(string morse)
        {
            var dict = new Dictionary<String, char> 
            {
                {".-", 'A'}, 
                {"-...", 'B'},
                {"-.-.", 'C'}, 
                {"-..", 'D'},
                {".", 'E'},
                {"..-.", 'F'},
                {"--.", 'G'},
                {"....", 'H'},
                {"..", 'I'},
                {".---", 'J'},
                {"-.-", 'K'},
                {".-..", 'L'},
                {"--", 'M'},
                {"-.", 'N'},
                {"---", 'O'},
                {".--.", 'P'},
                {"--.-", 'Q'},
                {".-.", 'R'},
                {"...", 'S'},
                {"-", 'T'},
                {"..-", 'U'},
                {"...-", 'V'},
                {".--", 'W'},
                {"-..-", 'X'},
                {"-.--", 'Y'},
                {"--..", 'Z'},
                {".----", '1'},
                {"..---", '2'},
                {"...--", '3'},
                {"....-", '4'},
                {".....", '5'},
                {"-....", '6'},
                {"--...", '7'},
                {"---..", '8'},
                {"----.", '9'},
                {"-----", '0'}
            };

            List<char> chars = new List<char>();
            foreach (var part in morse.Split(' ').Where(s => !String.IsNullOrWhiteSpace(s)))
                chars.Add(dict[part]);

            return new String(chars.ToArray());
        }

        private static String ParseImage(Bitmap png)
        {
            int lastPixelLocation = 0;
            List<char> chars = new List<char>();

            for (int y = 0; y < png.Height; y++)
            {
                for (int x = 0; x < png.Width; x++)
                {
                    var color = png.GetPixel(x, y);
                    if (color.R != Color.Black.R &&
                        color.G != Color.Black.G &&
                        color.B != Color.Black.B)
                    {
                        int currentIndex = (y * png.Width) + x;

                        int diff = currentIndex - lastPixelLocation;
                        chars.Add((char)diff);
                        lastPixelLocation = currentIndex;
                    }
                }
            }

            return new String(chars.ToArray());
        }

        private static Bitmap GetPngImage(ChallengePage page)
        {
            var request = page.CreateAuthenticatedWebRequest("https://www.hackthissite.org/missions/prog/2/PNG/");
            var resposne = request.GetResponse();
            var image = new Bitmap(resposne.GetResponseStream());

            Console.WriteLine("Retrieved png image");

            return image;
        }
    }
}
