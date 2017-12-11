using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TwitchBot
{
    class IRC
    {
        private string OAuth { get; set; }
        private string NickName { get; set; }
        //private byte[] Byte = new byte[1024];
        string pattern = @"(\w*)!(?:.*[:])(.*)";
        const string ServerAddress = "irc.chat.twitch.tv";
        static IPHostEntry IPHost = Dns.GetHostEntry(ServerAddress);
        static IPAddress IPAddr = IPHost.AddressList[0];
        static int[] ports = new int[2] { 443, 6667 };
        //static Socket Sender = new Socket(IPAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //static IPEndPoint TwitchServer = new IPEndPoint(IPAddr, ports[0]);
        //static TcpClient tcpClient = new TcpClient(TwitchServer);
        TcpClient tcpClient;
        StreamWriter writer;
        StreamReader reader;

        public IRC() { }
        public IRC(string OAuth, string NickName)
        {
            this.OAuth = OAuth;
            this.NickName = NickName;
            tcpClient = new TcpClient();
            tcpClient.Connect(ServerAddress, ports[1]);
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
        }

        public void Connect()
        {
            writer.WriteLine($"PASS {OAuth}");
            Console.WriteLine($"PASS OAuth...");
            writer.WriteLine($"NICK {NickName}");
            Console.WriteLine($"NICK {NickName}");
            writer.Flush();
            //Sender.Connect(TwitchServer);
            //Sender.Send(Encoding.UTF8.GetBytes($"PASS {OAuth}"));
            //Console.WriteLine($"PASS {OAuth}");
            //Sender.Send(Encoding.UTF8.GetBytes($"NICK {NickName}"));
            //Console.WriteLine($"NICK {NickName}");
        }

        public void Disconnect()
        {
            tcpClient.Dispose();
            writer.Dispose();
            reader.Dispose();
            Console.WriteLine("Disconnected...");
        }

        public void JoinChannel(string ChannelName)
        {
            // TODO: Use StreamReader
            writer.WriteLine($"JOIN {ChannelName}");
            writer.Flush();
            //Sender.Send(Encoding.UTF8.GetBytes($"JOIN {ChannelName}"));
            Console.WriteLine($"Joined Channel #{ChannelName}");
        }

        public void PartChannel(string ChannelName)
        {
            writer.WriteLine($"PART {ChannelName}");
            writer.Flush();
            //Sender.Send(Encoding.UTF8.GetBytes($"PART {ChannelName}"));
            Console.WriteLine($"Left Channel #{ChannelName}");
        }

        public void SendMessage(string Message)
        {
            writer.WriteLine(Message);
            writer.Flush();
            //Sender.Send(Encoding.UTF8.GetBytes(Message));
            Console.WriteLine("Sent: " + Message);
        }

        public void ReadMessage()
        {
            // TODO: Use StreamWriter
            //int byteMessage = Sender.Receive(Byte);
            //string Message = Encoding.UTF8.GetString(Byte, 0, byteMessage);
            if (reader.Peek() > -1)
            {
                string Message = reader.ReadLine();
                MatchCollection Matches = Regex.Matches(Message, pattern);
                //Console.WriteLine(Regex.Match(Message,pattern).Value);
                if (Matches.Count > 0)
                {
                    Console.Write(Matches[0].Value);
                    Console.WriteLine(Matches[1].Value);
                }
                else { Console.WriteLine(Message); }
                if (Message == "PING :tmi.twitch.tv")
                {
                    //Sender.Send(Encoding.UTF8.GetBytes("PONG :tmi.twitch.tv"));
                    writer.WriteLine("PONG :tmi.twitch.tv");
                    writer.Flush();
                    Console.WriteLine("PONG");
                }
            }
        }
    }
}
