using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleSheetImport
{
    class Program
    {
        /// <summary>
        /// simply start the program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // most of the information about the import is inside the GoogleSheetReader class
            GoogleSheetReader sheetReader = new GoogleSheetReader();
            sheetReader.Run();
        }
    }
}