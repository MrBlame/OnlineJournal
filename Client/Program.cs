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
using ProtoBuf;
using Newtonsoft.Json;




namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            ServerConnection serverConnection = new ServerConnection();
            serverConnection.Connect();
            serverConnection.StartSession();
            serverConnection.CloseConnection();

        }

    }
}
