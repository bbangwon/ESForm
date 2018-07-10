using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ESForm
{
    public class ESClient : ESLogBase
    {
        public class AsyncObject
        {
            public byte[] buffer;
            public NetworkStream networkStream;
            public AsyncObject(int bufferSize)
            {
                buffer = new byte[bufferSize];
            }

            public AsyncObject(string Message)
            {
                buffer = Encoding.UTF8.GetBytes(Message);
            }
        }

        bool connected;
        TcpClient clientSocket;
        AsyncCallback receiveHandler;
        AsyncCallback sendHandler;

        public Action<string> onRecvMsg = null;

        public ESClient()
        {
            receiveHandler = new AsyncCallback(HandleDataReceive);
            sendHandler = new AsyncCallback(HandleDataSend);
        }
        public bool Connected
        {
            get => connected;
        }

        public bool ConnectToServer(string host, int port)
        {
            bool isConnected = false;
            try
            {
                clientSocket = new TcpClient(host, port);
                isConnected = true;                
            }
            catch(Exception ex)
            {
                isConnected = false;
                Log(string.Format("TCP 연결중 오류 발생 : {0}", ex.Message));
                return false;
            }

            connected = isConnected;
            if (isConnected)
            {
                
                AsyncObject ao = new AsyncObject(4096);
                ao.networkStream = clientSocket.GetStream();
                ao.networkStream.BeginRead(ao.buffer, 0, ao.buffer.Length, receiveHandler, ao);
                Log(string.Format("TCP 서버 접속 성공 {0}:{1}", host, port));
                return true;
            }
            else
            {
                Log(string.Format("TCP 서버 접속 실패 {0}:{1}", host, port));
            }
            return false;
        }

        public void StopClient()
        {
            clientSocket.Close();
        }

        public void SendMessage(string message)
        {
            AsyncObject ao = new AsyncObject(message);
            ao.networkStream = clientSocket.GetStream();
            try
            {
                ao.networkStream.BeginWrite(ao.buffer, 0, ao.buffer.Length, sendHandler, ao);
            }
            catch(Exception ex)
            {
                Log(string.Format("TCP 전송중 오류발생! {0}", ex.Message));
            }
        }

        void HandleDataReceive(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;
            int recvBytes;
            try
            {
                recvBytes = ao.networkStream.EndRead(ar);
            }catch
            {
                return;
            }

            if(recvBytes > 0)
            {
                byte[] msgByte = new byte[recvBytes];
                Array.Copy(ao.buffer, msgByte, recvBytes);
                string strRecvMsg = Encoding.UTF8.GetString(msgByte);

                Log(string.Format("TCP로 메시지 받음: {0}", strRecvMsg));

                if (onRecvMsg != null)
                    onRecvMsg(strRecvMsg);
            }

            try
            {
                ao.networkStream.BeginRead(ao.buffer, 0, ao.buffer.Length, receiveHandler, ao);
            }
            catch(Exception ex)
            {
                Log(string.Format("TCP 자료 수신 대기 도중 오류 발생! 메시지: {0}", ex.Message));
                return;
            }
        }

        void HandleDataSend(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;            

            try
            {
                ao.networkStream.EndWrite(ar);
            }
            catch(Exception ex)
            {
                Log(string.Format("TCP 자료 송신 도중 오류 발생! 메시지: {0}", ex.Message));
            }

            int sentBytes = ao.buffer.Length;
            if (sentBytes > 0)
            {
                byte[] msgByte = new byte[sentBytes];
                Array.Copy(ao.buffer, msgByte, sentBytes);

                Log(string.Format("TCP로 메세지 보냄: {0}", Encoding.UTF8.GetString(msgByte)));
            }
        }


    }
}
