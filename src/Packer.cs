using System;
using System.IO;

namespace ZeroTwo.src {
    class Packer {
        private static string EXE = "C:\\Users\\Jonat\\Desktop\\ZeroTwo.exe";
        private static string RES = "C:\\Users\\Jonat\\Desktop\\Core";
        public static void Main(string[] args) {
            Pack();
        }

        private static void Pack() {
            Byte[] bytes = File.ReadAllBytes(EXE);
            String enc = Convert.ToBase64String(bytes);
            using (StreamWriter sw = File.CreateText(RES)) {
                sw.Write(enc);
            }
        }

    }
}
