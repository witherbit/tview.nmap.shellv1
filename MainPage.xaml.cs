using Newtonsoft.Json;
using NMap.Scanner;
using NMap.Schema;
using NMap.Schema.Enums;
using NMap.Schema.Models;
using pwither.thrd.Locker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using tview.nmap.shellv1.Controls;
using tview.nmap.shellv1.Objects;
using wcheck.Pages;
using wcheck.Utils;
using wcheck.wcontrols;
using wshell.Abstract;
using wshell.Utils;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace tview.nmap.shellv1
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public Host[] Hosts { get; private set; }
        public NMapResult Result { get; private set; }
        private static Brush _inProgress = "#77cefc".GetBrush();
        private static Brush _inComplete = "#fca577".GetBrush();
        internal static NmapShell Shell { get; private set; }
        internal static NmapProcessor Processor { get; private set; }
        public MainPage(NmapShell shell)
        {
            InitializeComponent();
            Shell = shell;
            Processor = new NmapProcessor(Shell.Settings.GetValue<string>("pNmapPath"));
            uiCircleWait.Fill = _inProgress;
        }

        public void StartTask(string hosts, string gatewayIp, string subnetMask)
        {
            Processor.OutputRedirect = Shell.GetEventRedirect();

            Processor.OutputRedirect.InvokeEventAsync("state", "Запуск задания [1/7]");

            var myIp = GetIp();
            hosts = hosts.Replace("localhost", myIp).Replace("127.0.0.1", myIp);
            Processor.HostsSelected = hosts;
            Processor.CheckSelectedOnly = Shell.Settings.GetValue<bool>("pChkOnlySelected");

            this.Invoke(() =>
            {
                uiCircleWait.Fill = _inComplete;

                uiCircleInit.Fill = _inProgress;
            });

            Processor.OutputRedirect.InvokeEventAsync("state", "Инициализация Nmap API [2/7]");

            Processor.TcpEV.StartInfo += (info => 
            { 
                //Log(new LogContent($"Start nmap.exe with {info.Arguments} [TCP]", Processor));
                Processor.OutputRedirect.InvokeEventAsync("output", $"Start nmap.exe for port scan task", Processor);

            });
            Processor.HostEV.StartInfo += (info =>
            {
                //Log(new LogContent($"Start nmap.exe with {info.Arguments} [HOSTS DISCOVER]", Processor));
                Processor.OutputRedirect.InvokeEventAsync("output", $"Start nmap.exe for hosts discover task", Processor);
            });

            Processor.TcpEV.Output += OnOutputTcp;
            Processor.HostEV.Output += OnOutputHost;

            Processor.TcpEV.ProcessExit += (() => Log(new LogContent($"Process Exit [TCP]", Processor)));
            Processor.HostEV.ProcessExit += (() => Log(new LogContent($"Process Exit [HOSTS DISCOVER]", Processor)));

            Processor.TcpEV.Result += OnResult;

            Processor.HostEV.HostDiscovery += OnHostDiscovery;

            Shell.CancellationToken.Register(() =>
            {
                Processor.CloseAll();
            });

            this.Invoke(() =>
            {
                uiCircleInit.Fill = _inComplete;
                uiCircleSearch.Fill = _inProgress;
            });

            Processor.OutputRedirect.InvokeEventAsync("state", "Поиск активных хостов [3/7]");

            Processor.DiscoverHostsAsync(gatewayIp, subnetMask);
        }

        bool break1 = false;
        bool break2 = false;

        private void OnOutputHost(string output)
        {
            if (output == null) return;
            this.Invoke(()=>uiTextCaption.Text = output);
            //Log(new LogContent($"[HOSTS DISCOVER]: {output}", Processor));
            Processor.OutputRedirect.InvokeEventAsync("output", output);
            if (output.Contains("Initiating SYN Stealth Scan") && !break1)
            {
                break1 = true;
                this.Invoke(() =>
                {
                    uiCircleSearch.Fill = _inComplete;
                    uiCircleScanDefault.Fill = _inProgress;
                });

                Processor.OutputRedirect.InvokeEventAsync("state", "Распределение списков [4/7]");
            }
            else if (output.Contains("Initiating OS detection") && !break2)
            {
                break2 = true;
                this.Invoke(() =>
                {
                    uiCircleScanDefault.Fill = _inComplete;
                    uiCircleGetOS.Fill = _inProgress;
                });
                Processor.OutputRedirect.InvokeEventAsync("state", "Получение информации об ОС [5/7]");
            }
        }

        private void OnResult(NMapResult result)
        {
            var locker = new Lock();
            using (locker.Write())
            {
                Result = result;
            }
            this.Invoke(() =>
            {
                uiCircleScan.Fill = _inComplete;
                uiCircleStop.Fill = _inProgress;
            });
            Processor.OutputRedirect.InvokeEventAsync("state", "Завершение работы Nmap [7/7]");
            ShowResults();
        }

        private void OnOutputTcp(string output)
        {
            if (output == null) return;
            this.Invoke(() => uiTextCaption.Text = output);
            //Log(new LogContent($"[TCP]: {output}", Processor));
            Processor.OutputRedirect.InvokeEventAsync("output", output);
        }

        private void OnHostDiscovery(IEnumerable<Host> hosts)
        {
            this.Invoke(() =>
            {
                uiCircleGetOS.Fill = _inComplete;
                uiCircleScan.Fill = _inProgress;
            });

            Processor.OutputRedirect.InvokeEventAsync("state", "Сканирование портов [6/7]");

            Hosts = hosts.ToArray();
            var hostsTargets = new List<string>();
            foreach (Host host in hosts)
                if (Processor.CheckSelectedOnly)
                {
                    if(Processor.CheckAcceptedIp(host.Address.IP))
                        hostsTargets.Add(host.Address.IP);
                }
                else
                {
                    hostsTargets.Add(host.Address.IP);
                }
                    
            Processor.ScanTcpPortsAsync(hostsTargets);
        }
        private void ShowResults()
        {
            this.Invoke(() =>
            {
                uiCircleScan.Fill = _inComplete;
                uiGridCaption.Visibility = Visibility.Collapsed;
                uiStackPanel.Visibility = Visibility.Visible;
                var netmap = Shell.InvokeCustomRequest("b4877dc5-a5b5-4b7e-b08b-1b1995e8c8d8", "type.gethostswithgroups").GetProviding<string>().Replace("localhost", GetIp()).Replace("127.0.0.1", GetIp()).Split(',', StringSplitOptions.RemoveEmptyEntries);
                var objects = GroupObject.FromMapping(netmap, Result);
                foreach (var obj in objects)
                    uiStackPanel.Children.Add(obj.Expander);
                Processor.OutputRedirect.InvokeEventAsync("state", "Выполнено");
                Processor.OutputRedirect.InvokeEventAsync("complete", new NmapStatisticTemplate());
            });
        }

        public void Log(LogContent content)
        {
            this.Invoke(() =>
            {
                Logger.Log(content);
            });
        }

        private static string GetIp()
        {
            string result = "?";
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {

                var defGate = from nics in NetworkInterface.GetAllNetworkInterfaces()


                              from props in nics.GetIPProperties().GatewayAddresses
                              where nics.OperationalStatus == OperationalStatus.Up
                              select props.Address.ToString(); // this sets the default gateway in a variable

                GatewayIPAddressInformationCollection prop = ni.GetIPProperties().GatewayAddresses;

                if (defGate.First() != null)
                {

                    IPInterfaceProperties ipProps = ni.GetIPProperties();

                    foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                    {

                        if (addr.Address.ToString().Contains(defGate.First().Remove(defGate.First().LastIndexOf(".")))) // The IP address of the computer is always a bit equal to the default gateway except for the last group of numbers. This splits it and checks if the ip without the last group matches the default gateway
                        {

                            if (result == "?") // check if the string has been changed before
                            {
                                result = addr.Address.ToString(); // put the ip address in a string that you can use.
                            }
                        }

                    }

                }

            }
            return result;
        }

        private void uiCloseTab_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
