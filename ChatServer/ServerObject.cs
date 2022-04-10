using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ChatServer {
    class ServerObject
    {
        static TcpListener ServerListener;
        List<ClientObject> clients = new List<ClientObject>();

        protected internal void AddClient(ClientObject client)
        {
            clients.Add(client);
        }

        protected internal void RemoveClient(string id)
        {
            ClientObject DeletableClient = clients.FirstOrDefault(c=>c.Id==id);
            if (DeletableClient!=null)
                clients.Remove(DeletableClient);
        }

        protected internal void Listen()
        {
            try
            {
                ServerListener = new TcpListener(IPAddress.Any, 1337);
                ServerListener.Start();
                Console.WriteLine("Server was started waitin' for connnections");

                while (true)
                {
                    TcpClient tcpClient = ServerListener.AcceptTcpClient();

                    ClientObject client = new ClientObject(tcpClient, this);
                    Thread thread = new Thread(new ThreadStart(client.Process));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        protected internal void BroadcastToAll(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for(int i = 0; i < clients.Count; i++)
                if(clients[i].Id != id)
                    clients[i].Stream.Write(data, 0, data.Length);
        }

        protected internal void Disconnect()
        {
            ServerListener.Stop();
            for (int i = 0; i < clients.Count; i++)
                clients[i].Close();
            Environment.Exit(0);
        }
    }
}
