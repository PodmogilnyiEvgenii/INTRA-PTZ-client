﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace INTRA_PTZ_client
{
    public partial class RouteWindow : Window
    {
        private Device device;
        private List<Route.RouteTableRow> savedRouteList = new List<Route.RouteTableRow>();

        public RouteWindow(Device device)
        {
            InitializeComponent();
            this.device = device;
            this.Loaded += RouteWindow_Loaded;
            this.IsVisibleChanged += RouteWindow_IsVisibleChanged;
            this.Closing += RouteWindow_Closing;
        }

        private void RouteWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //routeTable.AutoGeneratingColumn += RouteTable_AutoGeneratedColumns;
        }
        private void RouteWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            deviceDataText.Text=device.GetStatusString();
            deviceCoordinatesText.Text=device.GetCoordinatesString();
            savedRouteList = device.Route.GetRouteList();
            routeTable.ItemsSource = device.Route.GetRouteList();            
        }
        private void RouteWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

       /* public void RouteTable_AutoGeneratedColumns(object s, DataGridAutoGeneratingColumnEventArgs e)
        {
            //routeTable.Columns[0].Header = "№";
        */

        private void TableMenuAddRowUp_Click(object sender, RoutedEventArgs e)
        {
            device.Route.AddrouteListUp(routeTable.SelectedIndex < 0 ? 0 : routeTable.SelectedIndex);
            routeTable.ItemsSource = device.Route.GetRouteList();
        }
        private void TableMenuAddRowDown_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine(routeTable.SelectedIndex);
            device.Route.AddrouteListDown(routeTable.SelectedIndex < 0 ? 0 : routeTable.SelectedIndex);
            routeTable.ItemsSource = device.Route.GetRouteList();
        }
        private void TableMenuDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            device.Route.RouteListDeleteRow(routeTable.SelectedIndex);
            routeTable.ItemsSource = device.Route.GetRouteList();
        }
        private void TableMenuDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            device.Route.RouteListDeleteAll();
            routeTable.ItemsSource = device.Route.GetRouteList();
        }
        private void TableMenuStartFromThisRow_Click(object sender, RoutedEventArgs e)
        {           
            if (!device.Route.RouteService.GetIsTimerOn())
            {
                device.Route.RouteService.SetRouteQueue(device.Route.GetRouteList(), routeTable.SelectedIndex);
                device.Route.RouteService.SetIsTimerOn(true);
                pauseRouteButton.Content = "Пауза";
            }
        }

        private void RouteSaveButton_Click(object sender, RoutedEventArgs e)
        {
            //device.Route.SetRouteList(device.Route.GetRouteList());
            /*
            List<Route.RouteTableRow> list = device.Route.GetRouteList();

            for (int i = 0; i < list.Count; i++)
            {
                System.Diagnostics.Trace.WriteLine(list[i]);
            } 
            */
            routeWindow.Visibility = Visibility.Hidden;
        }
        private void RouteCancelButton_Click(object sender, RoutedEventArgs e)
        {
            device.Route.SetRouteList(savedRouteList);
            routeWindow.Visibility = Visibility.Hidden;
        }

        private void Hyperlink_OpenWebConsole(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(/*e.Uri.AbsoluteUri*/"http://" + device.Ip) { UseShellExecute = true });
            e.Handled = true;
        }

        private void StartRouteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!device.Route.RouteService.GetIsTimerOn())
            {
                device.Route.RouteService.SetRouteQueue(device.Route.GetRouteList(), 0);
                device.Route.RouteService.SetIsTimerOn(true);
                pauseRouteButton.Content = "Пауза";
            }
        }
        private void PauseRouteButton_Click(object sender, RoutedEventArgs e)
        {
            if (device.Route.RouteService.GetIsTimerOn())
            {
                device.Route.RouteService.SetIsTimerOn(false);
                pauseRouteButton.Content = "Продолжить";
            }
            else
            {
                device.Route.RouteService.SetIsTimerOn(true);
                pauseRouteButton.Content = "Пауза";                
            }
        }
        private void StopRouteButton_Click(object sender, RoutedEventArgs e)
        {
            device.Route.RouteService.SetIsTimerOn(false);
            device.Route.RouteService.SetRouteQueue(new List<Route.RouteTableRow>(),0);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (System.Windows.Controls.ComboBox)sender;
            DataGridRow dataGridRow = FindParent<DataGridRow>(comboBox);            

            int index = dataGridRow.GetIndex();
            string selected = comboBox.SelectedItem as string;

            int type = (int)Enum.Parse(typeof(Route.RouteTableRow.OperationTypeEnum), selected.Replace(" ", "_"));
            DataGridCell cell = new DataGridCell();            

            switch (type)
            {
                case 0:     //Калибровка

                    device.Route.SetRouteTypeByIndex(index, type);

                    cell = routeTable.Columns[2].GetCellContent(dataGridRow).Parent as DataGridCell;  
                    cell.Background = Brushes.Black;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 0;                    

                    cell = routeTable.Columns[3].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Black;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 0;

                    cell = routeTable.Columns[4].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Transparent;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 0;
                    break;

                case 1:     //Координаты

                    device.Route.SetRouteTypeByIndex(index, type);

                    cell = routeTable.Columns[2].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Transparent;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 90;

                    cell = routeTable.Columns[3].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Transparent;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 45;

                    cell = routeTable.Columns[4].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Transparent;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 5;
                    break;

                case 2:     //Пресет

                    device.Route.SetRouteTypeByIndex(index, type);

                    cell = routeTable.Columns[2].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Transparent;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 1;

                    cell = routeTable.Columns[3].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Black;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = "";

                    cell = routeTable.Columns[4].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Transparent;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 5;
                    break;

                case 3:     //В начало

                    device.Route.SetRouteTypeByIndex(index, type);

                    cell = routeTable.Columns[2].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Black;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 0;

                    cell = routeTable.Columns[3].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Black;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 0;

                    cell = routeTable.Columns[4].GetCellContent(dataGridRow).Parent as DataGridCell;
                    cell.Background = Brushes.Black;
                    cell.Foreground = Brushes.Black;
                    //cell.Content = 0;
                    break;
            }
        }

        public static Parent FindParent<Parent>(DependencyObject child)
            where Parent : DependencyObject
        {
            DependencyObject parentObject = child;

            //We are not dealing with Visual, so either we need to fnd parent or
            //get Visual to get parent from Parent Heirarchy.
            while (!((parentObject is System.Windows.Media.Visual)
                    || (parentObject is System.Windows.Media.Media3D.Visual3D)))
            {
                if (parentObject is Parent || parentObject == null)
                {
                    return parentObject as Parent;
                }
                else
                {
                    parentObject = (parentObject as FrameworkContentElement).Parent;
                }
            }

            //We have not found parent yet , and we have now visual to work with.
            parentObject = VisualTreeHelper.GetParent(parentObject);

            //check if the parent matches the type we're looking for
            if (parentObject is Parent || parentObject == null)
            {
                return parentObject as Parent;
            }
            else
            {
                //use recursion to proceed with next level
                return FindParent<Parent>(parentObject);
            }
        }
    }
}
