using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public MainForm()
        {
            InitializeComponent();
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
                ports = new ESPorts(programSetting.settings.SerialPortName, programSetting.settings.BaudRate,
                    programSetting.settings.Parity, programSetting.settings.DataBit, programSetting.settings.StopBits);

                ports.onRecvData = OnSerialMsgRecv;

                esClient.onRecvMsg = OnTCPMsgRecv;
                bool isConnected = esClient.ConnectToServer(programSetting.settings.IP, programSetting.settings.PORT);

                tbSendMsg.Enabled = true;
                btnSend.Enabled = true;
                if (isConnected)
                {
                    radioTCP.Enabled = true;
                    radioSerial.Enabled = true;
                    radioTCP.Checked = true;
                }
                else
                {
                    radioSerial.Enabled = true;
                    radioSerial.Checked = true;
                }
            }
        }


        public void Log(string logmsg)
        {
            string LogFormat = string.Format("[{0}] {1}", DateTime.Now, logmsg);

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
        }

        void OnSerialMsgRecv(string msg)
        {
            //Serial 메시지 받았을때
        }




    }
}
