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
                    uiTextPorts.Text += $"{port.PortID} : {port.State.StateProperty}\r\n";
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
    }
}
