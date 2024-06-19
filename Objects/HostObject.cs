using NMap.Schema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tview.nmap.shellv1.Objects
{
    public sealed class HostObject
    {
        public Host Source { get; set; }
        public string TargetIp {  get; set; }
        public string TargetGroup { get; set; }
        public bool IsUp { get; set; }
    }
}
