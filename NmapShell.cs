using Newtonsoft.Json;
using NMap.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wcheck.Utils;
using wcheck.wcontrols;
using wcheck.wshell.Enums;
using wcheck.wshell.Objects;
using wshell.Abstract;
using wshell.Utils;

namespace tview.nmap.shellv1
{
    public class NmapShell : ShellBase
    {
        public MainPage Page { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        private CancellationTokenSource _cts { get; set; }

        public NmapShell() : base(new wshell.Objects.ShellInfo("Nmap API", "1.0.0", new Guid("4f1fc6ba-56b0-4844-8e2a-485578e8bc1f"), wshell.Enums.ShellType.TaskView, "Nmap API для работы с Nmap.exe и сканирования сети", "Артем И.С."))
        {
            Settings = ShellSettings.Load(ShellInfo.Id.ToString(), new List<SettingsObject>
            {
                new SettingsObject
                {
                    Name = "pNmapPath",
                    Value = @"C:\Program Files (x86)\Nmap\nmap.exe",
                    ViewAdditional = "Default C:\\Program Files (x86)\\Nmap\\nmap.exe",
                    ViewName = "Путь к утилите Nmap"
                },
                new SettingsObject
                {
                    Name = "pMainIp",
                    Value = @"192.168.0.1",
                    ViewAdditional = "Default: 192.168.0.1",
                    ViewName = "IP адрес шлюза"
                },
                new SettingsObject
                {
                    Name = "pSubnet",
                    Value = @"255.255.255.0",
                    ViewAdditional = "Default: 255.255.255.0",
                    ViewName = "Маска подсети"
                },
                new SettingsObject
                {
                    Name = "pChkOnlySelected",
                    Value = true,
                    ViewName = "Проверять порты только выбранных хостов"
                },
                //new SettingsObject
                //{
                //    Name = "pComboBox",
                //    Value = 0,
                //    ViewName = "Combo box parameter",
                //    ViewAdditional = "Item 1;Item 2;Item 3"
                //},
            });
        }

        public override Schema OnHostCallback(Schema schema)
        {
            switch (schema.Type)
            {
                case CallbackType.StartTaskView:
                    var hosts = this.InvokeCustomRequest("b4877dc5-a5b5-4b7e-b08b-1b1995e8c8d8", "type.gethosts").GetProviding<string>();
                    Page.StartTask(hosts, Settings.GetValue<string>("pMainIp"), Settings.GetValue<string>("pSubnet"));
                    break;
            }
            return new Schema(CallbackType.EmptyResponse);
        }

        public override void OnPause()
        {

        }

        public override void OnResume()
        {

        }

        public override void OnRun()
        {
            //registering page
            _cts = new CancellationTokenSource();
            CancellationToken = _cts.Token;
            Page = new MainPage(this);
            Callback.Invoke(this, new Schema(CallbackType.RegisterPage).SetProviding(Page));
        }

        public override void OnStop()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
            MainPage.Processor.CloseAll();
            Callback.Invoke(this, new Schema(CallbackType.UnregisterPage).SetProviding(Page));
            Page = null;
        }

        public override void OnSettingsEdit(SettingsObject obj, PropertyEventArgs propertyEventArgs)
        {
            var setting = Settings.Get(obj.Name);
            if (propertyEventArgs.Type == PropertyType.TextBox)
            {
                setting.Value = propertyEventArgs.Text;
            }
            else if (propertyEventArgs.Type == PropertyType.CheckBox)
            {
                setting.Value = propertyEventArgs.Checked;
            }
            else if (propertyEventArgs.Type == PropertyType.ComboBox)
            {
                setting.Value = propertyEventArgs.SelectedIndex;
            }
            Settings.Save();
            this.InvokeSettingsPropertyChanged("b4877dc5-a5b5-4b7e-b08b-1b1995e8c8d8");
        }
    }
}
