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
        private string OAuth;
        private string NickName;

        string Pattern = @"(\w*)!(?:.*[:])(.*)";

        const string ServerAddress = "irc.chat.twitch.tv";
        static IPHostEntry IPHost = Dns.GetHostEntry(ServerAddress);
        static IPAddress IPAddr = IPHost.AddressList[0];
        static int port = 6667;
        TcpClient tcpClient;
        StreamWriter writer;
        StreamReader reader;

        public bool IsConnected { get; private set; }

        public IRC() { }
        public IRC(string OAuth, string NickName)
        {
            this.OAuth = OAuth;
            this.NickName = NickName;
            tcpClient = new TcpClient();
            tcpClient.Connect(ServerAddress, port);
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
        }

        public void Connect()
        {
            try
            {

                writer.WriteLine($"PASS {OAuth}");
                //Console.WriteLine($"PASS OAuth...");
                writer.WriteLine($"NICK {NickName}");
                //Console.WriteLine($"NICK {NickName}");
                writer.Flush();
                IsConnected = true;
            }
            catch(Exception e) { Console.WriteLine(e); }
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
            writer.WriteLine($"JOIN #{ChannelName}");
            writer.Flush();
            Console.WriteLine($"Joined Channel #{ChannelName}");
        }

        public void PartChannel(string ChannelName)
        {
            writer.WriteLine($"PART {ChannelName}");
            writer.Flush();
            Console.WriteLine($"Left Channel #{ChannelName}");
        }

        public void SendMessage(string Message)
        {
            writer.WriteLine(Message);
            writer.Flush();
            Console.WriteLine("Sent: " + Message);
        }

        public void ReadMessage()
        {
            if (tcpClient.Available > 0 || reader.Peek() >= 0)
            {
                string Message = reader.ReadLine();
                Match Matches = Regex.Match(Message, Pattern);

                if (Matches.Value != "")
                {
                    Console.Write(Matches.Groups[1].Value + ": ");
                    Console.WriteLine(Matches.Groups[2].Value);
                }

                if (Message == "PING :tmi.twitch.tv")
                {
                    writer.WriteLine("PONG :tmi.twitch.tv");
                    writer.Flush();
                    Console.WriteLine("PONG");
                }
            }
        }
    }
}
