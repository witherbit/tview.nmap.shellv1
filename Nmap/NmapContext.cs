using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using NMap.Scanner.Extensions;
using NMap.Schema;
using tview.nmap.shellv1.Enums;
using tview.nmap.shellv1.Scripts;
using wcheck.Utils;

namespace NMap.Scanner
{
    public class NmapContext
    {
        private ContextEV _ev;
        public NmapContext(string ExecutablePath, ContextEV ev, ProcessWindowStyle WindowStyle = ProcessWindowStyle.Hidden)
        {
            _ev = ev;
            Path = ExecutablePath;
            OutputPath = System.IO.Path.GetTempFileName();
            Options = new NmapOptions();
            this.WindowStyle = WindowStyle;
        }
        public readonly string Path;
        public string OutputPath { get; set; }
        public NmapOptions Options { get; set; }
        public string Target { get; set; }

        public ProcessWindowStyle WindowStyle { get; set; }
        public virtual NMapResult Run()
        {
            if (string.IsNullOrEmpty(OutputPath))
            {
                throw new ApplicationException("Nmap output file path is null or empty");
            }

            if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
            {
                throw new ApplicationException("Path to Nmap is invalid");
            }

            if (string.IsNullOrEmpty(Target))
            {
                throw new ApplicationException("Attempted run on empty target");
            }

            if (Options == null)
            {
                throw new ApplicationException("Nmap options null");
            }

            Options[NmapFlag.XmlOutput] = OutputPath;

            using (var Process = new Process())
            {
                Process.StartInfo.FileName = Path;
                Process.StartInfo.Arguments = $"{Options} {Target}";
                Process.StartInfo.WindowStyle = WindowStyle;
                Process.StartInfo.RedirectStandardError = true;
                Process.StartInfo.RedirectStandardOutput = true;

                if(WindowStyle == ProcessWindowStyle.Hidden)
                    Process.StartInfo.CreateNoWindow = true;

                Process.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
                {
                    _ev.InvokeEvent(EVType.CmdOutput, e.Data);
                });
                Process.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
                {
                    _ev.InvokeEvent(EVType.CmdException, e.Data);
                });

                _ev.InvokeEvent(EVType.CmdStartArgs, Process.StartInfo);

                Process.Start();
                _ev.Close += () => 
                {
                    try
                    {
                        Process.Kill();
                    }
                    catch { }
                };
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();
                Process.WaitForExit();


                _ev.InvokeEvent(EVType.CmdExit);

                if (!File.Exists(OutputPath))
                {
                    throw new NmapException(Process.StartInfo.Arguments);
                }
            }

            return Serialization.DeserializeFromFile<NMapResult>(OutputPath);
        }
    }
}