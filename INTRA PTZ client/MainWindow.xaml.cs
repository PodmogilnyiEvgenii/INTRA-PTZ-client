using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using static INTRA_PTZ_client.Preset;
using static INTRA_PTZ_client.Route;

namespace INTRA_PTZ_client
{
    public partial class MainWindow : Window
    {
        private OptionsWindow optionsWindow;
        private RouteWindow routeWindow;
        private ServiceWindow serviceWindow;
        private PresetWindow presetWindow;
        private Device device;

        public MainWindow()
        {
            InitializeComponent();

            this.device = new Device(this);

            this.Closing += MainWindow_Closing;

            this.optionsWindow = new OptionsWindow(this);
            this.routeWindow = new RouteWindow(device);
            this.serviceWindow = new ServiceWindow(device);
            this.presetWindow = new PresetWindow(device);

            deviceDataText.Text = device.GetStatusString();
            deviceCoordinatesText.Text = device.GetCoordinatesString();
            zoomField.IsEnabled = false;
            focusField.IsEnabled = false;

            DispatcherTimer refreshTimer = new DispatcherTimer();
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Interval = TimeSpan.FromMilliseconds(AppOptions.REFRESH_STATUS);
            refreshTimer.Start();

            //Device.Udp.getFirstData();            

            List<Route.RouteTableRow> routeList = new List<Route.RouteTableRow>();
            routeList.Add(new RouteTableRow(1, 0, 0, 0, 10));
            routeList.Add(new RouteTableRow(2, 1, 0, 0, 10));
            routeList.Add(new RouteTableRow(3, 2, 0, 0, 10));
            routeList.Add(new RouteTableRow(4, 3, 0, 0, 10));
            Device.Route.SetRouteList(routeList);

            List<Preset.PresetTableRow> presetList = new List<Preset.PresetTableRow>();
            for (int i = 1; i <= 20; i++)
            {
                presetList.Add(new PresetTableRow(i, 0, 0));
            }
            Device.Preset.SetPresetList(presetList);

            updateTooltips();
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            String statusString = device.GetStatusString();
            String coordinatesString = device.GetCoordinatesString();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            deviceDataText.Text = statusString));
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            deviceCoordinatesText.Text = coordinatesString));

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            serviceWindow.deviceDataText.Text = statusString));
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            serviceWindow.deviceCoordinatesText.Text = coordinatesString));

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            routeWindow.deviceDataText.Text = statusString));
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            routeWindow.deviceCoordinatesText.Text = coordinatesString));

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    routeWindow.Title = device.Route.RouteService.GetRouteStatusString()));
        }

        public OptionsWindow OptionsWindow { get => optionsWindow; set => optionsWindow = value; }
        public RouteWindow RouteWindow { get => routeWindow; set => routeWindow = value; }
        public ServiceWindow ServiceWindow { get => serviceWindow; set => serviceWindow = value; }
        public PresetWindow PresetWindow { get => presetWindow; set => presetWindow = value; }
        public Device Device { get => device; set => device = value; }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            device.Udp.Disconnect();
        }

        //menu

        private void ConfigurationLoad_Click(object sender, RoutedEventArgs e)
        {
            //menu

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ECI cfg files (*.cfg)|*.cfg";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                using (TextReader textReader = new StreamReader(fileName))
                {
                    try
                    {
                        Device device = JsonConvert.DeserializeObject<Device>(textReader.ReadToEnd());
                        device.MainWindow = this;
                        device.Udp = new UDP(this, device);
                        this.optionsWindow = new OptionsWindow(this);
                        this.routeWindow = new RouteWindow(device);
                        this.serviceWindow = new ServiceWindow(device);
                        this.presetWindow = new PresetWindow(device);

                        this.device = device;
                        //MessageBox.Show("Конфигурация загружена!");
                    }
                    catch (IOException ex)
                    {
                        System.Diagnostics.Trace.WriteLine("Configuration load failed: " + ex.Message);
                    }
                }
            }




        }
        private void ConfigurationSave_Click(object sender, RoutedEventArgs e)
        {
            //menu
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "INTRA PTZ client cfg files (*.cfg) |*.cfg";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;

                using (TextWriter textWriter = new StreamWriter(fileName))
                {
                    string data = JsonConvert.SerializeObject(device);

                    try
                    {
                        textWriter.Write(data);
                        //MessageBox.Show("Конфигурация сохранена!");


                    }
                    catch (IOException ex)
                    {
                        System.Diagnostics.Trace.WriteLine("Configuration save failed: " + ex.Message);
                    }

                }
            }


        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            //menu
            MessageBox.Show("Здесь могла бы быть ваша реклама!");
            //System.Windows.Application.Current.Shutdown();
        }

        //panel 1

        private void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            PresetWindow.Visibility = Visibility.Visible;
            PresetWindow.Owner = this;
            PresetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PresetWindow.ShowDialog();
        }
        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow.Visibility = Visibility.Visible;
            OptionsWindow.Owner = this;
            OptionsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            OptionsWindow.ShowDialog();
        }
        private void RouteButton_Click(object sender, RoutedEventArgs e)
        {
            RouteWindow.Visibility = Visibility.Visible;
            RouteWindow.Owner = this;
            RouteWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            RouteWindow.ShowDialog();
        }
        private void ServiceButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceWindow.Visibility = Visibility.Visible;
            ServiceWindow.Owner = this;
            ServiceWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ServiceWindow.ShowDialog();
        }

        //manual mode   

        public void MoveToCoordinates(float panValue, float tiltValue)
        {
            if (panValue > device.MaxPan || panValue < device.MinPan) panValue = device.GetCurrentPan();
            if (tiltValue > device.MaxTilt || tiltValue < device.MinTilt) tiltValue = device.GetCurrentTilt();

            byte[] pan = BitConverter.GetBytes(Device.PanAngleToStep(panValue.ToString()));
            byte[] tilt = BitConverter.GetBytes(Device.TiltAngleToStep(tiltValue.ToString()));

            //byte[] zoom = BitConverter.GetBytes(Device.panAngleToStep(zoomField.Text));
            //byte[] focus = BitConverter.GetBytes(Device.panAngleToStep(focusField.Text));

            if (AppOptions.DEBUG && device.CurrentStepPan == Device.PanAngleToStep(panValue.ToString()) && device.CurrentStepTilt == Device.TiltAngleToStep(tiltValue.ToString()))
            {
                System.Diagnostics.Trace.WriteLine("Same coordinates | pan= " + Device.PanAngleToStep(panValue.ToString()) + " tilt= " + Device.TiltAngleToStep(tiltValue.ToString()));
            }

            List<UdpCommand> list = new List<UdpCommand>();
            if (device.CurrentStepPan != Device.PanAngleToStep(panValue.ToString()))
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), pan[1], pan[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
            }

            if (device.CurrentStepTilt != Device.TiltAngleToStep(tiltValue.ToString()))
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), tilt[1], tilt[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
            }

            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setZoom"), tilt[1], tilt[0]), "Zoom", AppOptions.UDP_TIMEOUT_SHORT));
            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setFocus"), tilt[1], tilt[0]), "Focus", AppOptions.UDP_TIMEOUT_SHORT));

            Device.Udp.UdpServices.AddTaskToEnd(list);
        }
        private void SetCoordinatesButton_Click(object sender, RoutedEventArgs e)
        {
            float panValue = device.GetCurrentPan();
            float tiltValue = device.GetCurrentTilt();

            try { panValue = float.Parse(panField.Text); }
            catch { if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Pan parse error"); }

            if (panValue > device.MaxPan || panValue < device.MinPan) panField.Text = device.GetCurrentPan().ToString();

            try { tiltValue = float.Parse(tiltField.Text); }
            catch { if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Tilt parse error"); }

            if (tiltValue > device.MaxTilt || tiltValue < 0) tiltField.Text = device.GetCurrentTilt().ToString();

            MoveToCoordinates(panValue, tiltValue);
        }
        private void ManualControlUI(double speed, int dPan, int dTilt)
        {
            List<UdpCommand> list = new List<UdpCommand>();
            if (dPan != 0)
            {
                int resultPan = Device.CurrentStepPan + dPan * AppOptions.SPEED_STEP_PAN[(int)speed];
                if (resultPan < 0) resultPan += Device.GetMaxStepPan();
                if (resultPan > Device.GetMaxStepPan()) resultPan -= Device.GetMaxStepPan();

                byte[] pan = BitConverter.GetBytes(resultPan);

                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), pan[1], pan[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
            }

            if (dTilt != 0)
            {
                int resultTilt = Device.CurrentStepTilt + dTilt * AppOptions.SPEED_STEP_TILT[(int)speed];
                if (resultTilt < 0) resultTilt += Device.GetMaxStepTilt();
                if (resultTilt > Device.GetMaxStepTilt()) resultTilt -= Device.GetMaxStepTilt();

                byte[] tilt = BitConverter.GetBytes(resultTilt);

                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), tilt[1], tilt[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
            }
            Device.Udp.UdpServices.AddTaskToEnd(list);
        }

        public void CalibratePlatform()
        {
            List<UdpCommand> list = new List<UdpCommand>();

            if (device.CurrentStepPan != 0)
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));

            if (device.CurrentStepTilt != 0)
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));

            list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("makeTest"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));
            Device.Udp.UdpServices.AddTaskToEnd(list);
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, 1);
        }
        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 0, 1);
        }
        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, 1);
        }
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, 0);
        }
        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            CalibratePlatform();
        }
        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, 0);
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, -1);
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 0, -1);
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, -1);
        }

        private void SpeedPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (speedSleder.Value < 7) speedSleder.Value = speedSleder.Value + 1;
        }
        private void SpeedMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (speedSleder.Value > 0) speedSleder.Value = speedSleder.Value - 1;
        }

        private void ValidationPanField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            /*
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);*/

            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void ValidationTiltField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[-0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void ValidationZoomField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void ValidationFocusField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;

        }

        //status bar
        private void Hyperlink_OpenWebConsole(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(/*e.Uri.AbsoluteUri*/"http://" + device.Ip) { UseShellExecute = true });
            e.Handled = true;
        }

        //preset

        public void MoveToPreset(int presetNumber)
        {
            float panValue = Device.Preset.GetPresetList()[presetNumber].Pan;
            float tiltValue = Device.Preset.GetPresetList()[presetNumber].Tilt;

            if (panValue > device.MaxPan || panValue < device.MinPan) Device.Preset.GetPresetList()[presetNumber].Pan = Device.GetCurrentPan();
            if (tiltValue > device.MaxTilt || tiltValue < 0) Device.Preset.GetPresetList()[presetNumber].Tilt = Device.GetCurrentTilt();

            byte[] pan = BitConverter.GetBytes(Device.Preset.GetPresetList()[presetNumber].Pan);
            byte[] tilt = BitConverter.GetBytes(Device.Preset.GetPresetList()[presetNumber].Tilt);

            //byte[] zoom = BitConverter.GetBytes(Device.panAngleToStep(zoomField.Text));
            //byte[] focus = BitConverter.GetBytes(Device.panAngleToStep(focusField.Text));


            if (AppOptions.DEBUG && Device.Preset.GetPresetList()[presetNumber].Pan == Device.GetCurrentPan() && Device.Preset.GetPresetList()[presetNumber].Tilt == Device.GetCurrentTilt())
            {
                System.Diagnostics.Trace.WriteLine("Same coordinates | pan= " +
                Device.PanAngleToStep(Device.Preset.GetPresetList()[presetNumber].Pan.ToString())
                + " tilt= " +
                Device.TiltAngleToStep(Device.Preset.GetPresetList()[presetNumber].Tilt.ToString())
                );
            }

            List<UdpCommand> list = new List<UdpCommand>();
            if (Device.Preset.GetPresetList()[presetNumber].Pan != Device.GetCurrentPan())
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), pan[1], pan[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
            }

            if (Device.Preset.GetPresetList()[presetNumber].Tilt == Device.GetCurrentTilt())
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), tilt[1], tilt[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
            }

            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setZoom"), tilt[1], tilt[0]), "Zoom", AppOptions.UDP_TIMEOUT_SHORT));
            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setFocus"), tilt[1], tilt[0]), "Focus", AppOptions.UDP_TIMEOUT_SHORT));

            Device.Udp.UdpServices.AddTaskToEnd(list);
        }

        public void updateTooltips()
        {
            List<Button> list = new List<Button>();
            list.Add(preset1);
            list.Add(preset2);
            list.Add(preset3);
            list.Add(preset4);
            list.Add(preset5);
            list.Add(preset6);
            list.Add(preset7);
            list.Add(preset8);
            list.Add(preset9);
            list.Add(preset10);
            list.Add(preset11);
            list.Add(preset12);
            list.Add(preset13);
            list.Add(preset14);
            list.Add(preset15);
            list.Add(preset16);
            list.Add(preset17);
            list.Add(preset18);
            list.Add(preset19);
            list.Add(preset20);

            for (int i = 0; i < list.Count; i++)
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Content = Device.Preset.getTooltipDesc(i);
                list[i].ToolTip = toolTip;
            }
        }

        private void Preset1_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(0);
        }
        private void Preset2_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(1);
        }
        private void Preset3_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(2);
        }
        private void Preset4_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(3);
        }
        private void Preset5_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(4);
        }
        private void Preset6_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(5);
        }
        private void Preset7_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(6);
        }
        private void Preset8_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(7);
        }
        private void Preset9_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(8);
        }
        private void Preset10_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(9);
        }

        private void Preset11_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(10);
        }
        private void Preset12_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(11);
        }
        private void Preset13_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(12);
        }
        private void Preset14_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(13);
        }
        private void Preset15_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(14);
        }
        private void Preset16_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(15);
        }
        private void Preset17_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(16);
        }
        private void Preset18_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(17);
        }
        private void Preset19_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(18);
        }
        private void Preset20_Click(object sender, RoutedEventArgs e)
        {
            MoveToPreset(19);
        }
    }
}
