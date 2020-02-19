using System;

namespace GoogleSheetImport
{
    internal class ImportUtils
    {
        public ImportUtils()
        {
        }

        internal string[] CheckForSpaces(string column)
        {
            return column.Split(' ');
        }
    }
}