using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dapper;

namespace GoogleSheetImport
{
    class GoogleSheetReader
    {
        private readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private readonly string AppName = "Google Sheets test importer";
        private readonly string SpreadsheetId = "1hEFGwNDDYAx1Ch4oSiTLhosrh69d1u8Gd08fRcJMzKs";
        private readonly string sheet = "Service Lines";
        private int projectID;

        private SheetsService service;

        List<ServiceLine> serviceLines = new List<ServiceLine>();
        public ImportUtils utils = new ImportUtils();

        /// <summary>
        /// begin running the program
        /// </summary>
        internal void Run()
        {
        
            GoogleCredential credential;
            // read credentials from file
            using (var stream = new FileStream("client-secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // set up google sheet service to read information through Google Sheets API using the client-secrets.json file
            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });

            // begin process of reading entries from Google Sheets
            readEntries();

            insertEntriesIntoDatabase();
        }

        

        private void readEntries()
        {
            var range = $"{sheet}";  // specified range will encapsulate all fields with data on specified Google Sheet (as dictated by name of sheet)
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);  // request to get values from spreadsheet (based off of spreadsheetID and range)
            var response = request.Execute();  // retrieve data from sheet through request
            var values = response.Values;  // split response into values
            if (values != null && values.Count > 0)
                parseRowsFromResponse(values);
            else
                Console.WriteLine("no data found");
        }

        /// <summary>
        /// parse through each row to get values from each cell
        /// </summary>
        /// <param name="values"></param>
        private void parseRowsFromResponse(IList<IList<object>> values)
        {
            // iterate through each row in returned values
            foreach (var row in values)
            {
                if (values.First() == row)
                {
                    projectID = int.Parse((string)row[3]);
                    Console.WriteLine(projectID);
                    continue;
                }
                ServiceLine serviceLine = new ServiceLine();
                foreach (var column in row)
                {
                    // turn first column in row into name of service line
                    if (row.First() == column)
                        serviceLine.ServiceLineName = (string)column;
                    string[] splitKeywords = utils.CheckForSpaces((string)column);
                    for (int i = 0; i < splitKeywords.Length; i++)
                    {
                        serviceLine.AddKeyword(splitKeywords[i]);
                    }
                }
                serviceLines.Add(serviceLine);
            }
        }
        
        /// <summary>
        /// insert found entries into specific table using connection string from App.config
        /// </summary>
        internal void insertEntriesIntoDatabase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            var table = new DataTable();
            string sqlTableDestination = "TestDB.dbo.Temp_HCServiceLineKeywords";
            using (var adapter = new SqlDataAdapter($"SELECT TOP 0 * FROM {sqlTableDestination}", connectionString))
            {
                adapter.Fill(table);
            };

            foreach (var serviceLine in serviceLines)
            {
                foreach (var keyword in serviceLine.Keywords)
                {
                    var row = table.NewRow();
                    row["ServiceLine"] = serviceLine.ServiceLineName;
                    row["Keyword"] = keyword;
                    row["ProjectID"] = projectID;
                    table.Rows.Add(row);
                }
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                bulkCopy.DestinationTableName = sqlTableDestination;
                try
                {
                    bulkCopy.WriteToServer(table);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            using (var connection = new SqlConnection(connectionString))
            {
                string sql = "EXEC TestDB.dbo.s_ImportServiceLineKeywords";
                var affectedRows = connection.Execute(sql);
            }
        }
    }
}
