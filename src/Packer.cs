using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZeroTwo.src {
    class Packer {
        private static string EXE = "C:\\Users\\Jonat\\Desktop\\ZeroTwo.exe";
        private static string ENC_EXE = "C:\\Users\\Jonat\\Desktop\\EncExe";
        private static string RES = "C:\\Users\\Jonat\\Desktop\\Core";

        private static byte[] k = Encoding.ASCII.GetBytes("0202020202020202");
        private static byte[] s = Encoding.ASCII.GetBytes("0202020202020202");

        public static void Main(string[] args) {
            Pack();
        }

        private static void Pack() {
            string cryptFile = ENC_EXE;

            FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(k, s, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;
            AES.Mode = CipherMode.CBC;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
            FileStream fs = new FileStream(EXE, FileMode.Open);

            int data;
            while ((data = fs.ReadByte()) != -1)
                cs.WriteByte((byte)data);

            fs.Close();
            cs.Close();
            fsCrypt.Close();

            //File.Delete(EXE);

            byte[] bytes = File.ReadAllBytes(ENC_EXE);
            string encoded = Convert.ToBase64String(bytes);
            using (StreamWriter sw = File.CreateText(RES)) {
                sw.Write(encoded);
            }
        }
    }
}
