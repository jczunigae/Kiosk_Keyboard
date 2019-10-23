using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualKeyBoard
{
    public partial class Principal : Form
    {
        const int WS_EX_NOACTIVATE = 0x08000000;
        private TcpListener listener;
        //this line of code fixed the issue
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();
        public MinForm minimized { get; set; }
        public NumbersForm numbersForm { get; set; }
        public Back backForm { get; set; }
        private TcpClient client;
        private string command = "";

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        //private System.ComponentModel.IContainer components;

        private Boolean showBackground = Boolean.Parse(ConfigurationManager.AppSettings["ShowBackground"]);
        private String keyboardLanguage = ConfigurationManager.AppSettings["KeyboardLanguage"];
        private String capsLokOffText = "Caps Lock Off";
        private String capsLokOnText = "Caps Lock On";

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= WS_EX_NOACTIVATE;


                //This line of code fix the issues
                //param.Style = 0x40000000 | 0x4000000;
                //param.Parent = GetDesktopWindow();


                return param;
            }
        }

        public Principal(string command)
        {
            backForm = new Back(this);
            this.command = command;
            InitializeComponent();
            minimized = new MinForm(this, backForm);
            minimized.Hide();
            numbersForm = new NumbersForm(this, minimized, backForm);
            numbersForm.Hide();
            backForm.setNumberForm(numbersForm);

            if(showBackground)
                backForm.Show();

            Task.Run(() => ListenEvent(this, minimized));
            //t.Wait();

            this.StartPosition = FormStartPosition.Manual;
            //MessageBox.Show(Screen.PrimaryScreen.WorkingArea.Width.ToString());
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                                   Screen.PrimaryScreen.Bounds.Height - this.Height);
            this.FormBorderStyle = FormBorderStyle.None;
            if (command == "showNumbers")
            {
                numbersForm.Show();
            }
            this.FormClosing += Form1_FormClosing;


            this.components = new System.ComponentModel.Container();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);

            notifyIcon1.Icon = VirtualKeyBoard.Properties.Resources.keyboard;//new Icon("keyboard.ico");

            notifyIcon1.ContextMenu = this.contextMenu1;

            notifyIcon1.Visible = true;

            notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);

            setKeyboardLanguage();
        } 

        private void setKeyboardLanguage()
        {
            if (keyboardLanguage.Equals("ES"))
            {
                this.notifyIcon1.Text = "Salir";
                this.spacebarBtn.Text = "Espacio";
                this.hideBtn.Text = "Ocultar";
                this.capsLockBtn.Text = "BloqMayús On";
                this.backspaceBtn.Text = "Borrar";
                capsLokOffText = "BloqMayús Off";
                capsLokOnText = "BloqMayús On";
            }
            else //default EN
            {
                this.notifyIcon1.Text = "Exit";
                this.spacebarBtn.Text = "Spacebar";
                this.hideBtn.Text = "Hide";
                this.capsLockBtn.Text = "Caps Lock On";
                this.backspaceBtn.Text = "Backspace";
                capsLokOffText = "Caps Lock Off";
                capsLokOnText = "Caps Lock On";
            }
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void ListenEvent(Form _this, Form minimized) {
            Console.WriteLine("Starting echo server...");

            int port = Int32.Parse(ConfigurationManager.AppSettings["TcpPort"]);
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            listener.Start();
            //TcpClient client = null;

            while (true) // Add your exit flag here
            {
                try { 
                client = listener.AcceptTcpClient();
                List<Object> objs = new List<Object>();
                objs.Add(_this);
                objs.Add(minimized);
                objs.Add(client);
                ThreadPool.QueueUserWorkItem(ThreadProc, objs);
                }catch(Exception e) { MessageBox.Show(e.Message); }


            }
        }

        /// <summary>
        /// Read the command line instructions
        /// showPrincipal : Show full keyboard
        /// minimize : Hide keyboard
        /// showNumbers : Show number keyboard
        /// changeToPrincipal : Show full keyboard
        /// </summary>
        /// <param name="obj"></param>
        private void ThreadProc(object obj)
        {
            var list = (List<Object>)obj;
            var client = (TcpClient)list.ElementAt(2);
            var _this = (Form)list.ElementAt(0);
            MinForm minimized = (MinForm)list.ElementAt(1);

            NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
                StreamReader reader = new StreamReader(stream, Encoding.ASCII);
                
                if (client.Connected) // If you are connected
                {
                    new Thread(() => // Thread (like Timer)
                    {
                        while (client.Connected)
                        {
                            string inputLine = "";
                            try
                            {
                                while (inputLine != null)
                                {
                                    inputLine = reader.ReadLine();
                                    if (inputLine == "showPrincipal")
                                    {
                                        this.BeginInvoke((Action)delegate
                                        {
                                            if (showBackground)
                                                backForm.Show();
                                            _this.Show();
                                            minimized.Hide();
                                            numbersForm.Hide();
                                        });
                                    }
                                    else
                                    {
                                        if (inputLine == "minimize")
                                        {
                                            this.BeginInvoke((Action)delegate
                                            {
                                                try
                                                {
                                                    /*if (_this.Visible) {
                                                        Console.WriteLine("Setting principal to minimize");
                                                        minimized.setSource(_this);
                                                    }
                                                    if (numbersForm.Visible)
                                                    {
                                                        Console.WriteLine("Setting numbers to minimize");
                                                        minimized.setSource(numbersForm);
                                                    }*/
                                                    backForm.Hide();
                                                    _this.Hide();
                                                    numbersForm.Hide();
                                                    //ver quien estaba antes de que llegara el comando?
                                                    //aunque en realida esta funcion dificilmente se usaria
                                                    minimized.Show(this);
                                                }
                                                catch (Exception e) { Debug.WriteLine(e.Message); }
                                            });
                                        }
                                        else
                                        {
                                            if (inputLine == "showNumbers")
                                            {
                                                this.BeginInvoke((Action)delegate
                                                {
                                                    _this.Hide();
                                                    minimized.Hide();
                                                    numbersForm.Show();
                                                });
                                            }
                                            else {
                                                if (inputLine == "changeToPrincipal")
                                                {
                                                    this.BeginInvoke((Action)delegate
                                                    {
                                                        try
                                                        {
                                                            backForm.Hide();
                                                            _this.Hide();
                                                            numbersForm.Hide();
                                                            minimized.setSource(_this);
                                                            minimized.Show();
                                                        }
                                                        catch (Exception e) { Debug.WriteLine(e.Message); }
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    writer.WriteLine("Echoing string: " + inputLine);
                                    Console.WriteLine("Echoing string: " + inputLine);
                                }
                            }
                            catch (Exception e) { Debug.WriteLine(e.Message); }
                            Console.WriteLine("Server saw disconnect from client.");
                            Console.ReadLine();

                        }
                    }).Start();
                }
            
           
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

        private void QBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void WBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            //Taskbar.Hide();
        }


        private void HideBtn_Click(object sender, EventArgs e)
        {
            backForm.Hide();
            this.Hide();
            numbersForm.Hide();
            minimized.Show(this);
        }

        private void SpacebarBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send(" ");
        }

        private void EBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void RBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void TBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void YBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void UBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void IBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void OBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void PBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void ABtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void SBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void DBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void FBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void GBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void HBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void JBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void KBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void LBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void SemicolonBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void ZBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void XBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void CBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void VBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void BBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void MBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void CommaBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void DotBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void BackslashBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void AtSignBtn_Click(object sender, EventArgs e)
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

        private void NumberZeroBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        /// <summary>
        /// Change capsLock mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CapsLockBtn_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Text.Equals(capsLokOffText)) {
                (sender as Button).Text = capsLokOnText;
                this.qBtn.Text = this.qBtn.Text.ToUpper();
                this.wBtn.Text = this.wBtn.Text.ToUpper();
                this.eBtn.Text = this.eBtn.Text.ToUpper();

                this.rBtn.Text = this.rBtn.Text.ToUpper();
                this.tBtn.Text = this.tBtn.Text.ToUpper();
                this.yBtn.Text = this.yBtn.Text.ToUpper();
                this.uBtn.Text = this.uBtn.Text.ToUpper();
                this.iBtn.Text = this.iBtn.Text.ToUpper();
                this.oBtn.Text = this.oBtn.Text.ToUpper();
                this.pBtn.Text = this.pBtn.Text.ToUpper();
                this.semicolonBtn.Text = this.semicolonBtn.Text.ToUpper();
                this.lBtn.Text = this.lBtn.Text.ToUpper();
                this.kBtn.Text = this.kBtn.Text.ToUpper();
                this.jBtn.Text = this.jBtn.Text.ToUpper();
                this.hBtn.Text = this.hBtn.Text.ToUpper();
                this.gBtn.Text = this.gBtn.Text.ToUpper();
                this.fBtn.Text = this.fBtn.Text.ToUpper();
                this.dBtn.Text = this.dBtn.Text.ToUpper();
                this.sBtn.Text = this.sBtn.Text.ToUpper();
                this.aBtn.Text = this.aBtn.Text.ToUpper();
                this.backslashBtn.Text = this.backslashBtn.Text.ToUpper();
                this.dotBtn.Text = this.dotBtn.Text.ToUpper();
                this.commaBtn.Text = this.commaBtn.Text.ToUpper();
                this.mBtn.Text = this.mBtn.Text.ToUpper();
                this.nBtn.Text = this.nBtn.Text.ToUpper();
                this.bBtn.Text = this.bBtn.Text.ToUpper();
                this.vBtn.Text = this.vBtn.Text.ToUpper();
                this.cBtn.Text = this.cBtn.Text.ToUpper();
                this.xBtn.Text = this.xBtn.Text.ToUpper();
                this.zBtn.Text = this.zBtn.Text.ToUpper();
            }
            else
            {
                (sender as Button).Text = capsLokOffText;
                this.qBtn.Text = this.qBtn.Text.ToLower();
                this.wBtn.Text = this.wBtn.Text.ToLower();
                this.eBtn.Text = this.eBtn.Text.ToLower();

                this.rBtn.Text = this.rBtn.Text.ToLower();
                this.tBtn.Text = this.tBtn.Text.ToLower();
                this.yBtn.Text = this.yBtn.Text.ToLower();
                this.uBtn.Text = this.uBtn.Text.ToLower();
                this.iBtn.Text = this.iBtn.Text.ToLower();
                this.oBtn.Text = this.oBtn.Text.ToLower();
                this.pBtn.Text = this.pBtn.Text.ToLower();
                this.semicolonBtn.Text = this.semicolonBtn.Text.ToLower();
                this.lBtn.Text = this.lBtn.Text.ToLower();
                this.kBtn.Text = this.kBtn.Text.ToLower();
                this.jBtn.Text = this.jBtn.Text.ToLower();
                this.hBtn.Text = this.hBtn.Text.ToLower();
                this.gBtn.Text = this.gBtn.Text.ToLower();
                this.fBtn.Text = this.fBtn.Text.ToLower();
                this.dBtn.Text = this.dBtn.Text.ToLower();
                this.sBtn.Text = this.sBtn.Text.ToLower();
                this.aBtn.Text = this.aBtn.Text.ToLower();
                this.backslashBtn.Text = this.backslashBtn.Text.ToLower();
                this.dotBtn.Text = this.dotBtn.Text.ToLower();
                this.commaBtn.Text = this.commaBtn.Text.ToLower();
                this.mBtn.Text = this.mBtn.Text.ToLower();
                this.nBtn.Text = this.nBtn.Text.ToLower();
                this.bBtn.Text = this.bBtn.Text.ToLower();
                this.vBtn.Text = this.vBtn.Text.ToLower();
                this.cBtn.Text = this.cBtn.Text.ToLower();
                this.xBtn.Text = this.xBtn.Text.ToLower();
                this.zBtn.Text = this.zBtn.Text.ToLower();
            }
            
        }

        private void ExclamationMarkBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void NumberSignBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void DollarSignBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void PercentSignBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{%}");
        }

        private void AmpersandBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{&}");
        }

        private void UnderscoreBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void QuestionMarkBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void OpenParenthesisBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{(}");
        }

        private void CloseParenthesisBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{)}");
        }

        private void QuotationMarkBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void SlashBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void AsteriskBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void PlusSignBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{+}");
        }

        private void MinusSignBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void EqualsSignBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send((sender as Button).Text);
        }

        private void BackspaceBtn_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{BACKSPACE}");
        }
    }
}
