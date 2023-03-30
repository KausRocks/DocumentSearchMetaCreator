using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentSearchMetaCreator.Models
{
    public class MetaDataModel
    {
        public string FullContent { get; set; }

        public string SearchableContent { get; set; }

        public Dictionary<string, string> DocumentMetaData { get; set; }
    }
}
