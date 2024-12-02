using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatAppForm
{
    public partial class Form1 : Form
    {
        private Socket clientSocket;
        private Thread receiveThread;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect("127.0.0.1", 12345);

            receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string msg = TextBox1.Text;
            SendMessage(msg);
            RichTextBox1.AppendText($"You: {msg}\n");
            TextBox1.Clear();
        }

        private void SendMessage(string message)
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(data);
            }
            else
            {
                MessageBox.Show("Not connected to the server. Please try reconnecting.");
            }
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int received = clientSocket.Receive(buffer);
                    if (received == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, received);
                    Invoke(new Action(() => RichTextBox1.AppendText($"{msg}\n")));
                }
                catch
                {
                    MessageBox.Show("Disconnected from server.");
                    break;
                }
            }
        }
    }
}
