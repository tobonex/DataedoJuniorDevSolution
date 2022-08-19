namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using SqlCsv;

    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new DataReader();
            var printer = new DataPrinter();
            try
            {
                var data = reader.ImportData("data.csv");
                printer.PrintData(data);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}
