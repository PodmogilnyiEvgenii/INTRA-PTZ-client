using System;
using System.Collections.Generic;

namespace INTRA_PTZ_client
{
    public class Route
    {
        public class RouteTableRow
        {
            private int count = 0;
            private string operationType = ""; //move, moveToPreset, calibrate
            private float pan = 0;
            private float tilt = 0;
            //private float focus = 0;
            //private float zoom = 0;
            private double timeout = 0; //в сек

            public RouteTableRow(int count, string operationType, float pan, float tilt, double timeout)
            {
                this.count = count;
                this.operationType = operationType;
                this.pan = pan;
                this.tilt = tilt;
                this.timeout = timeout;
            }
            public int Count { get => count; set => count = value; }
            public string OperationType { get => operationType; set => operationType = value; }
            public float Pan { get => pan; set => pan = value; }
            public float Tilt { get => tilt; set => tilt = value; }
            public double Timeout { get => timeout; set => timeout = value; }


        }

        private List<RouteTableRow> routeList = new List<RouteTableRow>();

        public List<RouteTableRow> GetRouteList()
        {
            return routeList;
        }

        public void SetRouteList(List<RouteTableRow> value)
        {
            routeList = value;
        }

        public Route()
        {
            GetRouteList().Add(new RouteTableRow(0, "move", 0, 0, 60));
            GetRouteList().Add(new RouteTableRow(1, "move", 0, 0, 60));
            GetRouteList().Add(new RouteTableRow(2, "move", 0, 0, 60));
            GetRouteList().Add(new RouteTableRow(3, "move", 0, 0, 60));
            /*routeList.Add(new RouteTableRow(4, "move", 0, 0, 60));
            routeList.Add(new RouteTableRow(5, "move", 0, 0, 60));
            routeList.Add(new RouteTableRow(6, "move", 0, 0, 60));
            routeList.Add(new RouteTableRow(7, "move", 0, 0, 60));
            routeList.Add(new RouteTableRow(8, "move", 0, 0, 60));
            routeList.Add(new RouteTableRow(9, "move", 0, 0, 60));
            routeList.Add(new RouteTableRow(10, "move", 0, 0, 60));*/
        }

        public void addrouteListUp(int rowNumber)           //TODO
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < GetRouteList().Count; i++)
            {
                if (i == rowNumber) newRouteList.Add(new RouteTableRow(i, "move", 0, 0, 0));
                newRouteList.Add(GetRouteList()[i]);
            }
            if (GetRouteList().Count == rowNumber) newRouteList.Add(new RouteTableRow(rowNumber, "move", 0, 0, 0));
            SetRouteList(newRouteList);
        }

        public void addrouteListDown(int rowNumber)         //TODO
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < GetRouteList().Count; i++)
            {
                if (i == rowNumber + 1) newRouteList.Add(new RouteTableRow(i, "move", 0, 0, 0));
                newRouteList.Add(GetRouteList()[i]);
            }
            if (GetRouteList().Count == rowNumber || GetRouteList().Count-1 == rowNumber) newRouteList.Add(new RouteTableRow(rowNumber, "move", 0, 0, 0));
            SetRouteList(newRouteList);
        }

        public void routeListDeleteRow(int rowNumber)
        {
            List<RouteTableRow> newRouteList = new List<RouteTableRow>();
            for (int i = 0; i < GetRouteList().Count; i++)
            {
                if (i != rowNumber) newRouteList.Add(GetRouteList()[i]);
            }
            SetRouteList(newRouteList);
        }

        public void routeListDeleteAll()
        {
            SetRouteList(new List<RouteTableRow>());
        }

    }
}
