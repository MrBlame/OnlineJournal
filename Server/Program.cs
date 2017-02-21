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
using MySql.Data.MySqlClient;

namespace Server

{
    class Program
    {
        static void Main(string[] args)
        {

            MessageListener messageListener = new MessageListener();
            messageListener.StartServer();
            messageListener.Listen();     

            messageListener.WaitForClosing();

        }
      
    }
}