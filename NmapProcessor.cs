using NMap.Scanner;
using NMap.Schema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tview.nmap.shellv1.Enums;
using tview.nmap.shellv1.Scripts;

namespace tview.nmap.shellv1
{
    internal class NmapProcessor
    {
        private Scanner _hostsScanner;

        public bool CheckSelectedOnly { get; set; }

        public string HostsSelected { get; set; }

        public ContextEV TcpEV { get; private set; }
        public ContextEV HostEV { get; private set; }

        private string _exePath;

        public NmapProcessor(string exebutablePath)
        {
            _exePath = exebutablePath;
            TcpEV = new ContextEV();
            HostEV = new ContextEV();

            _hostsScanner = new Scanner(exebutablePath, new Target(), HostEV);
            //_tcpPortScanner.Options.Add(NmapFlag.A);
            

            _hostsScanner.Options.Add(NmapFlag.Verbose);
            _hostsScanner.Options.Add(NmapFlag.PortSpecification, "1");
        }

        public async Task ScanTcpPortsAsync(IEnumerable<string> targets)
        {
            var _tcpPortScanner = new Scanner(_exePath, new Target(), TcpEV);
            _tcpPortScanner.Options.Add(NmapFlag.Verbose);
            _tcpPortScanner.Options.Add(NmapFlag.PortSpecification, "1-9099,9101-65535");
            _tcpPortScanner.Options.Add(NmapFlag.MinProbeParallelization, "1024");
            _tcpPortScanner.Options.Add(NmapFlag.ParallelMinHostGroupSize, "1024");
            _tcpPortScanner.Options.Add(NmapFlag.IcmpEchoDiscovery);
            _tcpPortScanner.Options.Add(NmapFlag.AggressiveTiming);
            _tcpPortScanner.Target = new Target(targets);
            await _tcpPortScanner.PortScanAsync(ScanType.Syn);
        }

        public async Task DiscoverHostsAsync(string target)
        {
            _hostsScanner.Target = new Target(target);
            await _hostsScanner.HostDiscoveryAsync();
        }
        public async Task DiscoverHostsAsync(string gatewayIp, UInt32 cidr)
        {
            await DiscoverHostsAsync($"{gatewayIp}/{cidr}");
        }
        public async Task DiscoverHostsAsync(string gatewayIp, string subnetMask)
        {
            await DiscoverHostsAsync(gatewayIp, SubnetToCIDR(subnetMask));
        }

        public static UInt32 SubnetToCIDR(string subnetMask)
        {
            IPAddress subnetAddress = IPAddress.Parse(subnetMask);
            byte[] ipParts = subnetAddress.GetAddressBytes();
            UInt32 subnet = 16777216 * Convert.ToUInt32(ipParts[0]) + 65536 * Convert.ToUInt32(ipParts[1]) + 256 * Convert.ToUInt32(ipParts[2]) + Convert.ToUInt32(ipParts[3]);
            UInt32 mask = 0x80000000;
            UInt32 subnetConsecutiveOnes = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!(mask & subnet).Equals(mask)) break;

                subnetConsecutiveOnes++;
                mask = mask >> 1;
            }
            return subnetConsecutiveOnes;
        }

        public void Close(ContextEV ev)
        {
            ev.InvokeEvent(EVType.CloseProcess);
        }

        public void CloseAll()
        {
            Close(TcpEV);
            Close(HostEV);
        }
    }
}
