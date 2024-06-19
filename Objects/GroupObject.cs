using DocumentFormat.OpenXml.Bibliography;
using NMap.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using tview.nmap.shellv1.Controls;
using wcheck.wcontrols;

namespace tview.nmap.shellv1.Objects
{
    public sealed class GroupObject
    {
        public string TargetGroup { get; set; }
        public List<HostObject> HostObjects { get; }
        public Expander Expander { get; }
        public StackPanel StackPanel { get; }

        public GroupObject(string targetGroup, bool prefix = true)
        {
            HostObjects = new List<HostObject>();
            TargetGroup = targetGroup;
            var headerText = prefix ? "Группа хостов '" + TargetGroup + "'" : TargetGroup;
            var header = new TextBlock
            {
                Foreground = "#1f1f1f".GetBrush(),
                FontFamily = new FontFamily("Arial"),
                Text = headerText,
            };
            Expander = new Expander
            {
                Header = header,
            };
            StackPanel = new StackPanel();
            Expander.Content = StackPanel;
        }

        public void TryAddHostObject(HostObject hostObject)
        {
            if(hostObject.TargetGroup == TargetGroup)
            {
                HostObjects.Add(hostObject);
                if(hostObject.IsUp)
                    StackPanel.Children.Add(new NmapControl(hostObject.Source)
                    {
                        Margin = new Thickness(5, 5, 5, 0)
                    });
                else
                    StackPanel.Children.Add(new NmapControl(hostObject.TargetIp)
                    {
                        Margin = new Thickness(5, 5, 5, 0)
                    });
            }
        }
        public void UnsafeAddHostObject(HostObject hostObject)
        {
            HostObjects.Add(hostObject);
            if (hostObject.IsUp)
                StackPanel.Children.Add(new NmapControl(hostObject.Source)
                {
                    Margin = new Thickness(5, 5, 5, 0)
                });
            else
                StackPanel.Children.Add(new NmapControl(hostObject.TargetIp)
                {
                    Margin = new Thickness(5, 5, 5, 0)
                });
        }
        public void TryAddHostObjectRange(IEnumerable<HostObject> hostObjects)
        {
            foreach (var obj in hostObjects)
                TryAddHostObject(obj);
        }

        public static IEnumerable<GroupObject> FromMapping(string[] netmap, NMapResult result)
        {
            List<GroupObject> output = new List<GroupObject>();

            List<HostObject> hosts = new List<HostObject>();

            //parse network mappings: selected hosts and groups
            foreach (var mapping in netmap)
            {
                var hostObject = GetHostObject(mapping, result);
                hosts.Add(hostObject);
                if(output.FirstOrDefault(x => x.TargetGroup == hostObject.TargetGroup) == null)
                    output.Add(new GroupObject(hostObject.TargetGroup));
            }

            //get sources
            foreach(var obj in hosts)
                TryGetSource(obj, result);

            //add hostsObjects
            foreach (var groupObject in output)
                groupObject.TryAddHostObjectRange(hosts);

            output.Add(GetNonSelectedGroup(hosts, result));

            return output;
        }
        private static HostObject GetHostObject(string mapping, NMapResult result)
        {
            var ip = mapping.Split(":")[0];
            var groupname = mapping.Split(":")[1];
            var output = new HostObject
            {
                TargetGroup = groupname,
                TargetIp = ip,
                IsUp = IsActiveCompare(ip, result) //compare active
            };
            TryGetSource(output, result);
            return output;
        }
        private static bool IsActiveCompare(string host, NMapResult result)
        {
            return result.Hosts.FirstOrDefault(x => x.Address.IP == host) == null ? false : true;
        }
        private static bool TryGetSource(HostObject hostObject, NMapResult result)
        {
            var source = result.Hosts.FirstOrDefault(x => x.Address.IP == hostObject.TargetIp);
            if (source == null)
                return false;
            else
            {
                hostObject.Source = source;
                return true;
            }
        }
        private static GroupObject GetNonSelectedGroup(IEnumerable<HostObject> selectedObjects, NMapResult result)
        {
            var output = new GroupObject("Без группы", false);
            foreach (var host in result.Hosts) 
            {
                if (selectedObjects.FirstOrDefault(x => x.TargetIp == host.Address.IP) == null)
                    output.UnsafeAddHostObject(new HostObject
                    {
                        Source = host,
                        IsUp = true,
                        TargetIp = host.Address.IP
                    });
            }
            return output;
        }
    }
}
