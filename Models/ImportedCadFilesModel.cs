using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Models
{
    public class ImportedCadFilesModel
    {
        public ImportInstance CADFile { get; set; }
        public string Name { get; set; }
    }
}
