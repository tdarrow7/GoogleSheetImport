using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSheetImport
{
    class GoogleSheetReader
    {
        private readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private readonly string AppName = "Google Sheets test importer";
        private readonly string SpreadsheetId = "1hEFGwNDDYAx1Ch4oSiTLhosrh69d1u8Gd08fRcJMzKs";
        private readonly string sheet = "Service Lines";

        private SheetsService service;

        List<ServiceLine> serviceLines = new List<ServiceLine>();
        public ImportUtils utils = new ImportUtils();

        internal void Run()
        {
            GoogleCredential credential;
            using (var stream = new FileStream("client-secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);

            }
            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });

            ReadEntries();


            //var service = new SheetsService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = AppName
            //});

            //String spreadsheetId = "1hEFGwNDDYAx1Ch4oSiTLhosrh69d1u8Gd08fRcJMzKs";
            //String range = "Class Data!A2:E";
            //SpreadsheetsResource.ValuesResource.GetRequest request =
            //        service.Spreadsheets.Values.Get(spreadsheetId, range);

            //ValueRange response = request.Execute();
            //IList<IList<Object>> values = response.Values;
            //if (values != null && values.Count > 0)
            //{
            //    Console.WriteLine("Name, Major");
            //    foreach (var row in values)
            //    {
            //        // Print columns A and E, which correspond to indices 0 and 4.
            //        Console.WriteLine("{0}, {1}", row[0], row[4]);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No data found.");
            //}
            //Console.Read();
        }

        internal void ReadEntries()
        {
            //var range = $"{sheet}!A1:F10";
            var range = $"{sheet}";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    ServiceLine serviceLine = new ServiceLine();
                    foreach (var column in row)
                    {
                        if (row.First() == column)
                        {
                            serviceLine.ServiceLineName = (string)column;
                        }
                        string[] splitKeywords = utils.CheckForSpaces((string)column);
                        for (int i = 0; i < splitKeywords.Length; i++)
                        {
                            if (!serviceLine.Keywords.Contains(splitKeywords[i]))
                                serviceLine.Keywords.Add(splitKeywords[i]);
                        }
                        Console.Write($"{column}, ");
                    }
                    Console.WriteLine();
                    serviceLines.Add(serviceLine);
                }
            }
            else
            {
                Console.WriteLine("no data found");
            }
        }
    }
}
