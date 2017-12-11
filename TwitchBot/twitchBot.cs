using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
    class twitchBot
    {
        static void Main(string[] args)
        {
            string filePath = @"D:\Code projects\TwitchBot\TwitchBot\Twitch Login.txt";
            string[] fileContent = null;
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File doesn't exists. Create a text file with your OAuth and Nickname" +
                    "respectively in the project folder.");
                File.Create(filePath);
            }
            else { fileContent = File.ReadAllLines(filePath); }
            string OAuth = fileContent[0];
            string NickName = fileContent[1];
            IRC twitchBot = new IRC(OAuth, NickName);
            twitchBot.Connect();
            twitchBot.JoinChannel(NickName);
            //try
            //{
            //    twitchBot.ReadMessage();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
            //Console.ReadKey();
            do
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        twitchBot.ReadMessage();
                    }
                    catch (System.Net.Sockets.SocketException e)
                    {
                        Console.WriteLine(e);
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            twitchBot.PartChannel(NickName);
            twitchBot.Disconnect();
            Console.ReadKey();
        }
    }
}
