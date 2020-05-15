using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Cyphers
    {
        public Cyphers()
        {
            string a1 = simpleReverse("Vraťte se ke mně a já se vrátím k vám, praví Hospodin zástupů.");
            string a2 = alphabetReverse("Víno a pivo, tak zní mé proroctví");
            string a3 = alphabetMove("Celou zemi totiž naplní poznání slávy Hospodinovy.");
            string a4 = squere("AbysjednalspravedlivěmilovalmilosrdenstvíakráčelsesvýmBohemvpokoře");
            string a5 = insertStupidities("kapr");
            string a6 = rotate("Kdyby byla včela medvědem, stavěla by své hnízdo při zemi");
            string b6 = rotate("Kdyby pak šel medvěd za medem, nemusel by šplhat větvemi");

            string b = a1 + a2 + a3 + a4;
        }

        public string simpleReverse(string input)
        {
            return new string(input.RemoveDiacritics(false).Reverse().ToArray());
        }

        public string alphabetReverse(string input)
        {
            // reverse alphabet
            char[] alphabet = new char[26];
            for(int i = 0; i < 26; i++)
            {
                alphabet[i] = (char)(i + 97);
            }
            alphabet = alphabet.Reverse().ToArray();

            //
            input = input.RemoveDiacritics(false);

            char[] result = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                result[i] = alphabet[((int)input[i]) - 97];
            }

            return new string(result);
        }

        public string alphabetMove(string input, int move = 12)
        {
            input = input.RemoveDiacritics(false);

            char[] result = new char[input.Length];
            for(int i = 0; i < input.Length; i++)
            {
                int charNum = input[i];
                int resultNum = (((charNum - 97) + move) % 26) + 97;
                result[i] = (char)resultNum;
            }

            return new string(result);
        }

        public string squere(string input, string matrix = null)
        {
            input = input.RemoveDiacritics(false);

            matrix = "yhtedsgbukuojcrafpilvnmqz";

            char[] result = new char[input.Length];
            for (int i = 0; i < input.Length; i += 2)
            {
                int i1 = matrix.IndexOf(input[i]);
                int x1 = i1 % 5;
                int y1 = i1 / 5;

                int i2 = matrix.IndexOf(input[i + 1]);
                int x2 = i2 % 5;
                int y2 = i2 / 5;

                result[i] = matrix[(y1*5) + x2];
                result[i + 1] = matrix[(y2 * 5) + x1];
            }

            return new string(result);
        }

        public string rotate(string input, int frequency = 4)
        {
            input = input.RemoveDiacritics(false);

            char[] result = new char[input.Length];
            int starting = 0;
            int i = 0;

            foreach (char c in input)
            {
                result[i] = c;

                i += frequency;
                if (i >= input.Length)
                {
                    starting += 1;
                    i = starting;
                }
            }

            return new string(result);
        }

        public string insertStupidities(string input, int frequency = 4, int start = 2, int end = 2)
        {
            int totalRandomLenght = (input.Length - 1) * (frequency - 1) + start + end;
            string result = Random(totalRandomLenght);

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                result = result.Insert(i * (frequency) + start, c.ToString());
            }

            return result;
        }

        public string Random(int length, string chars = "abcdefghijklmnopqrstuvwxyz")
        {
            var random = new Random();
            string str = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return str;
        }

        //private char toChar(int item)
        //{
        //    int alphabethLength = 26;

        //    if (item < 65)
        //    {
        //        do
        //        {
        //            item += alphabethLength;
        //        }
        //        while (item < 65);
        //        return (char)item;
        //    }

        //    if (item <= 90)
        //        return (char)item;

        //    if (item < 97)
        //        return
        //}
    }
}
