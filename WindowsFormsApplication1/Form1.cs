using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        WebClient client = new WebClient();
        string downloadString;

        public Form1()
        {
            InitializeComponent();
            OutputToText("Output Started" + "\r\n");
            //DoTheJSON();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringComplete);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadPregressChanged);
        }

        private void LoadData_Click(object sender, EventArgs e)
        {
            client.DownloadStringAsync(new Uri("http://www.pathofexile.com/api/public-stash-tabs"));
            textBox1.AppendText("Load Started..."+"\r\n");
        }

        private void DownloadPregressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //float size = (e.BytesReceived / 1000000.0f);
            //textBox1.AppendText(size.ToString("0.00") + " mb recieved" + "\r\n");
        }

        private void DownloadStringComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            OutputToText(e.Result);
            textBox1.AppendText("Load Complete!" + "\r\n");
            var definition = new { next_change_id = "" };
            string result = e.Result;
            var temp = JsonConvert.DeserializeAnonymousType(result, definition);
            textBox1.AppendText(temp.next_change_id);
            if (temp.next_change_id != null)
            {
                string address = "http://www.pathofexile.com/api/public-stash-tabs?id=";
                address += temp.next_change_id;
                client.DownloadStringAsync(new Uri(address));
                textBox1.AppendText("getting next change id..." + "\r\n");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        void OutputToText (string _string)
        {
            using (StreamWriter writer =
    new StreamWriter("important.txt",true))
            {
                writer.Write(_string);
            }
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            using (StreamReader reader = new StreamReader("important.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line); // Add to list.
                }
            }
            foreach (string item in list)
            {
                if (item.Contains("jimbobrlw"))
                {
                    textBox1.AppendText(item + "\r\n");
                }
            }
        }

        //void DoTheJSON ()
        //{
        //    //"http://www.pathofexile.com/api/public-stash-tabs"
        //    Class1 class1 = new Class1();
        //    class1.price = 99;
        //    class1.Name = "Pizza";

        //    string output = JsonConvert.SerializeObject(class1);
        //    label1.Text = output;
        //}

    }
}
