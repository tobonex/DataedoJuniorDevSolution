using System;
using System.Collections.Generic;


namespace SqlCsv
{

    public class ImportedObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public double NumberOfChildren { get; set; }


        public bool IsChildOf(ImportedObject parent)
        {
            return  this.ParentType.ToUpper() == parent.Type &&
                    this.ParentName == parent.Name;
        }

    }

}