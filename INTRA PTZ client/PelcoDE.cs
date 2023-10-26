using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRA_PTZ_client
{
    public static class PelcoDE
    {
        public static Byte[] getCommand(Byte address, Byte k1, Byte k2, Byte dt1, Byte dt2)
        {
            Byte[] command = new Byte[7];

            command[0] = 0xff;
            command[1] = address;
            command[2] = k1;
            command[3] = k2;
            command[4] = dt1;
            command[5] = dt2;
            command[6] = (Byte)(command[1]+ command[2] + command[3] + command[4] + command[5]);


            System.Diagnostics.Trace.WriteLine(BitConverter.ToString(command));

            return  command;
        }

    }
}
