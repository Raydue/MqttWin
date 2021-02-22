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

namespace MqttWin
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        List<string> TopicsHome = new List<string>();
     
       public static MqttClient client;
        static string clientid;
             

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

            //Console.WriteLine(ReceiveMsg);
            Console.WriteLine($"{DateTime.Now}\t{ReceiveMsg.Split(',').Length}");
            Console.WriteLine("===============================================");
        }
        List<TreeNode> nodechecked = new List<TreeNode>();
        List<string> connectchecked = new List<string>();

       
      
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
                if (node.Checked)               //先把想要刪除掉的node加入到list中
                {
                    connectchecked.Add(node.Text) ;
                   
                }
                else
                {
                    checkedconnectMt(node.Nodes);
                    
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            for(int i = 0; i <= treeView1.Nodes.Count; i++)
            {
                if (treeView1.Nodes[i].Checked)
                {
                    checkednodesMt(treeView1.Nodes[i].Nodes);
                    break;
                }
                
            }
            
            foreach(var sub in nodechecked)
            {
                string a = sub.Text;
                client.Subscribe(new string[] { a }, new byte[] { 0 });
            }
               /* if (TopicsHome.Contains(topic))
                {
                     client.Unsubscribe(new string[] { topic });
                     TopicsHome.Remove(topic);
                }
                else
                {
                     client.Subscribe(new string[] { topic }, new byte[] { 0 });
                     TopicsHome.Add(topic);                    
                }*/
                
                                  
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
                    
            checkedconnectMt(treeView1.Nodes);
            client = new MqttClient(connectchecked.Last());
            clientid = Guid.NewGuid().ToString();

            client.Connect(clientid);
            MessageBox.Show(connectchecked.Last()+" 已連線!");
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.ConnectionClosed += Form1_Formclosing;

            for(int i = 0; i < treeView1.Nodes.Count; i++)
            {
                if (treeView1.Nodes[i].Checked)
                {
                    treeView1.Nodes[i].Tag = client;
                    break;
                }                  
            }           
        }
       
        private void Form1_Formclosing(object sender, EventArgs e)
        {
            client.Disconnect();
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
                treeView1.SelectedNode.Nodes.Add(topic);
            }
            catch (Exception)
            {
                MessageBox.Show("請點選Broker!!");
            }
            
        }
    }
}
