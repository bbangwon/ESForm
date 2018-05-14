using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ESForm
{
    public class ESPorts : ESLogBase
    {
        SerialPort serialPort = null;
        public Action<string> onRecvData = null;

        public ESPorts(string portName, int baudRate, string parity, int dataBit, float stopBits)
        {
            Parity p = Parity.None;
            StopBits s = StopBits.One;

            switch (parity)
            {
                case "None":
                    p = Parity.None;
                    break;
                case "Even":
                    p = Parity.Even;
                    break;
                case "Mark":
                    p = Parity.Mark;
                    break;
                case "Odd":
                    p = Parity.Odd;
                    break;
                case "Space":
                    p = Parity.Space;
                    break;
            }

            switch (stopBits)
            {
                case 0f:
                    s = StopBits.None;
                    break;
                case 1f:
                    s = StopBits.One;
                    break;
                case 1.5f:
                    s = StopBits.OnePointFive;
                    break;
                case 2f:
                    s = StopBits.Two;
                    break;
            }


            serialPort = new SerialPort(portName, baudRate, p, dataBit, s);
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        public ESPorts(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBit = 8, StopBits stopBits = StopBits.One)
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBit, stopBits);
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            string data = port.ReadExisting();

            Log(string.Format("시리얼포트 받은 데이터 [{0}] : {1}", data.Length, data));
            if (onRecvData != null)
                onRecvData(data);

        }

        public void Close()
        {
            serialPort.Close();
        }

        public void SendData(string message)
        {
            if(serialPort.IsOpen)
            {
                serialPort.Write(message);
                Log(string.Format("시리얼포트 보낸 데이터 [{0}] : {1}", message.Length, message));
            }
        }
    }
}
