using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    static List<Socket> clients = new List<Socket>();
    static void Main()
    {
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 12345));
        serverSocket.Listen(5);

        Console.WriteLine("Server is running on port 12345...");

        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            clients.Add(clientSocket);
            Console.WriteLine("Client connected.");

            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(clientSocket);
        }
    }

    static void HandleClient(object clientObject)
    {
        Socket clientSocket = (Socket)clientObject;

        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int received = clientSocket.Receive(buffer);
                if (received == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine($"Received: {msg}");

                Broadcast(msg, clientSocket);
            }
            catch
            {
                clients.Remove(clientSocket);
                clientSocket.Close();
                break;
            }
        }
    }

    static void Broadcast(string message, Socket senderSocket)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        foreach (Socket client in clients)
        {
            if (client != senderSocket)
            {
                client.Send(data);
            }
        }
    }
}
