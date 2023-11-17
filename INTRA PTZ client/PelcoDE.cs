using System;
using System.Collections.Generic;

namespace INTRA_PTZ_client
{
    public static class PelcoDE
    {
        private static IDictionary<string, byte> descriptionCommands = new Dictionary<string, Byte>();
        private static IDictionary<byte, string> descriptionRequests = new Dictionary<byte, string>();


        static PelcoDE()
        {
            descriptionCommands.Add("getPan", 0x51);
            descriptionCommands.Add("getTilt", 0x53);
            descriptionCommands.Add("getMaxStepPan", 0x55);
            descriptionCommands.Add("getMaxStepTilt", 0x57);
            descriptionCommands.Add("getZoom", 0x59);
            descriptionCommands.Add("getFocus", 0x5B);
            descriptionCommands.Add("getMaxStepZoom", 0x5D);
            descriptionCommands.Add("getMaxStepFocus", 0x5F);

            descriptionCommands.Add("getTemperature", 0x91);
            descriptionCommands.Add("getVoltage", 0x9B);

            descriptionCommands.Add("getRegister", 0x9F);
            descriptionCommands.Add("setRegister", 0x9D);



            //descriptionCommands.Add("getAllCoordinates", 0x79);
            descriptionCommands.Add("getAllMaxStepCoordinates", 0x7B);

            descriptionCommands.Add("setPan", 0x71);
            descriptionCommands.Add("setTilt", 0x73);
            descriptionCommands.Add("setZoom", 0x75);
            descriptionCommands.Add("setFocus", 0x77);


            descriptionRequests.Add(0x61, "Pan");
            descriptionRequests.Add(0x63, "Tilt");
            descriptionRequests.Add(0x65, "MaxStepPan");
            descriptionRequests.Add(0x67, "MaxStepTilt");
            descriptionRequests.Add(0x69, "Zoom");
            descriptionRequests.Add(0x6B, "Focus");
            descriptionRequests.Add(0x6D, "MaxStepZoom");
            descriptionRequests.Add(0x6F, "MaxStepFocus");
            descriptionRequests.Add(0x7C, "Done");

            descriptionRequests.Add(0xA1, "Temperature");
            descriptionRequests.Add(0xAB, "Voltage");

            descriptionRequests.Add(0xAF, "Register");
        }

        public static byte getByteCommand(string command)
        {
            return descriptionCommands[command];
        }

        public static string getDescriptionRequest(byte request)
        {
            return descriptionRequests.ContainsKey(request) ? descriptionRequests[request] : "Unknown key";
        }

        public static byte getRequestByte(string request)
        {
            foreach (var descriptionRequest in descriptionRequests)
            {
                if (descriptionRequest.Value.Equals(request)) return descriptionRequest.Key;
            }

            return new byte();
        }

        public static byte[] getCommand(int address, byte k1, byte k2, byte dt1, byte dt2)
        {
            byte[] command = new byte[7];

            command[0] = 0xff;
            command[1] = (byte)address;
            command[2] = k1;
            command[3] = k2;
            command[4] = dt1;
            command[5] = dt2;
            command[6] = (byte)(command[1] + command[2] + command[3] + command[4] + command[5]);

            //System.Diagnostics.Trace.WriteLine(BitConverter.ToString(command));

            return command;
        }



    }
}
