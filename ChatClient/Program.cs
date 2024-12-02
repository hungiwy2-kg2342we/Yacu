using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    static void Main()
    {
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect("127.0.0.1", 12345);

        Console.Write("Enter your username: ");
        string username = Console.ReadLine();
        SendMessage(clientSocket, $"{username} has joined the chat.");

        Thread receiveThread = new Thread(() => ReceiveMessages(clientSocket));
        receiveThread.Start();

        while (true)
        {
            string msg = Console.ReadLine();
            if (msg.ToLower() == "exit")
            {
                SendMessage(clientSocket, $"{username} has left the chat.");
                break;
            }
            SendMessage(clientSocket, $"{username}: {msg}");
        }

        clientSocket.Close();
    }

    static void SendMessage(Socket clientSocket, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        clientSocket.Send(data);
    }

    static void ReceiveMessages(Socket clientSocket)
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int received = clientSocket.Receive(buffer);
                if (received == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine(msg);
            }
            catch
            {
                Console.WriteLine("Disconnected from server.");
                break;
            }
        }
    }
}
