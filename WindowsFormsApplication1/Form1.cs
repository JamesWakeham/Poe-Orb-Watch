using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsFormsApplication1
{
    [Serializable()]
    public partial class Form1 : Form
    {
        Data data;

        List<Item> items;

        WebClient client = new WebClient();

        Timer timer = new Timer();
        int time;

        float dataSize;

        string leagueName, itemName;
        
        public Form1()
        {
            items = new List<Item>();
            InitializeComponent();
            FormWithTimer();
            client.Headers["Accept-Encoding"] = "gzip";
            data = new Data();
            if (Directory.Exists("Data"))
            {
                AppendToTextbox("Data directory exists");
            }
            else
            {
                Directory.CreateDirectory("Data");
                if (Directory.Exists("Data"))
                {
                    AppendToTextbox("Data directory has been created");
                }
            }
            if (File.Exists("data.txt"))
            {
                AppendToTextbox("data.txt found");
                LoadStashData();
                AppendToTextbox(data.nextChangeId + " is next change ID");
            }
        }

        public void FormWithTimer()
        {
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = (1000) * (1);              // Timer will tick every two seconds
            timer.Enabled = true;                       // Enable the timer
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time++;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataComplete);
            //client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringComplete);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
        }

        private void DownloadDataComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            AppendToTextbox(dataSize.ToString("0.00") + "mb recieved");
            dataSize = 0;
            textBox1.AppendText("Load Complete!" + "\r\n");

            var definition = new { next_change_id = "" };
            
            string result = System.Text.ASCIIEncoding.ASCII.GetString(Decompress(e.Result));

            var temp = JsonConvert.DeserializeAnonymousType(result, definition);

            data.nextChangeId = temp.next_change_id;

            RootObject tempRoot = JsonConvert.DeserializeObject<RootObject>(result);

            //BinaryOutput(tempRoot.next_change_id, tempRoot);
            ParseRootObject(tempRoot);


            DownloadData(data.nextChangeId);

            SaveStashData();
        }

        void DownloadData (string nextChangeID)
        {
            string address = "http://www.pathofexile.com/api/public-stash-tabs?id=";
            address += nextChangeID;
            data.nextChangeId = nextChangeID;
            client.DownloadDataAsync(new Uri(address));
            textBox1.AppendText("getting data at " + nextChangeID + "\r\n");
        }

        private void LoadData_Click(object sender, EventArgs e)
        {
            if (data.nextChangeId != null)
            {
                string address = "http://www.pathofexile.com/api/public-stash-tabs?id=";
                address += data.nextChangeId;
                client.DownloadDataAsync(new Uri(address));
                textBox1.AppendText("getting next change id..." + "\r\n");
                textBox1.AppendText(data.nextChangeId + " is next change ID" + "\r\n");
            }
            else
            {
                data = new Data();
                data.dataLoc = new Dictionary<string, string>();
                client.DownloadDataAsync(new Uri("http://www.pathofexile.com/api/public-stash-tabs"));
                textBox1.AppendText("Load Started..." + "\r\n");
            }
        }

        void LoadStashData()
        {
            string temp;

            using (StreamReader reader = new StreamReader("data.txt"))
            {
                temp = reader.ReadToEnd();
            }
            data = JsonConvert.DeserializeObject<Data>(temp);
        }

        void SaveStashData()
        {
            string temp = JsonConvert.SerializeObject(data);

            OutputToText(temp);
        }

        

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            dataSize = (e.BytesReceived / 1000000.0f);
            //float size = (e.BytesReceived / 1000000.0f);
            //textBox1.AppendText(size.ToString("0.00") + " mb recieved" + "\r\n");
        }

        private void DownloadStringComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            AppendToTextbox(dataSize.ToString("0.00") + "mb recieved");
            dataSize = 0;
            textBox1.AppendText("Load Complete!" + "\r\n");

            var definition = new { next_change_id = "" };
            string result = e.Result;

            var temp = JsonConvert.DeserializeAnonymousType(result, definition);

            RootObject tempRoot = JsonConvert.DeserializeObject<RootObject>(result);

            ParseRootObject(tempRoot);


            string address = "http://www.pathofexile.com/api/public-stash-tabs?id=";
            address += temp.next_change_id;
            data.nextChangeId = temp.next_change_id;
            client.DownloadDataAsync(new Uri(address));
            textBox1.AppendText("getting next change id..." + "\r\n");

            SaveStashData();
        }

        public void AppendToTextbox (string _text)
        {
            textBox1.AppendText(_text + "\r\n");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        void OutputToText (string _string)
        {
            using (StreamWriter writer = new StreamWriter("data.txt",false))
            {
                writer.Write(_string);
            }
        }

        void OutputToText(string _string, string _filePath)
        {
            Stream stream = File.Open(_filePath, FileMode.OpenOrCreate);
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(_string);
            }
        }

        string InputFromFile(string _filePath)
        {
            string temp = null;
            Stream stream = File.Open(_filePath, FileMode.Open);
            using (StreamReader reader = new StreamReader(stream))
            {
                temp = reader.ReadToEnd();
            }
            return temp;
        }

        void ParseRootObject (RootObject _root)
        {
            if (_root.stashes != null)
            {
                int i = 0;
                foreach (Stash stash in _root.stashes)
                {
                    foreach (Item item in stash.items)
                    {
                        i++;
                        string path = Path.Combine(Environment.CurrentDirectory, @"Data\", stash.accountName);
                        path += item.id;
                        path += ".stash";
                        BinaryOutput(path, item);
                    }
                }
                AppendToTextbox("Updating " + i + " items...");
            } else
            {
                AppendToTextbox("No changes made!");
            }
        }

        void BinaryOutput (string path, Object data)
        {
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter bin = new BinaryFormatter();
                bin.Serialize(stream, data);
            }
        }

        void CreateDataFile(Stash _stash)
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Data\", _stash.id);
            data.dataLoc[_stash.id] = path;
            string tempData = JsonConvert.SerializeObject(_stash);
            OutputToText(tempData, path);

            AppendToTextbox(_stash.accountName + " added to data");
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            int counter =0;
            items.Clear();
            string[] itemDirs = Directory.GetFiles("Data");
            foreach (string dir in itemDirs)
            {
                using (Stream stream = File.Open(dir, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    Item tempStash = (Item) bin.Deserialize(stream);
                    if (tempStash.frameType == 6)
                    {
                        counter++;
                        items.Add(tempStash);
                        AppendToTextbox(tempStash.name + " Found! " + counter);
                    }
                }
            }
            AppendToTextbox(items.Count + " Items found");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            leagueName = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            itemName = textBox3.Text;
        }

        static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

    }
}
