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
        //delegate void txtReturn(String text);
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                string topic = textBox1.Text;

                if (TopicsHome.Contains(topic))
                {
                     client.Unsubscribe(new string[] { topic });
                     TopicsHome.Remove(topic);
                }
                else
                {
                     client.Subscribe(new string[] { topic }, new byte[] { 0 });
                     TopicsHome.Add(topic);                    
                }
                
            }
            else
            {
               MessageBox.Show("請輸入要訂閱的主題!!");
            }

                        
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            string gateway = textBox2.Text;
            client = new MqttClient(gateway);
            clientid = Guid.NewGuid().ToString();

            client.Connect(clientid);
            MessageBox.Show(gateway+" 已連線!");
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.ConnectionClosed += Form1_Formclosing;
        }
       
        private void Form1_Formclosing(object sender, EventArgs e)
        {
            client.Disconnect();
        }
      
    }
}
