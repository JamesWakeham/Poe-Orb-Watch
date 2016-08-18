using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Serializable()]
    class TestClass
    {
        List<string> tempList { get; set; }
        string temp = "temp";
        public TestClass()
        {
            tempList = new List<string>();
            tempList.Add(temp);
        }
    }
}
