using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Serializable]
    public class RootObject
    {
        public string next_change_id { get; set; }
        public List<Stash> stashes { get; set; }
        public bool @public { get; set; }
    }
}
