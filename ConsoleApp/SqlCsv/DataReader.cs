namespace SqlCsv
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        List<ImportedObject> ImportedObjects;

        public List<ImportedObject> ImportData(string fileToImport) 
        {
            ImportedObjects = new List<ImportedObject>() { };
            List<string> importedLines = ReadAsLines(fileToImport);
            ImportFromLines(importedLines);
            CorrectImportedData();
            AssignNumberOfChildren();
            return ImportedObjects;
        }

        private void ImportFromLines(IEnumerable<string> importedLines)
        {
            foreach(var importedLine in importedLines)
            {
                ImportFromLine(importedLine);
            }
        }

        private void AssignNumberOfChildren()
        {
            foreach (var parent in ImportedObjects)
            {
                foreach (var child in ImportedObjects)
                {
                    if (child.IsChildOf(parent))
                    {
                        parent.NumberOfChildren++;
                    }
                }
            }
        }

        private List<string> ReadAsLines(string fileToImport)
        {
            var importedLines = new List<string>();
            using (var streamReader = new StreamReader(fileToImport))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    importedLines.Add(line);
                }
                return importedLines;
            }
        }

        private void ImportFromLine(string importedLine)
        {
            var values = importedLine.Split(';');

            if(values.Length < 7) {
                Console.WriteLine("Ignoring invalid line: \" " + importedLine + "\"");
                return;
            }
            ImportedObject importedObject = CreateImportedObject(values);
            ImportedObjects.Add(importedObject);
        }

        private ImportedObject CreateImportedObject(string[] values)
        {
            ImportedObject iobj = new ImportedObject();
            iobj.Type = values[0];
            iobj.Name = values[1];
            iobj.Schema = values[2];
            iobj.ParentName = values[3];
            iobj.ParentType = values[4];
            iobj.DataType = values[5];
            iobj.IsNullable = values[6];
            return iobj;
        }

        private void CorrectImportedData()
        {
            foreach (var importedObject in ImportedObjects)
            {
                CleanFields(importedObject);
            }
        }

        private void CleanFields(ImportedObject importedObject)
        {
            importedObject.Type = CleanStrField(importedObject.Type).ToUpper();
            importedObject.Name = CleanStrField(importedObject.Name);
            importedObject.Schema = CleanStrField(importedObject.Schema);
            importedObject.ParentName = CleanStrField(importedObject.ParentName);
            importedObject.ParentType = CleanStrField(importedObject.ParentType);
        }

        private string CleanStrField(string field){
            return field.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
        } 

    }




}
