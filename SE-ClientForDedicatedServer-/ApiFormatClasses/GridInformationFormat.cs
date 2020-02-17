using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_ClientForDedicatedServer_.ApiFormatClasses
{
    public class GridInformationFormat
    {
        public class _Meta
        {
            public string apiVersion { get; set; }
            public string queryTime { get; set; }
        }

        
        public _Data data { get; set; }
        public _Meta Meta { get; set; }

        public GridInformationFormat()
        {
            data = new _Data();
            Meta = new _Meta();
        }

        public class _Data
        {
            public _Grids[] Grids { get; set; }
        }
    }
}
