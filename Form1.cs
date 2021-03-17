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
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]      //
        static extern bool AllocConsole();                              //這兩行主要是import Kernel32.dll進來，為的就是顯示console視窗

        delegate void carry(string txt);                        //用來可以在textbox上實時更新的委派，請參考第157行
        private List<string> clients = new List<string>();      //用來儲存clients的list

        private float X;        //
        private float Y;        //這兩樣變數是用來儲存winform x,y軸，縮放倍數用的 ，詳情視窗功能請參考第202行~第226行的ReSize函數為止


        private Form3 form3 = new Form3();
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.Nodes.Add("Connectivity");        //為了解決winform會自動選取節點問題，而創立的
            X = this.Width;//獲取窗體的寬度
            Y = this.Height;//獲取窗體的高度
            setTag(this);//調用方法
            
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
       

        private void Form1_Formclosing(object sender, EventArgs e)          //winform關閉的時候  將所以連線進行斷線
        {
            foreach (var close in clients)
            {
                MqttClient client = new MqttClient(close);
                client.Disconnect();
            }       
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)           //當treeview的checkbox被勾或取消時觸發
        {
            if (e.Node.Level == 1 && e.Node.Checked==true)                      //當第一層也就是broker address 且被打勾時
            {
                //連線
                /* MqttClient client = new MqttClient(e.Node.Text);             
                 string clientid = Guid.NewGuid().ToString();
                 client.Connect(clientid);
                 client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                 client.ConnectionClosed += Form1_Formclosing;
                 // clients.Add(new MqttClient(e.Node.Text));
                 clients.Add(e.Node.Text);*/
                //SubManager manager = new SubManager();                         
              //  manager.connecter(e.Node.Text);                           這邊目前暫時沒有任何作作用
                //e.Node.Tag = manager.CTag;                                      
                MessageBox.Show(e.Node.Text + " 已連線!");
            }
            else if (e.Node.Level > 1 && e.Node.Checked==false)             //當第一層以下的node被取消勾選
            {
                //Unsub
                MqttClient mqttClient = (MqttClient)e.Node.Tag;             //產生一個物件，將事件發生的node.tag提出來
                mqttClient.Unsubscribe(new string[] { e.Node.Text });       //進行退訂
                
            }
            else if (e.Node.Level > 1 && e.Node.Checked == true)             //當第一層以下的node被勾選
            {
                //Subscribe
                SubManager manager = new SubManager(e.Node.Parent.Text,e.Node.Text,this);       //產生一個物件，並輸入父節點的名稱，也就是broker name、topic name,還有視窗參數傳遞進去
                e.Node.Tag = manager.CTag;                                                  //把當初創立這個client裡面的"GUID"存進Tag裡，以方便之後操作
               // manager.Subscriber((MqttClient)e.Node.Parent.Tag, e.Node.Text, e.Node.Parent.Text,manager.brokername); 
                
                /*MqttClient mqttClient = (MqttClient)e.Node.Parent.Tag;
                mqttClient.Subscribe(new string[] { e.Node.Text }, new byte[] { 0 });*/
            }
        }

        private void 新增TopicToolStripMenuItem_Click(object sender, EventArgs e)                 //右鍵新增選項被按下時發生
        {
            Form2 form2 = new Form2();                                              //將from2當作輸入視窗  
            form2.Text = treeView1.SelectedNode.Text;                               //將form2的名稱換成被選中node的名稱，方便提醒使用者
            DialogResult result = form2.ShowDialog();                               //創立一個對話視窗的物件，以from2為基底
            
            if (result == DialogResult.OK)                                          //當按下確定時
            {
                List<string> list = form2.GetTpc().Split(',').ToList<string>();     //如果需要進行複數名稱輸入時，以逗號為分割，一一加到list中
                if (treeView1.SelectedNode == null)                                 //如果沒有選到節點時
                {
                    foreach (var ch in list)
                        treeView1.Nodes.Add(ch);                                    //直接創建node
                }
                else if(treeView1.SelectedNode.IsSelected)                          //當選取的node被選取時
                {
                    foreach (var ch in list)
                        treeView1.SelectedNode.Nodes.Add(ch);                       //在被選取的node的下一層批次加入節點
                }
                treeView1.ExpandAll();
            }         
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)                 //當右鍵觸發時
            {
                treeView1.SelectedNode = e.Node;                //被點選的node就是被反白的node
            }  
        }
        public string GetTagDT()                                //用來取得textBox3的節點資訊，用來分析收進來的資料流想要看哪個Tag之用
        {
            return textBox3.Text;
        }
        public void setTextBox4(string msg)                     //用來對textBox4進行特定Tag資訊的更新
        {
            if (this.textBox4.InvokeRequired)                   //當textbox4觸發invoke時
            {
                carry cary = new carry(setTextBox4);
                this.Invoke(cary, new object[] { msg });        //觸發invoke
            }
            else
            {
                textBox4.Text = msg;            
            }

        //    form3.RecData = msg;  傳資料到form3用
        }

        private void 刪除ToolStripMenuItem_Click(object sender, EventArgs e)      //當右鍵刪除發生時
        {
            if (treeView1.SelectedNode.IsSelected)
            {
                if (treeView1.SelectedNode.Checked)
                {
                    MessageBox.Show("請先取消勾選!!");
                }
                else
                    treeView1.SelectedNode.Remove();
            }
        }
        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete&& treeView1.SelectedNode.IsSelected)                   //按下的按鈕為Delete鍵且有節點被選取時
            {
                if (treeView1.SelectedNode.Checked)                                             
                {
                    MessageBox.Show("請先取消勾選!!");
                }
                else
                    treeView1.SelectedNode.Remove();
            }
        }
        //將控制項的寬，高，左邊距，頂邊距和文字大小存進到tag中
        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }
        private void setControls(float newx, float newy, Control cons)                  //設定各控制項的大小
        {
            //遍尋Window中的控制項，重新設定control各項的值
            foreach (Control con in cons.Controls)
            {

                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//獲取control的Tag屬性值，並分割後存成char
                float a = System.Convert.ToSingle(mytag[0]) * newx;//根據window縮放比例確定char的值，寬度
                con.Width = (int)a;//寬度
                a = System.Convert.ToSingle(mytag[1]) * newy;//高度
                con.Height = (int)(a);
                a = System.Convert.ToSingle(mytag[2]) * newx;//左邊距離
                con.Left = (int)(a);
                a = System.Convert.ToSingle(mytag[3]) * newy;//上邊緣距離
                con.Top = (int)(a);
                Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字體大小
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)           //針對視窗放大縮小時
        {
            float newx = (this.Width) / X;                              
            float newy = (this.Height) / Y;
            setControls(newx, newy, this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form3.Show();
            
        }
    }
}
