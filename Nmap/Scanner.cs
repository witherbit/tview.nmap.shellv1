using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using NMap.Schema;
using NMap.Schema.Models;
using tview.nmap.shellv1.Enums;
using tview.nmap.shellv1.Scripts;

namespace NMap.Scanner
{
    public class Scanner
    {
        private readonly Dictionary<ScanType, NmapFlag> ScanTypeToNmapFlag = new Dictionary<ScanType, NmapFlag>
            {
                {ScanType.Null, NmapFlag.TcpNullScan},
                {ScanType.Fin, NmapFlag.FinScan},
                {ScanType.Xmas, NmapFlag.XmasScan},
                {ScanType.Syn, NmapFlag.TcpSynScan},
                {ScanType.Connect, NmapFlag.ConnectScan},
                {ScanType.Ack, NmapFlag.AckScan},
                {ScanType.Window, NmapFlag.WindowScan},
                {ScanType.Maimon, NmapFlag.MaimonScan},
                {ScanType.SctpInit, NmapFlag.SctpInitScan},
                {ScanType.SctpCookieEcho, NmapFlag.CookieEchoScan},
                {ScanType.Udp, NmapFlag.UdpScan}
            };

        public ContextEV EV { get; private set; }

        public Scanner(string ExecutablePath, Target Target, ContextEV ev, ProcessWindowStyle NmapWindowStyle = ProcessWindowStyle.Hidden)
        {
            this.ExecutablePath = ExecutablePath;
            this.Target = Target;
            this.NmapWindowStyle = NmapWindowStyle;
            EV = ev;
        }

        public readonly string ExecutablePath;
        public Target Target { get; set; }
        public NmapOptions Options { get; set; } = new NmapOptions();
        public ProcessWindowStyle NmapWindowStyle { get; set; }
        private NmapContext GetContext()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                throw new ApplicationException("No network reachable");
            }

            var NmapContext = new NmapContext(ExecutablePath, EV)
            {
                Target = Target.ToString(),
                WindowStyle = NmapWindowStyle
            };

            if (Options != null)
            {
                NmapContext.Options.AddAll(Options);
            }

            return NmapContext;
        }
        public async Task HostDiscoveryAsync()
        {
            await Task.Run(() =>
            {
                var Ctx = GetContext();
                Ctx.Options.AddAll(new[]
                    {
                    NmapFlag.TcpSynScan,
                    NmapFlag.OsDetection
                });

                var result = Ctx.Run().Hosts.Where(x => x.Status.State == Schema.Enums.StatusState.Up);
                EV.InvokeEvent(EVType.HostDiscovery, result);
            });
        }
        private NmapContext _portScanCommon(ScanType ScanType, string Ports)
        {
            var NmapContext = GetContext();
            NmapContext.Options.AddAll(new[]
                {
                    NmapFlag.ServiceVersion,
                    NmapFlag.OsDetection
                });
            if (ScanType != ScanType.Default)
            {
                NmapContext.Options.Add(ScanTypeToNmapFlag[ScanType]);
            }
            //if (ScanType != ScanType.Default && ScanType != ScanType.Udp)
            //{
            //    NmapContext.Options.Add(NmapFlag.UdpScan);
            //}
            if (!string.IsNullOrEmpty(Ports))
            {
                NmapContext.Options.Add(NmapFlag.PortSpecification, Ports);
            }

            return NmapContext;
        }
        public async Task PortScanAsync()
        {
            await Task.Run(() =>
            {
                var NmapContext = _portScanCommon(ScanType.Default, null);
                var result = NmapContext.Run();
                EV.InvokeEvent(EVType.NmapResult, result);
            });
        }
        public async Task PortScanAsync(ScanType ScanType)
        {
            await Task.Run(() =>
            {
                var NmapContext = _portScanCommon(ScanType, null);
                var result = NmapContext.Run();
                EV.InvokeEvent(EVType.NmapResult, result);
            });
        }
        public async Task PortScanAsync(ScanType ScanType, IEnumerable<int> Ports)
        {
            await Task.Run(() =>
            {
                var NmapContext = _portScanCommon(ScanType,
                                              string.Join(",",
                                                          Ports.Select(X => X.ToString(CultureInfo.InvariantCulture))));
                var result = NmapContext.Run();
                EV.InvokeEvent(EVType.NmapResult, result);
            });
            
        }
        public async Task PortScanAsync(ScanType ScanType, string Ports)
        {
            await Task.Run(() =>
            {
                var NmapContext = _portScanCommon(ScanType, Ports);
                var result = NmapContext.Run();
                EV.InvokeEvent(EVType.NmapResult, result);
            });
        }
        public NetworkInterface[] GetAllHostNetworkInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces();
        }
    }
}
