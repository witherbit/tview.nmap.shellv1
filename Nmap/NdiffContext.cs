using System;
using System.Diagnostics;
using System.IO;

namespace NMap.Scanner
{
    public class NdiffContext
    {
        public NdiffContext(string ExecutablePath)
        {
            Path = ExecutablePath;
            Options = new NdiffOptions();
        }
        public string Path { get; set; }
        public NdiffOptions Options { get; set; }
        public string File1 { get; set; }
        public string File2 { get; set; }
        public string Run()
        {
            if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
            {
                throw new ApplicationException("Path to ndiff is invalid");
            }

            if (string.IsNullOrEmpty(File1))
            {
                throw new ApplicationException("Attempted run on missing comparison File1.");
            }

            if (string.IsNullOrEmpty(File2))
            {
                throw new ApplicationException("Attempted run on missing comparison File2.");
            }

            if (Options == null)
            {
                throw new ApplicationException("Ndiff options null");
            }

            string Output, Error;

            using (var Process = new Process())
            {
                Process.StartInfo.FileName = Path;
                Process.StartInfo.Arguments = $"{Options} {File1} {File2}";
                Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process.StartInfo.UseShellExecute = false;
                Process.StartInfo.RedirectStandardOutput = true;
                Process.StartInfo.RedirectStandardError = true;
                Process.Start();
                Process.WaitForExit();

                Output = Process.StandardOutput.ReadToEnd();
                Error = Process.StandardError.ReadToEnd();
            }
            

            return string.IsNullOrEmpty(Error) ? Output : Error;
        }
    }
}