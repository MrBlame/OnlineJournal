using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using NetworkCommsDotNet;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using System.Data;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace Server
{
    class MessageListener
    {
        private List<DataTable> tablesToSend;
        static string toSendObject;
        private MySqlConnection sqlConnection;
        private CommandRecognizer commandRecognizer;
        private ServerStatus serverStatus;

        public void StartServer()
        {
            System.Net.IPEndPoint adress = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 8080);
            Connection.StartListening(ConnectionType.TCP, adress);

            ConfigureServer();

            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
            {
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);
            }
        }

        private void ConfigureServer()
        {
            DataSerializer dataSerializer = DPSManager.GetDataSerializer<JSONSerializer>();

            List<DataProcessor> dataProcessors;
            Dictionary<string, string> dataProcessorOptions;

            SelectDataProcessors(out dataProcessors, out dataProcessorOptions);

            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(dataSerializer, dataProcessors, dataProcessorOptions);

            ConfigureDataBaseConnection();
        }

        private void ConfigureDataBaseConnection()
        {
            string serverName = "localhost";
            string userName = "root";
            string dbName = "universitydb";
            string port = "3306";
            string password = "IwtFbhGh_71";
            string connection = "server=" + serverName +
                ";user=" + userName +
                ";database=" + dbName +
                ";port=" + port +
                ";password=" + password + ";";
            sqlConnection = new MySqlConnection(connection);
        }

        private void SelectDataProcessors(out List<DataProcessor> dataProcessors, out Dictionary<string, string> dataProcessorOptions)
        {
            dataProcessors = new List<DataProcessor>();
            dataProcessorOptions = new Dictionary<string, string>();
            dataProcessors.Add(DPSManager.GetDataProcessor<NetworkCommsDotNet.DPSBase.SevenZipLZMACompressor.LZMACompressor>());
        }

        public void Listen()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Message", ReadIncommingCommand);
            Console.WriteLine("Server is waiting for new connection. To close server press 'C' ");
        }

        private void ReadIncommingCommand(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("\nA message was received from " + connection.ToString() + " which said '" + message + "'.");
            GenerateDataFromRequest(message);
            GetTablesToSend();
            GetCommandStatus();
            CreateSendObject(message);
            connection.SendObject("Message", toSendObject);
        }

        private void GenerateDataFromRequest(string message)
        {
            commandRecognizer = new CommandRecognizer(sqlConnection);
            commandRecognizer.GenerateDataFromRequest(message.Split(';'));
        }

        private void GetTablesToSend()
        {
            tablesToSend = new List<DataTable>();
            tablesToSend.AddRange(commandRecognizer.GetTables());
        }

        private void GetCommandStatus()
        {
            serverStatus = commandRecognizer.GetCommand();
        }

        private void CreateSendObject(string message)
        {
            switch (serverStatus)
            {
                case ServerStatus.EMPTY_RESULT:
                    {
                        toSendObject = "Empty result!";
                        break;
                    }
                case ServerStatus.WRONG_COMMAND:
                    {
                        toSendObject = "Wrong command";
                        break;
                    }
                case ServerStatus.LOGIN_FAILED:
                    {
                        toSendObject = "Wrong login or password";
                        break;
                    }
                case ServerStatus.DB_ERROR_IN_DATA:
                    {
                        toSendObject = "Error id data was found";
                        break;
                    }
                case ServerStatus.INSERT_PRESENCE_SUCCESFULL:
                    {
                        toSendObject = "Presence was updated";
                        break;
                    }
                case ServerStatus.INSERT_PRESENCE_FAILED:
                    {
                        toSendObject = "Presence wasn't updated coz of fail";
                        break;
                    }
                case ServerStatus.INSERT_MARK_SUCCESFULL:
                    {
                        toSendObject = "Mark was updated";
                        break;
                    }
                case ServerStatus.INSERT_MARK_FAILED:
                    {
                        toSendObject = "Mark was't updated coz of fail";
                        break;
                    }
                default:
                    {
                        toSendObject = ConvertToJSONStringTablesForSending(tablesToSend);
                        break;
                    }
            }
        }

        private string ConvertToJSONStringTablesForSending(List<DataTable> tables)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string result = "";
            for (int i = 0; i < tables.Count; i++)
            {
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                DataTable table = tables[i];
                foreach (DataRow dr in table.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                result += serializer.Serialize(rows) + "/";
            }
            return result;
        }

        public void WaitForClosing()
        {
            if (Console.ReadKey(true).Key == ConsoleKey.C) CloseServer();
        }

        public void CloseServer()
        {
            sqlConnection.Close();
            NetworkComms.Shutdown();
        }

    }
}
