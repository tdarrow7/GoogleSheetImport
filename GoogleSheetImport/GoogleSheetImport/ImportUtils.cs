using System;

namespace GoogleSheetImport
{
    internal class ImportUtils
    {
        public ImportUtils()
        {
        }

        /// <summary>
        /// split keyword into multiple keywords if spaces are found. Return result as array
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        internal string[] CheckForSpaces(string column)
        {
            return column.Split(' ');
        }
    }
}