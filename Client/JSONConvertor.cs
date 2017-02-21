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


namespace Client
{
    class JSONConvertor
    {
        private DataTable table;

        public void ConvertStringToDataTable(string jsonMessage)
        {
            table = JsonConvert.DeserializeObject<DataTable>(jsonMessage);

            ShowTable();

        }

        private void ShowTable()
        {
            for (int j = 0; j < table.Rows.Count; j++)
            {
                DataRow row = table.Rows[j];

                for (int n = 0; n < table.Columns.Count; n++)
                {
                    DataColumn column = table.Columns[n];

                    Console.Write(row[column]+", ");
                }

                Console.WriteLine();
            }
        }
    }
}

