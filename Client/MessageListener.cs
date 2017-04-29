using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;

namespace Client
{

    class MessageListener
    {
        private Connection connection;
        private JSONConvertor convertor;

        public MessageListener(Connection connection)
        {
            this.connection = connection;
            convertor = new JSONConvertor();
        }

        public void StartListen()
        {
            string message = "";

            while (true)
            {
                Console.Write("Enter message: ");
                message = Console.ReadLine();
                connection.SendObject("Message", message);
                connection.RemoveIncomingPacketHandler();
                connection.AppendIncomingPacketHandler<string>("Message", PrintIncomingMessage);
                Console.WriteLine("\nPress q to quit or any other key to send another message.");
                if (Console.ReadKey(true).Key == ConsoleKey.Q) break;
            }
        }

        private void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("\nA message was received from " + connection.ToString() + " which said '" + message + "'.");

            Console.WriteLine("After convert");
            //try
            //{
            //    convertor.ConvertStringToDataTable(message);
            //}
            //catch (IndexOutOfRangeException)
            //{
            //   Console.WriteLine("next error was occured: "+ convertor.ConvertJSONStringToString(message));
            //}
            //catch (Newtonsoft.Json.JsonReaderException)
            //{
            //    Console.WriteLine("next error was occured: " + convertor.ConvertJSONStringToString(message));
            //}
            convertor.ConvertStringToDataTable(message);
        }

    }
}
