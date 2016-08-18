using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Serializable()]
    public class Requirement
    {
        public string name { get; set; }
        //public List<List<object>> values { get; set; }
        public int displayMode { get; set; }
    }
}
