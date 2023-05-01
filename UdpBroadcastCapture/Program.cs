using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace UdpBroadcastCapture
{
    class Program
    {
        private const int Port = 10000 ;
        static async Task Main()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, Port);
            using (UdpClient socket = new UdpClient(ipEndPoint))
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(0, 0);
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast {0}", socket.Client.LocalEndPoint);
                    byte[] datagramReceived = socket.Receive(ref remoteEndPoint);

                    string message = Encoding.ASCII.GetString(datagramReceived, 0, datagramReceived.Length);
                    Console.WriteLine("Receives {0} bytes from {1} port {2} message {3}", datagramReceived.Length,
                        remoteEndPoint.Address, remoteEndPoint.Port, message);
                    Parse(message);
                    await Post(message);
                }
            }
        }

        // To parse data from the IoT devices (depends on the protocol)
        private static void Parse(string response)
        {
            string[] parts = response.Split(' ');
            //foreach (string part in parts)
            //{
            //    Console.WriteLine(part);
            //}
            string speed = parts[1];
            Console.WriteLine(speed);
            Console.WriteLine();
        }

        private static async Task Post(string message)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("https://speedtrapapi20230411142537.azurewebsites.net/api/SpeedTraps", content).Result;
                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }
        }
    }
}