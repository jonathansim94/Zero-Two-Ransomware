# Zero-Two-Ransomware

A cryptolocker ransomware developed for a Security exam.

![Alt text](https://raw.githubusercontent.com/jonathansim94/Zero-Two-Ransomware/main/ZT.ico?raw=true "Title")

ZeroTwo is a cryptolocker ransomware written in C# effective against an up-to-date Windows system with virus protection. ZeroTwo presents itself as a simple image. In fact, once opened, nothing happens except for the image itself opening. Yet, once the file is executed, the system is infected with malicious and absolutely "silent" extraction of destructive code and infected registry keys. The lack of suspicious video output is manageable through the use of various VBS and batch scripts. Once the computer is accessed again, however, the malware activates, encrypts the user's files and extracts a reverter with instructions to pay the ransom. In addition, once the damage is done, the malware self-deletes from the system, leaving no trace. I made the malware undetectable by antivirus software through antivirus evasion techniques, such as encrypting and encoding the malicious payload. Also, thanks to some heuristics, I was able to point out some vulnerabilities specific to Windows Defender, especially with regard to avoiding its cloud analysis.

Instructions in Visual Studio:
- to compile the Cryptolocker exclude the Revert.cs and the Reverter icon from compilation
- to compile the Reverter exclude the ZeroTwo.cs, the res files and the Cryptolocker icon from compilation 
- to run the Packer just run Packer.cs with the compiled exe on Desktop
- to compile 02 exclude all except 02.cs, the Core file generated from the Packer and the fill file generated with python
- Don't forget to fill the payload with the python script output and to change files meta data

