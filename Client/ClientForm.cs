using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;

        public ClientForm()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();

                receiveThread = new Thread(ReceiveData);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                AppendMessage("✅ Connecté au serveur.");
            }
            catch (Exception ex)
            {
                AppendMessage("❌ Échec de connexion : " + ex.Message);
            }
        }

        private void ReceiveData()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // connexion fermée
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    AppendMessage(message);
                }
            }
            catch (Exception ex)
            {
                AppendMessage("🔌 Connexion perdue : " + ex.Message);
            }
            AppendMessage("❌ Le serveur a fermé la connexion.");
        }

        private void AppendMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendMessage), message);
                return;
            }
            textBoxMessages.AppendText(message + Environment.NewLine);
        }

        private void SendCommand(string command)
        {
            if (client == null || !client.Connected) return;

            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(command);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                AppendMessage("❌ Échec d'envoi, connexion perdue.");
            }
        }

        private void buttonHit_Click(object sender, EventArgs e)
        {
            SendCommand("hit");
        }

        private void buttonStand_Click(object sender, EventArgs e)
        {
            SendCommand("stand");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            try { receiveThread?.Abort(); } catch { }
            stream?.Close();
            client?.Close();
        }
    }
}
