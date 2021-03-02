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

namespace MqttWin
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AllocConsole();

       
       private List<string> clients = new List<string>();
        
      
        
             

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }
       
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            
            string ReceiveMsg = Encoding.UTF8.GetString(e.Message);
            string ReceiveTpc = e.Topic;
            //Console.WriteLine(ReceiveMsg);
            
            Console.WriteLine(ReceiveTpc+$": {DateTime.Now}\t{ReceiveMsg.Split(',').Length}");
            Console.WriteLine("===============================================");
        }
        List<TreeNode> nodechecked = new List<TreeNode>();
        List<TreeNode> connectchecked = new List<TreeNode>();

        void checkednodesMt(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)               //先把想要刪除掉的node加入到list中
                {
                    nodechecked.Add(node);
                }
                else
                {
                    checkednodesMt(node.Nodes);
                }
            }
        }

        void checkedconnectMt(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked && node.Level==0)               //如果有找到，就加到List中
                {
                    connectchecked.Add(node) ;
                   
                }
                else
                {
                    checkedconnectMt(node.Nodes);
                    
                }
            }
        }

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
            treeView1.Nodes.Add(a);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string topic = textBox1.Text;
            
            try
            {
                treeView1.SelectedNode.Nodes.Add(topic) ;
                
                
            }
            catch (Exception)
            {
                MessageBox.Show("請點選Broker!!");
            }
            
        }
        

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 0&&e.Node.Checked==true)
            {
                //連線
                
                MqttClient client = new MqttClient(e.Node.Text);
                string clientid = Guid.NewGuid().ToString();
                client.Connect(clientid);
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                client.ConnectionClosed += Form1_Formclosing;
                // clients.Add(new MqttClient(e.Node.Text));
                clients.Add(e.Node.Text);
                e.Node.Tag = client;
                MessageBox.Show(e.Node.Text + " 已連線!");
            }
            else if (e.Node.Level > 0 && e.Node.Checked==false)
            {
                //Unsub
                MqttClient mqttClient = (MqttClient)e.Node.Parent.Tag;
                mqttClient.Unsubscribe(new string[] { e.Node.Text });
            }
            else if (e.Node.Level > 0 && e.Node.Checked == true)
            {
                //Subscribe
                MqttClient mqttClient = (MqttClient)e.Node.Parent.Tag;
                mqttClient.Subscribe(new string[] { e.Node.Text }, new byte[] { 0 });
            }

           /* if (e.Node.Level == 0 && e.Node.Checked == true)
            {
                button2.Enabled = true;      
            }
            else
            {
                button2.Enabled = false;             
            }

            if ( e.Node.Level > 0 && e.Node.Checked == true )
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }*/

        }
     
    }
}
