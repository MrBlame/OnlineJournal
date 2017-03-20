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
        private List<DataTable> tables;

        public JSONConvertor()
        {
            tables = new List<DataTable>();
        }

        public void ConvertStringToDataTable(string jsonMessage)
        {
            string[] arrayOfTables = jsonMessage.Split('/');

            for (int i = 0; i < arrayOfTables.Count(); i++)
            {
                tables.Add(JsonConvert.DeserializeObject<DataTable>(arrayOfTables[i]));
            }
            ShowTable();            // Only for console testing
        }

        public string ConvertJSONStringToString(string jsonMessage)
        {
            return JsonConvert.ToString(jsonMessage);
        }

        private void ShowTable()
        {
            for (int i = 0; i < tables.Count; i++)
            {
                for (int j = 0; j < tables[i].Rows.Count; j++)
                {
                    DataRow row = tables[i].Rows[j];

                    for (int n = 0; n < tables[i].Columns.Count; n++)
                    {
                        DataColumn column = tables[i].Columns[n];

                        Console.Write(row[column] + ", ");
                    }
                    Console.WriteLine();
                }
            }
        }

    }
}

