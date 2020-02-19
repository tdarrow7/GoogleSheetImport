using System.Collections.Generic;

namespace GoogleSheetImport
{
    internal class ServiceLine
    {
        public string ServiceLineName { get; set; }
        public List<string> Keywords { get; set; }

        public ServiceLine()
        {
            Keywords = new List<string>();
        }
    }
}