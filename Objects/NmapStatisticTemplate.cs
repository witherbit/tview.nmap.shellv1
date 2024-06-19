using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wcheck.Statistic.Nodes;
using wcheck.Statistic.Styles;
using wcheck.Statistic.Templates;
using pwither.formatter;
using wcheck.Statistic.Items;

namespace tview.nmap.shellv1.Objects
{
    [BitSerializable]
    public class NmapStatisticTemplate : IStatisticTemplate
    {
        public List<IStatisticNode> Nodes { get; }

        public TextNodeStyle HeaderStyle => new TextNodeStyle
        {
            SpacingBetweenLines = 2,
            FontSize = 12,
            IsBold = true,
            WpfFontSize = 14,
            Aligment = wcheck.Statistic.Enums.TextAligment.Center
        };

        public string? Header => "Результаты сканирования сети";

        public bool UseBreakAfterTemplate => true;

        public NmapStatisticTemplate(IEnumerable<GroupObject> groupObjects)
        {
            Nodes = new List<IStatisticNode>
            {
                new TextStatisticNode(),
            };
            foreach(var g in groupObjects)
            {
                if (g.HostObjects.Count < 1)
                    continue;
                Nodes.Add(new TextStatisticNode($"Данные для группы хостов {g.TargetGroup}", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Right,
                    FontSize = 12,
                    WpfFontSize = 14,
                    IsItalic = true,
                    WpfMargin = new WpfThinkness(5, 0, 5, 5),
                    SpacingBetweenLines = 1.5
                }));
                int row = 1;
                var ceils = new List<CeilItem>();
                ceils.Add(new CeilItem("Хост", 0, 0, "#fca577", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold= true,
                }));
                ceils.Add(new CeilItem("Порты", 1, 0, "#fca577", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                ceils.Add(new CeilItem("ОС", 2, 0, "#fca577", new TextNodeStyle
                {
                    Aligment = wcheck.Statistic.Enums.TextAligment.Center,
                    FontSize = 12,
                    WpfFontSize = 14,
                    Foreground = "#1f1f1f",
                    IsBold = true,
                }));
                foreach (var h in g.HostObjects)
                {
                    var portsStr = string.Empty;
                    var osStr = string.Empty;
                    if (h.IsUp)
                    {
                        foreach (var portItem in h.Source.Ports)
                        {
                            foreach (var port in portItem.Port)
                            {
                                var service = "unknown";
                                if (port.Service != null && port.Service.Name != null)
                                    service = port.Service.Name;
                                portsStr += $"{port.PortID}:{port.State.StateProperty}[{service}]\r\n";
                            }
                        }
                        foreach (var os in h.Source.OperatingSystems)
                        {
                            foreach (var match in os.Matches)
                            {
                                osStr += $"{match.Name}\r\n";
                            }
                        }
                    }
                    var fill = h.IsUp ? "#97ff87" : "#ff8686";
                    ceils.Add(new CeilItem(h.TargetIp, 0, row, fill, new TextNodeStyle
                    {
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    ceils.Add(new CeilItem(portsStr, 1, row, new TextNodeStyle
                    {
                        FontSize = 12,
                        WpfFontSize = 14,
                        WpfFontFamily = "Cascadia Code",
                        FontFamily = "Cascadia Code"
                    }));
                    ceils.Add(new CeilItem(osStr, 2, row, new TextNodeStyle
                    {
                        FontSize = 12,
                        WpfFontSize = 14,
                    }));
                    row++;
                }
                Nodes.Add(new TableStatisticNode(ceils));
            }
        }
    }
}
