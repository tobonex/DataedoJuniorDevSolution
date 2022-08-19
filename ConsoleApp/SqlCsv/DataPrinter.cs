using System;
using System.Collections.Generic;

namespace SqlCsv
{
    public class DataPrinter
    {
        public void PrintData(IEnumerable<ImportedObject> importedObjects)
        {
            PrintDatabases(importedObjects);
        }

        private void PrintDatabases(IEnumerable<ImportedObject> importedObjects)
        {
            foreach (var database in importedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");
                    PrintTablesOf(importedObjects, database);

                }
            }
        }

        private void PrintTablesOf(IEnumerable<ImportedObject> importedObjects, ImportedObject database)
        {
            foreach (var table in importedObjects)
            {
                if (table.IsChildOf(database))
                {
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");
                    PrintColumnsOf(importedObjects, table);
                }
            }
        }

        private void PrintColumnsOf(IEnumerable<ImportedObject> importedObjects, ImportedObject table)
        {
            foreach (var column in importedObjects)
            {
                if (column.IsChildOf(table))
                {
                    Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} " +
                                      $"data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");

                }
            }
        }
    }
}