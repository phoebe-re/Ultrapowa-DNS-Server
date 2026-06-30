using System;
using System.Threading;
using static System.Console;
using DNS.Server;
using System.Net;

namespace DNS
{
    class Program
    {
        static string Title, Ta;
        static Thread T { get; set; }

        static void Main(string[] args)
        {
            Title = "Ultrapowa DNS Server v1.0 - © 2016";
            foreach (char t in Title)
            {
                Ta += t;
                Console.Title = Ta;
                Thread.Sleep(20);
            }
            ForegroundColor = ConsoleColor.Red;
            WriteLine(
                @"
      ____ ___.__   __                                              
     |    |   \  |_/  |_____________  ______   ______  _  _______   
     |    |   /  |\   __\_  __ \__  \ \____ \ /  _ \ \/ \/ /\__  \  
     |    |  /|  |_|  |  |  | \// __ \|  |_> >  <_> )     /  / __ \_
     |______/ |____/__|  |__|  (____  /   __/ \____/ \/\_/  (____  /
                                    \/|__|                       \/
                  ");

            ResetColor();
            WriteLine("[DNS]    -> This program is made by the Ultrapowa Network development team.");
            WriteLine("[DNS]    -> You can find the source at www.ultrapowa.com");
            WriteLine("[DNS]    -> Ultrapowa is proudly licensed under GPLv3.0");
            WriteLine("[DNS]    -> Don't forget to visit www.ultrapowa.com daily for updates!");
            WriteLine("");
            WriteLine("[DNS]    -> DNS Server is now starting...");

            Start();

            ReadLine(); // keep alive
        }

        public static bool Start()
        {
            try
            {
                var port = 53;
                var ip = Convert.ToString(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]); // Use this if you're connecting to the DNS server using the same device as the device that is using the DNS server.
                //var ip = "192.168.1.108"; // You should  really use this, if you're hosting this DNS server online or using this for another device.
                var dns = new DnsServer("8.8.8.8", port);

                dns.MasterFile.AddIPAddressResourceRecord("gamea.clashofclans.com", ip);
                dns.MasterFile.AddIPAddressResourceRecord("game.clashofclans.com", ip);
                dns.MasterFile.AddIPAddressResourceRecord("game.clashroyaleapp.com", ip);
                dns.MasterFile.AddIPAddressResourceRecord("game.boombeachgame.com", ip);
                dns.MasterFile.AddIPAddressResourceRecord("game.haydaygame.com", ip);
                dns.MasterFile.AddIPAddressResourceRecord("game.brawlstarsgame.com", ip);

                WriteLine();
                WriteLine("[DNS]    DNS Server started on " + ip + " at " + port);
                WriteLine();
                ForegroundColor = ConsoleColor.Red;
                WriteLine("[DNS]    Games:");
                WriteLine("[DNS]    - Clash Royale (IOS / Android)");
                WriteLine("[DNS]    - Clash of Clans (IOS / Android)");
                WriteLine("[DNS]    - Hay Day (IOS / Android)");
                WriteLine("[DNS]    - Boom Beach (IOS / Android)");
                WriteLine("[DNS]    - Brawl Stars (IOS / Android)");
                WriteLine("[DNS]    All clients will be linked to " + ip);
                WriteLine();
                ResetColor();

                dns.Responded += ((request, response) => WriteLine("[DNS]    Linked client successful using " + request.Questions[0].Name + " at " + DateTime.UtcNow));
                dns.Responded += ((request, response) => Write(""));
                dns.Listen(port);
            }
            catch (Exception ex)
            {
                WriteLine("Can't start DNS Server on Port 53 " + ex);
                return false;
            }
            return true;
        }
    }
}
