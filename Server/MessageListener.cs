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

        private DataTable tableToSend;
        static string toSendObject;
        private string sqlQuery;
        private MySqlDao mySqlDao;

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
            MySqlConnection sqlConnection = new MySqlConnection(connection);
            mySqlDao = new MySqlDao(sqlConnection);
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
            createSendObject(message);
            connection.SendObject("Message", toSendObject); 
        }

        private void createSendObject(string message)
        {
            sqlQuery = message;
            tableToSend = mySqlDao.RunQuery(message);
            toSendObject = convertToJSONString(tableToSend);
        }

        private string convertToJSONString(DataTable table)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in table.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        public void WaitForClosing()
        {
            if (Console.ReadKey(true).Key == ConsoleKey.C) CloseServer();
        }

        public void CloseServer()
        {
            mySqlDao.CloseConnection();
            NetworkComms.Shutdown();    
        }

    }
}
