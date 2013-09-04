using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace remotePC
{
    public partial class main : Form
    {
        #region 定义
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
        const uint WM_APPCOMMAND = 0x319;
        const uint APPCOMMAND_VOLUME_UP = 0x0a;
        const uint APPCOMMAND_VOLUME_DOWN = 0x09;
        const uint APPCOMMAND_VOLUME_MUTE = 0x08;
        #endregion

        #region 托盘图标
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu notifyContextMenu;
        private System.Windows.Forms.MenuItem menuItemClose;
        private System.Windows.Forms.MenuItem menuItemHelp;
        private System.Windows.Forms.MenuItem menuItemMain;
        #endregion

        SerialPort sp = null;
        string buffer = string.Empty;
        bool shuttingDownComputer = false;

        public main()
        {
            InitializeComponent();
            this.lblPortName.Text = string.Empty;
            this.lblActionTip.Text = string.Empty;

            #region 托盘图标
            this.menuItemMain = new System.Windows.Forms.MenuItem();
            this.menuItemMain.Index = 2;
            this.menuItemMain.Text = "主界面(&M)";
            this.menuItemMain.Click += (sender, e) =>
            {
                this.WindowState = FormWindowState.Normal;
            };


            this.menuItemClose = new System.Windows.Forms.MenuItem();
            this.menuItemClose.Index = 1;
            this.menuItemClose.Text = "退出(&X)";
            this.menuItemClose.Click += (sender, e) =>
            {
                this.Close();
            };

            this.menuItemHelp = new System.Windows.Forms.MenuItem();
            this.menuItemHelp.Index = 0;
            this.menuItemHelp.Text = "帮助关于(&H)";
            this.menuItemHelp.Click += (sender, e) =>
            {
                frmAbout frm = new frmAbout();
                frm.Show();
            };

            this.notifyContextMenu = new System.Windows.Forms.ContextMenu();
            this.notifyContextMenu.MenuItems.Add(menuItemMain);
            this.notifyContextMenu.MenuItems.Add(menuItemHelp);
            this.notifyContextMenu.MenuItems.Add(menuItemClose);

            this.components = new System.ComponentModel.Container();

            // Create the NotifyIcon.
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);

            notifyIcon1.Icon = new Icon("14.ico");

            notifyIcon1.ContextMenu = this.notifyContextMenu;

            notifyIcon1.Text = "PC远程遥控";
            notifyIcon1.Visible = true;

            #endregion

            this.setupListeningPort();

        }
        public void cancelShutDownPC()
        {
            this.shuttingDownComputer = false;
        }

        public void setupListeningPort()
        {
            this.lblPortName.Text = Program.portName;
            if (Program.portName != string.Empty)
            {
                try
                {
                    sp = new SerialPort(Program.portName, 9600);
                    sp.DataReceived += sp_DataReceived;
                    sp.Open();
                }
                catch
                {
                }
            }
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = ((SerialPort)sp).ReadExisting();

            buffer += indata;
            Console.WriteLine(((SerialPort)sp).PortName + " Data Received :" + buffer);
            Console.WriteLine(buffer);


            if (buffer.IndexOf("confirm") >= 0)
            {
                Console.WriteLine("确认");
                Action invoke = () =>
                {
                    if (this.shuttingDownComputer == true)
                    {
                        Process.Start("shutdown", "-s -t 0");
                        Console.WriteLine("已确认关闭电脑");
                    }
                };
                this.Invoke(invoke);
                buffer = string.Empty;
                this.refreshTip("确认关闭电脑");
            }

            if (buffer.IndexOf("shutdown") >= 0)
            {
                Console.WriteLine("关闭电脑");
                Action invoke = () =>
                {
                    this.shuttingDownComputer = true;
                    frmTip frmTip = new remotePC.frmTip();
                    frmTip.Show();
                };
                this.Invoke(invoke);

                //Process.Start("shutdown", "-s -t 0");


                buffer = string.Empty;
                this.refreshTip("关闭电脑");
            }
            if (buffer.IndexOf("up") >= 0)
            {
                Console.WriteLine("增大声音");
                Action invoke = () =>
                {
                    //加音量  
                    SendMessage(this.Handle, WM_APPCOMMAND, 0x30292, APPCOMMAND_VOLUME_UP * 0x10000);
                };
                this.Invoke(invoke);
                buffer = string.Empty;
                this.refreshTip("增大声音");

            }
            if (buffer.IndexOf("down") >= 0)
            {
                Console.WriteLine("减小声音");
                Action invoke = () =>
                {
                    //减音量  
                    SendMessage(this.Handle, WM_APPCOMMAND, 0x30292, APPCOMMAND_VOLUME_DOWN * 0x10000);
                };
                this.Invoke(invoke);
                buffer = string.Empty;
                this.refreshTip("减小声音");
            }
            if (buffer.IndexOf("mute") >= 0)
            {
                Console.WriteLine("电脑静音");
                Action invoke = () =>
                {
                    //静音  
                    SendMessage(this.Handle, WM_APPCOMMAND, 0x200eb0, APPCOMMAND_VOLUME_MUTE * 0x10000);
                };
                this.Invoke(invoke);
                buffer = string.Empty;
                this.refreshTip("电脑静音");
            }

            Console.WriteLine("End");
        }

        void refreshTip(string tip)
        {
            Action<string> invoke = (_tip) =>
            {
                this.lblActionTip.Text = _tip;
            };
            this.Invoke(invoke, tip);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sp != null)
            {
                sp.Close();
                sp.DataReceived -= sp_DataReceived;
                sp = null;
            }
            Form1 frm = new Form1();
            frm.ShowDialog();
        }
    }
}
