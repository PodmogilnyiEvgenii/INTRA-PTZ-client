using System.Collections.Generic;
using System.Linq;

namespace INTRA_PTZ_client
{
    public class UdpCommand
    {
        private byte[] request;
        private byte answer;
        private string answerString;
        private int timeout = 0; //in millis

        public byte[] Request { get => request; set => request = value; }
        public byte Answer { get => answer; set => answer = value; }
        public string AnswerString { get => answerString; set => answerString = value; }
        public int Timeout { get => timeout; set => timeout = value; }

        public UdpCommand(byte[] request, string answerString, int timeout)
        {
            this.request = request;
            this.answerString = answerString;
            this.answer = PelcoDE.getRequestByte(answerString);
            this.timeout = timeout;
        }

        public UdpCommand(byte[] request, string answerString)
        {
            this.request = request;
            this.answerString = answerString;
            this.answer = PelcoDE.getRequestByte(answerString);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            UdpCommand command = (UdpCommand)obj;


            return //EqualityComparer<byte[]>.Default.Equals(Request, command.Request) &&
                   Request.SequenceEqual(command.Request) &&
                   AnswerString.Equals(command.AnswerString) &&
                   Timeout == command.Timeout;
        }
    }
}
