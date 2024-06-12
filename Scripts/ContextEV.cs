using NMap.Schema;
using NMap.Schema.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tview.nmap.shellv1.Attributes;
using tview.nmap.shellv1.Enums;

namespace tview.nmap.shellv1.Scripts
{
    public class ContextEV : NmapScript
    {
        public delegate void OutputHandler(string output);
        public delegate void ExceptionHandler(string ex);
        public delegate void HostDiscoveryHandler(IEnumerable<Host> hosts);
        public delegate void NmapResultHandler(NMapResult result);
        public delegate void ProcessInfoHandler(ProcessStartInfo info);
        public delegate void ProcessExitHandler();

        public delegate void CloseHandler();

        public event OutputHandler Output;
        public event ExceptionHandler Exception;
        public event HostDiscoveryHandler HostDiscovery;
        public event NmapResultHandler Result;
        public event ProcessInfoHandler StartInfo;
        public event ProcessExitHandler ProcessExit;

        public event CloseHandler Close;

        [ScriptType(EVType.CmdOutput)]
        public void OnOutput(string output)
        {
            Output?.Invoke(output);
        }
        [ScriptType(EVType.CmdOutput)]
        public void OnException(string ex)
        {
            Exception?.Invoke(ex);
        }
        [ScriptType(EVType.HostDiscovery)]
        public void OnHostDiscovery(IEnumerable<Host> hosts)
        {
            HostDiscovery?.Invoke(hosts);
        }
        [ScriptType(EVType.NmapResult)]
        public void OnHostDiscovery(NMapResult result)
        {
            Result?.Invoke(result);
        }
        [ScriptType(EVType.CmdStartArgs)]
        public void OnHostDiscovery(ProcessStartInfo info)
        {
            StartInfo?.Invoke(info);
        }
        [ScriptType(EVType.CmdExit)]
        public void OnHostDiscovery()
        {
            ProcessExit?.Invoke();
        }
        [ScriptType(EVType.CloseProcess)]
        public void OnClose()
        {
            Close?.Invoke();
        }
    }
}
