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
    public partial class MinForm : Form
    {
        const int WS_EX_NOACTIVATE = 0x08000000;
        //this line of code fixed the issue
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();
        
        private Form source;
        private Back backForm;
        private Form Keyboard;
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

        public MinForm(Form Keyboard, Back backForm)
        {
            this.backForm = backForm;
            this.TransparencyKey = (BackColor);
            this.source = Keyboard;
            this.Keyboard = Keyboard;
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width,
                                   Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            setKeyboardLanguage();
        }

        private void MinForm_Load(object sender, EventArgs e)
        {

        }

        public new void Hide() {
            //this.source = source;
            this.source.Show();
            base.Hide();
        }

        public void Show(Form source)
        {
            backForm.Hide();
            this.source = source;
            base.Show();
        }

        public void setSource(Form s) {
            this.source = s;
        }

        private void KeyboardBtn_Click(object sender, EventArgs e)
        {
            if(showBackground)
                backForm.Show();
            this.Hide();
            //Keyboard.Show();
            this.source.Show();
        }

        private void setKeyboardLanguage()
        {
            if (keyboardLanguage.Equals("ES"))
            {
                this.keyboardBtn.Text = "Teclado";
            }
            else
            {
                this.keyboardBtn.Text = "Keyboard";
            }
        }
    }
}
