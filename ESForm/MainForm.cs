using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ESForm
{
    public partial class MainForm : Form
    {

        ProgramSetting programSetting = new ProgramSetting();
        ESClient esClient = new ESClient();
        ESPorts ports = null;
        StreamWriter logWriter = null;

        public MainForm()
        {
            InitializeComponent();
            logWriter = File.AppendText(string.Format(@"C:\EM_HW\HWLog_{0}.txt", DateTime.Now.ToString("yyyyMMdd")));
            ProgramSetting.onLog = Log;
            ESPorts.onLog = Log;
            ESClient.onLog = Log;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tbSendMsg.Enabled = false;
            btnSend.Enabled = false;
            radioTCP.Enabled = false;
            radioSerial.Enabled = false;
            

            Log("프로그램 시작");
            
            //세팅 정보 초기화
            bool init = programSetting.Initialize();
            if(init)
            {
                try
                {
                    ports = new ESPorts(programSetting.settings.SerialPortName, programSetting.settings.BaudRate,
                        programSetting.settings.Parity, programSetting.settings.DataBit, programSetting.settings.StopBits);

                    ports.onRecvData = OnSerialMsgRecv;
                    radioSerial.Enabled = true;
                }
                catch(Exception ex)
                {
                    Log(ex.Message);
                }

                esClient.onRecvMsg = OnTCPMsgRecv;
                bool isConnected = esClient.ConnectToServer(programSetting.settings.IP, programSetting.settings.PORT);

                if (isConnected)
                {
                    radioTCP.Enabled = true;
                    radioTCP.Checked = true;

                    tbSendMsg.Enabled = true;
                    btnSend.Enabled = true;
                }
                else
                {
                    if(radioSerial.Enabled)
                    {
                        radioSerial.Checked = true;
                        tbSendMsg.Enabled = true;
                        btnSend.Enabled = true;
                    }
                }

                timer_MessageClear.Interval = 1000 * programSetting.settings.LogUIFormClearTime;
                timer_MessageClear.Enabled = true;
                timer_MessageClear.Tick += timer_MessageClear_Tick;

            }
        }


        public void Log(string logmsg)
        {
            string LogFormat = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), logmsg);

            if(tbLog.InvokeRequired)
            {
                tbLog.BeginInvoke(new Action(() =>
                {
                    tbLog.AppendText(LogFormat + Environment.NewLine);
                }));
            }
            else
            {
                tbLog.AppendText(LogFormat + Environment.NewLine);
            }

            FileLog(logmsg);
        }

        public void FileLog(string logmsg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] "));
            sb.Append(logmsg);

            logWriter.WriteLine(sb.ToString());
            logWriter.Flush();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(radioTCP.Checked)
            {
                if (esClient.Connected && tbSendMsg.Text.Length > 0)
                {
                    esClient.SendMessage(tbSendMsg.Text);
                }
            }
            else
            {                
                if (tbSendMsg.Text.Length > 0)
                {
                    ports.SendData(tbSendMsg.Text);
                }              
            }
        }

        void OnTCPMsgRecv(string msg)
        {
            //TCP 메시지 받았을때            
            if (msg == "Connected")
                return;

            if (msg.Substring(0, 9) == "SEND_FLA ") //FLASH에서 Echo한 메시지는 무시처리
                return;

            Log(string.Format("(TCP수신처리) TCP 받은 메시지 : (string){0}", msg));
            if (msg.Substring(0, 8) == "SEND_HW ")
                msg = msg.Substring(8);

            if (msg[0] == 'F' || msg[0] == 'f')
            {
                var writeArray = Enumerable.Range(0, msg.Length).Where(x => x % 3 == 0).Select(x => Convert.ToByte(msg.Substring(x, 2), 16)).ToArray();
                Log(string.Format("(TCP수신처리) Serial 보낸 메시지 : (byte){0}", msg));
                ports.SendData(writeArray);
                
            }
            else
            {
                Log(string.Format("(TCP수신처리) Serial 보낸 메시지 : (string){0}", msg));
                ports.SendData(msg);
            }          
        }

        void OnSerialMsgRecv(byte[] msg)
        {
            string sendString = BitConverter.ToString(msg).Replace("-", " ");
            Log(string.Format("(Serial수신처리) Serial 받은 메시지 : (byte){0}", sendString));
            //Serial 메시지 받았을때

            if(sendString[0] == 'F' || sendString[0] == 'f')
            {

            }
            else
            {
                sendString = Encoding.ASCII.GetString(msg);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("SEND_FLA "); //Header
            sb.Append(sendString);

            Log(string.Format("(Serial수신처리) TCP 보낸 메시지 : (string){0}", sb.ToString()));
            esClient.SendMessage(sb.ToString());
            
        }

        private void timer_MessageClear_Tick(object sender, EventArgs e)
        {
            if (tbLog.InvokeRequired)
            {
                tbLog.BeginInvoke(new Action(() =>
                {
                    tbLog.Clear();
                }));
            }
            else
            {
                tbLog.Clear();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log("프로그램 종료");
            logWriter.Close();
        }
    }
}
