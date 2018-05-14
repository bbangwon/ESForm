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

        ProgramSetting programSetting = null;
        ESClient esClient = new ESClient();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tbSendMsg.Enabled = false;
            btnSend.Enabled = false;

            Log("프로그램 시작");

            ESClient.onLog = Log;
            bool isConnected = esClient.ConnectToServer("127.0.0.1", 5000);

            if(isConnected)
            {
                tbSendMsg.Enabled = true;
                btnSend.Enabled = true;
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
            if(esClient.Connected && tbSendMsg.Text.Length > 0)
            {
                esClient.SendMessage(tbSendMsg.Text);
            }
        }
    }
}
