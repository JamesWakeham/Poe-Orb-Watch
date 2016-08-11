using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class Item
    {
        public bool verified { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public int ilvl { get; set; }
        public string icon { get; set; }
        public string league { get; set; }
        public string id { get; set; }
        public List<Socket> sockets { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public bool corrupted { get; set; }
        public bool lockedToCharacter { get; set; }
        public string note { get; set; }
        public List<Property> properties { get; set; }
        public List<Requirement> requirements { get; set; }
        public List<string> explicitMods { get; set; }
        public List<string> flavourText { get; set; }
        public int frameType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string inventoryId { get; set; }
        public List<object> socketedItems { get; set; }
    }

    public class Socket
    {
        public int group { get; set; }
        public string attr { get; set; }
    }

    public class Property
    {
        public string name { get; set; }
        public List<object> values { get; set; }
        public int displayMode { get; set; }
    }

    public class Requirement
    {
        public string name { get; set; }
        public List<List<object>> values { get; set; }
        public int displayMode { get; set; }
    }
}
