using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace remotePC
{
    public partial class frmTip : Form
    {
        Timer timer = new Timer();
        int count = 5;
        public frmTip()
        {
            InitializeComponent();
            timer.Interval = 1000;
            timer.Tick += (sender, e) =>
            {
                if (count > 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.lblTime.Text = count.ToString();
                        int temp = count * 100 / 5;
                        this.progressBar1.Value = temp;
                    }));
                    count--;
                }
                else
                {
                    timer.Enabled = false;
                    Program.frmMain.cancelShutDownPC();
                    this.Close();
                }
            };
            timer.Enabled = true;
        }
    }
}
