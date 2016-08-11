using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class Stash
    {
        public string accountName { get; set; }
        public string lastCharacterName { get; set; }
        public string id { get; set; }
        public string stash { get; set; }
        public string stashType { get; set; }
        public List<Item> items { get; set; }
        public bool @public { get; set; }
    }
}
