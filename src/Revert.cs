using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZeroTwo.src {
    class Reverter {
        private const string EXT = ".02";

        private static byte[] k = Encoding.ASCII.GetBytes("0202020202020202");
        private static byte[] s = Encoding.ASCII.GetBytes("0202020202020202");

        public static void Main(string[] args) {
            Exec();
        }

        private static void Exec() {
            string mainPath = "C:/users/" + Environment.UserName + "/Desktop";
            string[] files = Directory.GetFiles(mainPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files) {
                if (Path.GetExtension(file).Equals(EXT)) {
                    ToggleCipher(file, file.Remove(file.Length - EXT.Length), k);
                }
            }
        }

        private static void ToggleCipher(string inputFile, string outputFile, byte[] passwordBytes) {
            byte[] saltBytes = s;

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;
            AES.Mode = CipherMode.CBC;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            FileStream fs = new FileStream(outputFile, FileMode.Create);

            int data;
            while ((data = cs.ReadByte()) != -1)
                fs.WriteByte((byte)data);

            fs.Close();
            cs.Close();
            fsCrypt.Close();

            File.Delete(inputFile);
        }
    }
}
