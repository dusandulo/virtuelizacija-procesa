using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ImportedFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }

        public ImportedFile()
        {

        }
        
        public ImportedFile(int id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }

    }
}
