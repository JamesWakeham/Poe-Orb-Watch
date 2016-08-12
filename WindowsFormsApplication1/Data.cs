using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Serializable()]
    class Data
    {
        public Dictionary<string, string> dataLoc = new Dictionary<string, string>();

        public string nextChangeId;
    }
}
