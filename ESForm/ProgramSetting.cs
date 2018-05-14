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

        public class Settings
        {
            public string IP { get; set; }
            public int PORT { get; set; }
            public string SerialPortName { get; set; }
            public int BaudRate { get; set; }
            public string Parity { get; set; }
            public int DataBit {get; set;}
            public float StopBits { get; set; }
        }

        public Settings settings
        {
            get; private set;
        }

        public bool Initialize()
        {
            try
            {
                
                string json = File.ReadAllText("settings.json");
                settings = JsonConvert.DeserializeObject<Settings>(json);
                Log("세팅정보 읽어오기 성공");
                return true;
            }
            catch (Exception e)
            {
                Log(e.Message);
                Log("세팅정보 읽어오기 실패");
            }
            return false;
        }
    }
}
