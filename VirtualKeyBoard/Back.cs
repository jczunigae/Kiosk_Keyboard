using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualKeyBoard
{
    public partial class Back : Form
    {
        private Form mainForm;
        private NumbersForm numberForm;
        const int WS_EX_NOACTIVATE = 0x08000000;
        //this line of code fixed the issue
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= WS_EX_NOACTIVATE;
                return param;
            }
        }
        public Back(Form mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            
            this.GotFocus += new EventHandler(Sub_GotFocus);
        }
        public void setNumberForm(NumbersForm numberForm) {
            this.numberForm = numberForm;
        }

        private void Sub_GotFocus(object sender, EventArgs e)
        {
            if (mainForm.Visible)
                mainForm.BringToFront();
            else {
                if (numberForm.Visible) {
                    numberForm.BringToFront();
                }
            }
        }

        private void Back_Load(object sender, EventArgs e)
        {
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.StartPosition = FormStartPosition.Manual;
            //MessageBox.Show(Screen.PrimaryScreen.WorkingArea.Width.ToString());
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                                   Screen.PrimaryScreen.Bounds.Height - this.Height);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
