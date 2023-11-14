using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
