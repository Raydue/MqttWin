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
            string ReceiveTpc = e.Topic;
            //Console.WriteLine(ReceiveMsg);
            Console.WriteLine(ReceiveTpc+$": {DateTime.Now}\t{ReceiveMsg.Split(',').Length}");
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
                if (node.Checked)               //如果有找到，就加到List中
                {
                    connectchecked.Add(node.Text) ;
                   
                }
                else
                {
                    checkedconnectMt(node.Nodes);
                    
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)      //訂閱功能
        {

            for(int i = 0; i <= treeView1.Nodes.Count; i++)
            {
                if (treeView1.Nodes[i].Checked)
                {
                    checkednodesMt(treeView1.Nodes[i].Nodes);       //將treeview中的第二層放到遍尋方法中找出被勾選的nodes
                    break;
                }
               /* else
                {
                    MessageBox.Show("請先連上Broker!!");
                }*/
                
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
            nodechecked.Clear();
                                  
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
                    
            checkedconnectMt(treeView1.Nodes);          //先呼叫遍尋方法
            client = new MqttClient(connectchecked.Last());     
            clientid = Guid.NewGuid().ToString();

            client.Connect(clientid);
            MessageBox.Show(connectchecked.Last()+" 已連線!");
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.ConnectionClosed += Form1_Formclosing;

            connectchecked.Clear();

            for(int i = 0; i < treeView1.Nodes.Count; i++)    //從第一層中找到第一個checkbox有打勾的
            {
                if (treeView1.Nodes[i].Checked)
                {
                    treeView1.Nodes[i].Tag = client;          //有打勾的加到所屬的node裡面
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
        private void true2false(object sender, EventArgs e)
        {
            for(int i = 0; i < treeView1.Nodes.Count; i++)
            {
               
                    for(int u = 0; u < treeView1.Nodes[i].Nodes.Count;u++)
                    {
                        if (treeView1.Nodes[i].Nodes[u].Checked == false)
                        {
                            MessageBox.Show(treeView1.Nodes[i].Nodes[u].Text);
                            break;
                        }
                            
                        
                    }
                
            }
                     
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if(e.Node.Checked==false && e.Node.Level!=0)
            {
                client.Unsubscribe(new string[] { e.Node.Text } );
                MessageBox.Show("topic:"+e.Node.Text + " 已經退訂!!");              
            }
           
           
        }
       
    }
}
