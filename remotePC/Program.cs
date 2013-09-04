using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using nsConfigDB;

namespace remotePC
{
    static class Program
    {
        //public static string portName = "com4";
        public static string portName = string.Empty;
        public static main frmMain = null;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            object o = ConfigDB.getConfig("portName");
            if (o != null)
            {
                portName = o as string;
            }
            frmMain = new main();
            Application.Run(frmMain);
        }
        public static void savePortName(string port)
        {
            ConfigDB.saveConfig("portName", port);
            portName = port;
        }
    }
}
