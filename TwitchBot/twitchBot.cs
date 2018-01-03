using System;
using System.IO;

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
                Console.WriteLine("File was created. Fill a text file with your OAuth and Nickname" +
                    "respectively in the project folder.");
                File.Create(filePath);
                Console.WriteLine("Shutting down...");
                Console.ReadKey();
                return;
                //File.WriteAllLines(filePath, new string[] {"OAuth", "Nickname"});
            }
            else fileContent = File.ReadAllLines(filePath);

            string OAuth = fileContent[0];
            string NickName = fileContent[1];

            IRC twitchBot = new IRC(OAuth, NickName);
            twitchBot.Connect();

            if (twitchBot.IsConnected)
            {
                Console.WriteLine("Connected");
                twitchBot.JoinChannel(NickName);
            }
            else
            {
                Console.WriteLine("Could not connect to the server. Shutting down");
                Console.ReadKey();
                return;
            }

            do
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        twitchBot.ReadMessage();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            twitchBot.PartChannel(NickName);
            twitchBot.Disconnect();
            Console.ReadKey();
        }
    }
}
