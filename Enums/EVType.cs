using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tview.nmap.shellv1.Enums
{
    public enum EVType
    {
        CmdOutput,
        CmdException,
        HostDiscovery,
        NmapResult,
        CmdStartArgs,
        CmdExit,
        CloseProcess
    }
}
