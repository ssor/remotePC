using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace remotePC
{
    public partial class Form1 : Form
    {
        List<SerialPort> list = new List<SerialPort>();
        string remotePortName = string.Empty;
        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown;
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            //获取所有串口列表
            string[] names = SerialPort.GetPortNames();
            for (int i = 0, length = names.Length; i < length; i++)
            {
                string port_name = names[i];
                try
                {
                    SerialPort port = new SerialPort(port_name, 9600);
                    port.DataReceived += (sd, _e) =>
                    {
                        SerialPort sp = (SerialPort)sd;
                        string indata = ((SerialPort)sp).ReadTo("]");
                        indata += "]";
                        Console.WriteLine(((SerialPort)sp).PortName + " Data Received:");
                        Console.WriteLine(indata);

                        if (indata.IndexOf("remotePC") >= 0)
                        {
                            this.remotePortName = sp.PortName;
                            Program.savePortName(this.remotePortName);
                            foreach (SerialPort s in list)
                            {
                                s.Close();
                            }

                            Console.WriteLine("remote Port Found => " + remotePortName);
                            Action invoke = () =>
                            {
                                Program.frmMain.setupListeningPort();
                                this.Close();
                            };
                            this.Invoke(invoke);
                        }

                    };

                    list.Add(port);
                    port.Open();

                }
                catch
                {

                }
            }
        }
    }
}
