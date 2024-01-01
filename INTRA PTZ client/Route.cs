using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace INTRA_PTZ_client
{
    [Serializable]
    public class Route
    {
        [JsonIgnore] private RouteService routeService;

        [Serializable]
        public class RouteTableRow
        {
            public enum OperationTypeEnum
            {
                Калибровка,
                Координаты,
                Пресет,
                В_начало
            }

            private List<string> operationTypeList = new List<string>();
            private int count = 0;
            private int operationType = 0;
            private float pan = 0;
            private float tilt = 0;
            //private float focus = 0;
            //private float zoom = 0;
            private double timeout = 0; //в сек

            public RouteTableRow(int count, int operationType, float pan, float tilt, double timeout)
            {
                this.count = count;
                this.operationType = operationType;
                this.pan = pan;
                this.tilt = tilt;
                this.timeout = timeout;

                for (int i = 0; i < Enum.GetValues(typeof(OperationTypeEnum)).Length; i++)
                {
                    operationTypeList.Add(Enum.GetName(typeof(OperationTypeEnum), i).Replace("_", " "));
                }

            }

            public int Count { get => count; set => count = value; }
            public int OperationType { get => operationType; set => operationType = value; }
            public float Pan { get => pan; set => pan = value; }
            public float Tilt { get => tilt; set => tilt = value; }
            public double Timeout { get => timeout; set => timeout = value; }
            public List<string> OperationTypeList { get => operationTypeList; set => operationTypeList = value; }

            public override string ToString()
            {
                return "count = " + count + " operationType = " + operationType + " pan = " + pan + " tilt = " + tilt + " timeout = " + timeout;
            }
        }

        private List<RouteTableRow> routeList = new List<RouteTableRow>();
        public List<RouteTableRow> RouteList { get => routeList; set => routeList = value; }
        public RouteService RouteService { get => routeService; set => routeService = value; }

        public List<RouteTableRow> GetRouteList()
        {
            return RouteList;
        }
        public void SetRouteList(List<RouteTableRow> routeList)
        {
            this.RouteList = routeList;
        }

        public Route(Device device)
        {
            this.routeService = new RouteService(device);
        }

        public void AddrouteListUp(int rowNumber)
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < RouteList.Count; i++)
            {
                if (i == rowNumber) newRouteList.Add(new RouteTableRow(i, 2, 0, 0, 0));
                newRouteList.Add(RouteList[i]);
            }
            if (RouteList.Count == rowNumber) newRouteList.Add(new RouteTableRow(rowNumber, 2, 0, 0, 0));

            SetRouteList(SetRightOrder(newRouteList));
        }
        public void AddrouteListDown(int rowNumber)
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < RouteList.Count; i++)
            {
                if (i == rowNumber + 1) newRouteList.Add(new RouteTableRow(i, 2, 0, 0, 0));
                newRouteList.Add(RouteList[i]);
            }
            if (RouteList.Count == rowNumber || RouteList.Count - 1 == rowNumber) newRouteList.Add(new RouteTableRow(rowNumber, 2, 0, 0, 0));

            SetRouteList(SetRightOrder(newRouteList));
        }
        public void RouteListDeleteRow(int rowNumber)
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < RouteList.Count; i++)
            {
                if (i != rowNumber) newRouteList.Add(RouteList[i]);
            }

            SetRouteList(SetRightOrder(newRouteList));
        }
        public void RouteListDeleteAll()
        {
            SetRouteList(new List<RouteTableRow>());
        }
        private List<RouteTableRow> SetRightOrder(List<RouteTableRow> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Count = i + 1;
            }

            return list;
        }
        public void SetRouteTypeByIndex(int index, int type)
        {
            RouteList[index].OperationType = type;
        }

    }
}
