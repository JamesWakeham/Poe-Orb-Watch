using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Data data;

        WebClient client = new WebClient();

        float dataSize;

        public Form1()
        {
            InitializeComponent();
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
            } else
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataComplete);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringComplete);
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

            RootObject tempRoot = JsonConvert.DeserializeObject<RootObject>(result);

            ParseRootObject(tempRoot);

            if (temp.next_change_id != null)
            {
                string address = "http://www.pathofexile.com/api/public-stash-tabs?id=";
                address += temp.next_change_id;
                data.nextChangeId = temp.next_change_id;
                client.DownloadDataAsync(new Uri(address));
                textBox1.AppendText("getting next change id..." + "\r\n");
            }

            SaveStashData();
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

            if (temp.next_change_id != null)
            {
                string address = "http://www.pathofexile.com/api/public-stash-tabs?id=";
                address += temp.next_change_id;
                data.nextChangeId = temp.next_change_id;
                client.DownloadDataAsync(new Uri(address));
                textBox1.AppendText("getting next change id..." + "\r\n");
            }

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

        void ParseRootObject (RootObject _root)
        {
            AppendToTextbox("Updating " + _root.stashes.Count() + " stashes...");
            int i = 0;
            foreach (var item in _root.stashes)
            {
                if (item.items.Count == 0)
                {

                }
                else
                {
                    i++;
                    string path = Path.Combine(Environment.CurrentDirectory, @"Data\", item.id);

                    //if (!data.dataLoc.ContainsKey(item.id))
                    //    data.dataLoc.Add(item.id, path);

                    string tempData = JsonConvert.SerializeObject(item);
                    OutputToText(tempData, path);
                }
            }
            AppendToTextbox(i + " changes made");
            //AppendToTextbox("Now Storing " + data.dataLoc.Count() + " stashes.");
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
            OutputToText(data.ToString());
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
