using System;
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

        /// <summary>
        /// add new keyword if it hasn't been added yet into keyword list
        /// </summary>
        /// <param name="keyword"></param>
        internal void AddKeyword(string keyword)
        {
            if (!Keywords.Contains(keyword))
                Keywords.Add(keyword);
        }
    }
}