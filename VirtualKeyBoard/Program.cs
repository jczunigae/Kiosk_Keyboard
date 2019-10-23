using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualKeyBoard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static Mutex mutex = new Mutex(true, "{TKB-B9A1-45fd-1234-72F04E6BDE8F}");
        [STAThread]
        static void Main()
        {
            string command = "";
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                command = args[1].Split(':')[1];
            }
            catch (Exception e) { Debug.WriteLine(e.Message); }
            
            Principal principal = null;
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                principal = new Principal(command);
                if(command.Equals( "showNumbers"))
                    Application.Run();
                else
                    Application.Run(principal);
                mutex.ReleaseMutex();
            }
            else
            {
                //Send message
                TcpClient oClient = new TcpClient();

                try
                {

                    int port = Int32.Parse(ConfigurationManager.AppSettings["TcpPort"]);
                    oClient.Connect("127.0.0.1", port);

                    NetworkStream ns = oClient.GetStream();

                    
                    write(ns, command);

                    ns.Close();

                    oClient.Close();

                }

                catch (Exception e) { Debug.WriteLine(e.Message); }
                }
        }

        private static void write(NetworkStream ns, string message)

        {

            byte[] msg = Encoding.ASCII.GetBytes(message + Environment.NewLine);

            ns.Write(msg, 0, msg.Length);

        }
    }
}
