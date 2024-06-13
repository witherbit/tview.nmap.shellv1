using NMap.Schema;
using NMap.Schema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
using wcheck.wcontrols;

namespace tview.nmap.shellv1.Controls
{
    /// <summary>
    /// Логика взаимодействия для NmapControl.xaml
    /// </summary>
    public partial class NmapControl : UserControl
    {
        public NmapControl(Host host)
        {
            InitializeComponent();
            uiTextIp.Text = host.Address.IP;
            foreach(var portItem in host.Ports)
            {
                foreach(var port in portItem.Port)
                {
                    var service = "unknown";
                    if(port.Service != null && port.Service.Name != null)
                        service = port.Service.Name;
                    uiTextPorts.Text += $"{port.PortID}\t:\t{port.State.StateProperty}\t[{service}]\r\n";
                }
            }
            if (!string.IsNullOrEmpty(uiTextPorts.Text))
                uiTextPorts.Text = uiTextPorts.Text.Remove(uiTextPorts.Text.Length - 2);
            else
                uiTextPorts.Text = "Открытые порты не обнаружены";
            foreach (var os in host.OperatingSystems)
            {
                foreach(var match in os.Matches)
                {
                    uiTextSysInfo.Text += $"{match.Name}\r\n";
                }
            }
            if (!string.IsNullOrEmpty(uiTextSysInfo.Text))
                uiTextSysInfo.Text = uiTextSysInfo.Text.Remove(uiTextSysInfo.Text.Length - 2);
            else
                uiTextSysInfo.Text = "Операционные системы не обнаружены";
        }
        public NmapControl(string downTarget)
        {
            InitializeComponent();
            uiTextIp.Text = downTarget;
            uiTextPorts.Text = "Открытые порты не обнаружены";
            uiTextSysInfo.Text = "Операционные системы не обнаружены";
            uiBorderUp.Background = "#ff9999".GetBrush();
            uiBorderOs.Visibility = Visibility.Collapsed;
            uiBorderPorts.Visibility = Visibility.Collapsed;
        }
    }
}
