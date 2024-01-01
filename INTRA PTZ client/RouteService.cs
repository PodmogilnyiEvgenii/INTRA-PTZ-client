using System;
using System.Collections.Generic;
using System.Timers;
using Timer = System.Timers.Timer;

namespace INTRA_PTZ_client
{
    public class RouteService
    {
        private Device device;
        private Timer routeTimer;
        private List<Route.RouteTableRow> savedRouteList = new List<Route.RouteTableRow>();
        private Queue<Route.RouteTableRow> routeQueue = new Queue<Route.RouteTableRow>();
        private bool isTimerOnline = false;
        private int curentRouteStep = 0;
        private DateTime nextRouteStepTime;

        public Timer RouteTimer { get => routeTimer; set => routeTimer = value; }

        public RouteService(Device device)
        {
            this.device = device;
        }

        public string GetRouteStatusString()
        {
            if (!isTimerOnline) return "Маршрут: ВЫКЛ"; else
            {
                int sec = (int)(isTimerOnline ? (nextRouteStepTime - DateTime.Now).TotalSeconds : 0);
                string secString = sec > 0 ? sec.ToString() : "  ";

                return "Маршрут: ВКЛ   Шаг: " + curentRouteStep + "   Таймер: " + secString;
            } 
        }

        public bool GetIsTimerOn()
        {
            return isTimerOnline;
        }
        public void SetIsTimerOn(bool value)
        {
            isTimerOnline = value;

            if (value)
            {
                routeTimer.Start();
                //routeWindow.Title = "Маршрут (запущен)";
            }
            else
            {
                routeTimer.Stop();
                //routeWindow.Title = "Маршрут (остановлен)";
            }
        }

        public void SetRouteQueue(List<Route.RouteTableRow> list, int startIndex)
        {
            savedRouteList = list;
            routeQueue.Clear();

            for (int i = startIndex; i < list.Count; i++)
            {
                routeQueue.Enqueue(list[i]);
            }

            routeTimer = new Timer(AppOptions.ROUTE_START_TIMEOUT);
            routeTimer.AutoReset = true;
            routeTimer.Elapsed += new ElapsedEventHandler(OnRouteTimerEvent);
            nextRouteStepTime = DateTime.Now.AddMilliseconds(routeTimer.Interval);
        }

        private void OnRouteTimerEvent(Object source, ElapsedEventArgs e)
        {
            if (routeQueue.Count > 0)
            {

                routeTimer.Stop();
                Route.RouteTableRow routeStep = routeQueue.Dequeue();
                if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Route step " + routeStep.Count + " type = " + routeStep.OperationType + " timeout = " + routeStep.Timeout);


                switch (routeStep.OperationType)
                {
                    case 0:                                                                         //Калибровка
                        device.MainWindow.CalibratePlatform();
                        break;

                    case 1:                                                                         //Координаты
                        device.MainWindow.MoveToCoordinates(routeStep.Pan, routeStep.Tilt);
                        break;

                    case 2:                                                                         //Пресет
                        device.MainWindow.MoveToPreset((int)routeStep.Pan);
                        break;

                    case 3:                                                                         //В начало
                        SetRouteQueue(savedRouteList, 0);
                        SetIsTimerOn(true);
                        break;
                }

                if (routeQueue.Count > 0)
                {
                    if (routeStep.Timeout != 0)
                    {
                        routeTimer.Interval = routeStep.Timeout * 1000;
                    }
                    else
                    {
                        routeTimer.Interval = 100;
                    }
                    curentRouteStep = routeStep.Count;
                    nextRouteStepTime = DateTime.Now.AddMilliseconds(routeTimer.Interval);
                    routeTimer.Start();

                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("End route queue");
                    SetIsTimerOn(false);
                }
            }
        }
    }
}
