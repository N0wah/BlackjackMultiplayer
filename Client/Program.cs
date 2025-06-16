using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main()
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 5000);
            NetworkStream stream = client.GetStream();

            Thread receiveThread = new Thread(() =>
            {
                byte[] buffer = new byte[1024];
                try
                {
                    while (true)
                    {
                        int byteCount = stream.Read(buffer, 0, buffer.Length);
                        if (byteCount == 0) break; // connexion fermée
                        string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                        Console.WriteLine(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("🔌 Connexion au serveur perdue : " + ex.Message);
                }

                Console.WriteLine("❌ Le serveur a fermé la connexion. Appuyez sur une touche pour quitter.");
            });
            receiveThread.Start();

            while (true)
            {
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(input);
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch
                {
                    break; // connexion coupée, on sort de la boucle
                }
            }

            client.Close();
        }
    }
}
