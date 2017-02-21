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
using Newtonsoft.Json;


namespace Client
{
    class ServerConnection
    {
        private ConnectionInfo connectionInfo;
        private Connection newTCPConnection;
        private DataSerializer dataSerializer;
        private List<DataProcessor> dataProcessors;
        private Dictionary<string, string> dataProcessorOptions;
        private MessageListener messageListener;


        public void Connect()
        {
            EstablishConnection();
        }

        private void EstablishConnection()
        {
            dataSerializer = DPSManager.GetDataSerializer<JSONSerializer>();

            SelectDataProcessors(out dataProcessors, out dataProcessorOptions);

            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(dataSerializer, dataProcessors, dataProcessorOptions);


            connectionInfo = new ConnectionInfo("127.0.0.1", 8080);
            newTCPConnection = TCPConnection.GetConnection(connectionInfo);
        }

        private void SelectDataProcessors(out List<DataProcessor> dataProcessors, out Dictionary<string, string> dataProcessorOptions)
        {
            dataProcessors = new List<DataProcessor>();
            dataProcessorOptions = new Dictionary<string, string>();
            dataProcessors.Add(DPSManager.GetDataProcessor<NetworkCommsDotNet.DPSBase.SevenZipLZMACompressor.LZMACompressor>());
        }

        public void StartSession()
        {
            messageListener = new MessageListener(newTCPConnection);
            messageListener.StartListen();
        }

        public void CloseConnection()
        {
            newTCPConnection.CloseConnection(true);
        }
    }
}
