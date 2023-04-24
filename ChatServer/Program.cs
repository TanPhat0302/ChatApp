using ChatServer.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace ChatServer
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            while (true)
            {

                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                BroadcastConnection();
            }


        }

        static void BroadcastConnection()
        {
            foreach (var user in _users) 
            {
                foreach (var usr in _users)
                {
                    var broadcasePacket = new PacketBuilder();
                    broadcasePacket.WriteOpCode(1);
                    broadcasePacket.WriteMessage(usr.Username);
                    broadcasePacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcasePacket.GetPacketBytes());
                }
            
            }
        }

        public static void BroadcastMessage(string Message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(Message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcaseDisconnect(string uid)
        {
            var disconnectUser = _users.Where(x=>x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectUser);
            foreach (var user in _users)
            {
                var broadcasePacket = new PacketBuilder();
                broadcasePacket.WriteOpCode(10);
                broadcasePacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcasePacket.GetPacketBytes());
              
            }
            BroadcastMessage($"[{disconnectUser.Username}] Disconnected!");
        }

    }
}