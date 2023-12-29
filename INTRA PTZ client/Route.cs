using System;
using System.Collections.Generic;

namespace INTRA_PTZ_client
{
    [Serializable]
    public class Route
    {
        [Serializable]
        public class RouteTableRow
        {
            public enum OperationTypeEnum
            {
                Калибровка,
                Координаты,
                Пресет
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

                OperationTypeList.Add(Enum.GetName(typeof(OperationTypeEnum), 0));
                OperationTypeList.Add(Enum.GetName(typeof(OperationTypeEnum), 1));
                OperationTypeList.Add(Enum.GetName(typeof(OperationTypeEnum), 2));
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
        [NonSerialized] private bool isStart = false;

        public List<RouteTableRow> RouteList { get => routeList; set => routeList = value; }

        public bool GetIsStart()
        {
            return isStart;
        }
        public void SetIsStart(bool value)
        {
            isStart = value;
        }

        public List<RouteTableRow> GetRouteList()
        {
            return RouteList;
        }
        public void SetRouteList(List<RouteTableRow> routeList)
        {
            this.RouteList = routeList;
        }

        public Route()
        {/*
            if (RouteList.Count == 0)
            {
                RouteList.Add(new RouteTableRow(1, 0, 0, 0, 60));
                RouteList.Add(new RouteTableRow(2, 1, 0, 0, 60));
                RouteList.Add(new RouteTableRow(3, 2, 0, 0, 60));
                RouteList.Add(new RouteTableRow(4, 0, 0, 0, 60));
            }
            */
        }

        public void addrouteListUp(int rowNumber)
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < RouteList.Count; i++)
            {
                if (i == rowNumber) newRouteList.Add(new RouteTableRow(i, 2, 0, 0, 0));
                newRouteList.Add(RouteList[i]);
            }
            if (RouteList.Count == rowNumber) newRouteList.Add(new RouteTableRow(rowNumber, 2, 0, 0, 0));

            SetRouteList(setRightOrder(newRouteList));
        }
        public void addrouteListDown(int rowNumber)
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < RouteList.Count; i++)
            {
                if (i == rowNumber + 1) newRouteList.Add(new RouteTableRow(i, 2, 0, 0, 0));
                newRouteList.Add(RouteList[i]);
            }
            if (RouteList.Count == rowNumber || RouteList.Count - 1 == rowNumber) newRouteList.Add(new RouteTableRow(rowNumber, 2, 0, 0, 0));

            SetRouteList(setRightOrder(newRouteList));
        }
        public void routeListDeleteRow(int rowNumber)
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < RouteList.Count; i++)
            {
                if (i != rowNumber) newRouteList.Add(RouteList[i]);
            }

            SetRouteList(setRightOrder(newRouteList));
        }
        public void routeListDeleteAll()
        {
            SetRouteList(new List<RouteTableRow>());
        }
        private List<RouteTableRow> setRightOrder(List<RouteTableRow> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Count = i + 1;
            }

            return list;
        }
        public void setRouteTypeByIndex(int index, int type)
        {
            RouteList[index].OperationType = type;
        }

    }
}
