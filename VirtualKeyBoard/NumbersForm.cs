using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualKeyBoard
{
    public partial class NumbersForm : Form
    {
        private MinForm minizeForm;
        private Form mainForm;
        private Back backForm;
        const int WS_EX_NOACTIVATE = 0x08000000;
        //this line of code fixed the issue
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();
        private Boolean showBackground = Boolean.Parse(ConfigurationManager.AppSettings["ShowBackground"]);
        private String keyboardLanguage = ConfigurationManager.AppSettings["KeyboardLanguage"];

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= WS_EX_NOACTIVATE;
                return param;
            }
        }

       
        public NumbersForm(Form mainForm, MinForm minizeForm, Back backForm)
        {
            this.backForm = backForm;
            this.minizeForm = minizeForm;
            this.mainForm = mainForm;
            InitializeComponent();

            this.StartPosition = FormStartPosition.Manual;
            //MessageBox.Show(Screen.PrimaryScreen.WorkingArea.Width.ToString());
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                                   Screen.PrimaryScreen.Bounds.Height - this.Height);
            this.FormBorderStyle = FormBorderStyle.None;
            setKeyboardLanguage();
        }

        private void BackspaceBtn_Click(object sender, EventArgs e)
        {
            //mainForm.Show();
            //minizeForm.Hide();
            SendKeys.Send("{BACKSPACE}");
        }

        public new void Show()
        {
            if (showBackground)
                backForm.Show();
            base.Show();
            this.mainForm.Hide();
        }

        private void NumberZeroBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberOneBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberTwoBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberThreeBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberFourBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberFiveBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberSixBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberSevenBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberEightBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberNineBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void HideBtn_Click(object sender, EventArgs e)
        {
            minizeForm.Show(this);
            this.Hide();
        }

        private void numbers_Load(object sender, EventArgs e)
        {

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

        private void setKeyboardLanguage()
        {
            if (keyboardLanguage.Equals("ES"))
            {
                this.hideBtn.Text = "Ocultar";
                this.backspaceBtn.Text = "Borrar";
            }
            else //default EN
            {
                this.hideBtn.Text = "Hide";
                this.backspaceBtn.Text = "Backspace";
            }
        }
    }
}
