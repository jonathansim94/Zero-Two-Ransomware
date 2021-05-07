using System;
using System.IO;

namespace ZeroTwo.src {
    class Packer {
        private static string EXE = "C:\\Users\\Jonat\\Desktop\\ZeroTwo.exe";
        private static string RES = "C:\\Users\\Jonat\\Desktop\\Core";
        public static void Main(string[] args) {
            Pack();
        }

        public static char cipher(char ch, int key) {
            if (!char.IsLetter(ch)) {
                return ch;
            }
            char d = char.IsUpper(ch) ? 'A' : 'a';
            return (char)((((ch + key) - d) % 26) + d);
        }

        private static string Encipher(string input, int key) {
            string output = string.Empty;

            foreach (char ch in input)
                output += cipher(ch, key);

            return output;
        }

        private static string Decipher(string input, int key) {
            return Encipher(input, 26 - key);
        }

        private static void Pack() {
            Byte[] bytes = File.ReadAllBytes(EXE);
            String enc = Convert.ToBase64String(bytes);
            using (StreamWriter sw = File.CreateText(RES)) {
                string cipherText = Encipher(enc, 5);
                sw.Write(cipherText);
            }
        }

    }
}
