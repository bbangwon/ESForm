using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ESForm
{
    public partial class MainForm : Form
    {
        ProgramSetting programSetting = new ProgramSetting();
        ESClient esClient = new ESClient();
        List<ESPorts> ports = new List<ESPorts>();         
        StreamWriter logWriter = null;
        Dictionary<string, string> busLRMode = new Dictionary<string, string>();

        Dictionary<string, string> serialStringBuf = new Dictionary<string, string>();
        Dictionary<string, bool> busSerialMode = new Dictionary<string, bool>();

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
                    foreach(var portSetting in programSetting.settings.SerialPorts)
                    {                        
                        ESPorts port = new ESPorts(portSetting.SerialPortName, portSetting.BaudRate, portSetting.Parity, portSetting.DataBit, portSetting.StopBits);
                        busLRMode[portSetting.SerialPortName] = portSetting.BusLRMode;
                        port.alertPort = portSetting.AlertPort;
                        port.onRecvData = OnSerialMsgRecv;
                        ports.Add(port);


                        //버스모드 초기화
                        busSerialMode[portSetting.SerialPortName] = false;
                        serialStringBuf[portSetting.SerialPortName] = "";
                    }
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

                timer_reconnect_try.Interval = 3000;
                timer_reconnect_try.Enabled = true;
                timer_reconnect_try.Tick += Timer_reconnect_try_Tick;

            }


        }

        private void Timer_reconnect_try_Tick(object sender, EventArgs e)
        {
            if(!esClient.Connected)
            {
                Log(string.Format("연결 재시도 {0};{1}", programSetting.settings.IP, programSetting.settings.PORT));
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
                    if (radioSerial.Enabled)
                    {
                        radioSerial.Checked = true;
                        tbSendMsg.Enabled = true;
                        btnSend.Enabled = true;
                    }
                }
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
                    ports.ForEach(_ => _.SendData(tbSendMsg.Text));
                }              
            }
            //테스트..
            //OnSerialMsgRecv("COM2", Encoding.Default.GetBytes("A:0000 B:0000 C:0000 D:0000 T:0000"));
        }

        void OnTCPMsgRecv(string msg)
        {
            //TCP 메시지 받았을때            
            if (msg == "Connected")
                return;
            try
            {
                if (msg.Substring(0, 9) == "SEND_FLA ") //FLASH에서 Echo한 메시지는 무시처리
                    return;

                Log(string.Format("(TCP수신처리) TCP 받은 메시지 : (string){0}", msg));
                if (msg.Substring(0, 8) == "SEND_HW ")
                {
                    msg = msg.Substring(8);

                    if (msg[0] == 'F' || msg[0] == 'f')
                    {
                        var writeArray = Enumerable.Range(0, msg.Length).Where(x => x % 3 == 0).Select(x => Convert.ToByte(msg.Substring(x, 2), 16)).ToArray();

                        if (writeArray[1] == 0x00)
                        {
                            Log(string.Format("(TCP수신처리-경광등) Serial 보낸 메시지 : (byte){0}", msg));
                            ports.FirstOrDefault(_ => _.alertPort).SendData(writeArray);
                        }
                        else
                        {
                            Log(string.Format("(TCP수신처리-일반) Serial 보낸 메시지 : (byte){0}", msg));
                            ports.Where(_ => !_.alertPort).ToList().ForEach(_=>_.SendData(writeArray));
                        }
                    }
                }
                else
                {
                    /*
                    if (msg != "AliveCheck")
                    {
                        Log(string.Format("(TCP수신처리) Serial 보낸 메시지 : (string){0}", msg));
                        ports.ForEach(_ => _.SendData(msg));
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Log(string.Format("(TCP수신처리) 처리중에러 : {0}", e.Message));
                
            }
        }

        void OnSerialMsgRecv(string portName, byte[] msg)
        {
            string sendString = BitConverter.ToString(msg).Replace("-", " ");
            Log(string.Format("(Serial [{0}] 수신처리) Serial 받은 메시지 : (byte){1}", portName, sendString));
            //Serial 메시지 받았을때

            if(!busSerialMode[portName])
            {
                if (sendString[0] == 'F' || sendString[0] == 'f')
                {

                }
                else
                {
                    sendString = Encoding.ASCII.GetString(msg);
                }

                if (sendString[0] == 'A')    //Bus..
                {                    
                    busSerialMode[portName] = true;
                    serialStringBuf[portName] = sendString;

                    Log(string.Format("(Serial [{0}] 수신처리) 버스 데이터 시작 : (string[{1}]){2}", portName, serialStringBuf[portName].Length, serialStringBuf[portName]));
                }

            }
            else
            {
                sendString = Encoding.ASCII.GetString(msg);                

                if(serialStringBuf[portName][serialStringBuf[portName].Length-1] != ' ')
                    serialStringBuf[portName] += " ";

                serialStringBuf[portName] += sendString;
                Log(string.Format("(Serial [{0}] 수신처리) 버스 데이터 추가 : (string[{1}]){2}", portName, serialStringBuf[portName].Length, serialStringBuf[portName]));
            }

            StringBuilder sb = new StringBuilder();
            if(busSerialMode[portName])
            {
                if (serialStringBuf[portName].Length >= 35)
                {
                    sb.Append("SEND_FLA "); //Header
                    sb.Append(programSetting.settings.BusPCNum + " ");
                    sb.Append(busLRMode[portName] + " ");
                    sb.Append(serialStringBuf[portName]);
                    serialStringBuf[portName] = "";
                    busSerialMode[portName] = false;
                }
                else
                {
                    Log(string.Format("(Serial [{0}] 수신처리) 버스 데이터 저장 : (string){1}", portName, serialStringBuf[portName]));
                    return;
                }
            }
            else
            {
                sb.Append("SEND_FLA "); //Header
                sb.Append(sendString);
            }

            Log(string.Format("(Serial [{0}] 수신처리) TCP 보낸 메시지 : (string){1}", portName, sb.ToString()));
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
