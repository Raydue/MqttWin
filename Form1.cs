using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace MqttWin
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        delegate void carry(string txt);
        private List<string> clients = new List<string>();
                  

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.Nodes.Add("Connectivity");
            
        }

       /* static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            SubManager sub = new SubManager();
            string ReceiveMsg = Encoding.UTF8.GetString(e.Message);
            string ReceiveTpc = e.Topic;
            Console.WriteLine(ReceiveMsg);*/
           /* Console.WriteLine(sub.brokername);
            Console.WriteLine(ReceiveTpc+$": {DateTime.Now}\t{ReceiveMsg.Split(',').Length}");
            Console.WriteLine("===============================================");*/
            /*Console.WriteLine(DateTime.Now);
            
            string msg = Encoding.UTF8.GetString(e.Message);
            JObject msgPayload = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(e.Message));

            foreach (JObject jo in (from data in msgPayload["values"]
                                    group data by data["id"] into g
                                    select g.First()))
            {
                //Console.WriteLine($"{jo["id"]} \t {jo["v"]}");
                System.Console.WriteLine($"{jo["id"]} \t {jo["v"]}");
                
            }
            
            Console.WriteLine("===============================================");*/
        //}


        private void Form1_Formclosing(object sender, EventArgs e)
        {
            foreach(var close in clients)
            {
                MqttClient client = new MqttClient(close);
                client.Disconnect();     
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string a = textBox2.Text;
            List<string> list= a.Split(',').ToList<string>();
            if (treeView1.SelectedNode != null)
            {
                foreach (var ch in list)
                    treeView1.SelectedNode.Nodes.Add(ch);
            }
            else
            {
                MessageBox.Show("請選取Connectivity");
            }
                   
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string topic = textBox1.Text;
            List<string> list = topic.Split(',').ToList<string>();
            if (treeView1.SelectedNode != null)
            {
                foreach (var ch in list)
                    treeView1.SelectedNode.Nodes.Add(ch);
            }
            else
            {
                MessageBox.Show("請點選Broker!!");
            } 
        }
        

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 1&&e.Node.Checked==true)
            {
                //連線
                /* MqttClient client = new MqttClient(e.Node.Text);
                 string clientid = Guid.NewGuid().ToString();
                 client.Connect(clientid);
                 client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                 client.ConnectionClosed += Form1_Formclosing;
                 // clients.Add(new MqttClient(e.Node.Text));
                 clients.Add(e.Node.Text);*/
                SubManager manager = new SubManager();
              //  manager.connecter(e.Node.Text);
                e.Node.Tag = manager.CTag;
                MessageBox.Show(e.Node.Text + " 已連線!");
            }
            else if (e.Node.Level > 1 && e.Node.Checked==false)
            {
                //Unsub
                MqttClient mqttClient = (MqttClient)e.Node.Tag;
                mqttClient.Unsubscribe(new string[] { e.Node.Text });
                
            }
            else if (e.Node.Level > 1 && e.Node.Checked == true)
            {
                //Subscribe
                SubManager manager = new SubManager(e.Node.Parent.Text,e.Node.Text,this);
                e.Node.Tag = manager.CTag;
               // manager.Subscriber((MqttClient)e.Node.Parent.Tag, e.Node.Text, e.Node.Parent.Text,manager.brokername); 
                
                /*MqttClient mqttClient = (MqttClient)e.Node.Parent.Tag;
                mqttClient.Subscribe(new string[] { e.Node.Text }, new byte[] { 0 });*/
            }
        }

        private void 新增TopicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Text = treeView1.SelectedNode.Text;
            DialogResult result = form2.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                List<string> list = form2.GetTpc().Split(',').ToList<string>();
                if (treeView1.SelectedNode == null)
                {
                    foreach (var ch in list)
                        treeView1.Nodes.Add(ch);
                }
                else if(treeView1.SelectedNode.IsSelected)
                {
                    foreach (var ch in list)
                        treeView1.SelectedNode.Nodes.Add(ch);                   
                }
                treeView1.ExpandAll();
            }         

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
            }  
    
        }
        public string GetTagDT()
        {
            return textBox3.Text;
        }
        public void setTextBox4(string msg)
        {
            if (this.textBox4.InvokeRequired)
            {
                carry cary = new carry(setTextBox4);
                this.Invoke(cary, new object[] { msg });
            }
            else
            {
                textBox4.Text = msg;
            }
        }
    }
}
