using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace ESForm
{
    
    public class ProgramSetting : ESLogBase
    {
        public class SerialPort
        {
            public string SerialPortName { get; set; }
            public int BaudRate { get; set; }
            public string Parity { get; set; }
            public int DataBit { get; set; }
            public float StopBits { get; set; }
            public string BusLRMode { get; set; }
        }

        public class Settings
        {
            public string IP { get; set; }
            public int PORT { get; set; }
            public int BusPCNum { get; set; }
            public SerialPort[] SerialPorts { get; set; }
            public int LogUIFormClearTime { get; set; }
        }

        public Settings settings
        {
            get; private set;
        }

        public bool Initialize()
        {
            try
            {                
                string json = File.ReadAllText(@"C:\EM_HW\settings.json");
                settings = JsonConvert.DeserializeObject<Settings>(json);
                Log("세팅정보 읽어오기 성공");
                return true;
            }
            catch (Exception e)
            {
                Log(e.Message);
                Log(@"세팅정보 읽어오기 실패 - c:\EM_HW\settings.json 파일 확인해주시기 바랍니다.");
            }
            return false;
        }
    }
}
