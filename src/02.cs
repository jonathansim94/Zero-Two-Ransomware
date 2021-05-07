using System;
using System.IO;
using System.Reflection;

namespace ZeroTwo.src {
    class _02 {
        public static void Main(string[] args) {
            Unp();
        }

        private static void Unp() {
            var assembly = Assembly.GetExecutingAssembly();
            var res = "ZeroTwo.src.corefile.Core";
            using (Stream stream = assembly.GetManifestResourceStream(res)) {
                using (StreamReader reader = new StreamReader(stream)) {
                    string core = reader.ReadToEnd();
                    Byte[] bytes = Convert.FromBase64String(core);
                    var loaded = Assembly.Load(bytes);
                    var entryPoint = loaded.EntryPoint;
                    var commandArgs = new string[0];
                    var returnValue = entryPoint.Invoke(null, new object[] { commandArgs });
                }
            }
        }
    }

}
