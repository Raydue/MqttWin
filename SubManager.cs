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
namespace MqttWin
{
    public class SubManager                             //此處為物件連線的模板
    {    
        
        public MqttClient CTag;

        private Form1 form1 = null;
        private string name;
        private MqttClient client;



        public SubManager() { }
        public SubManager(string name,string topic,Form1 main)
        {
            form1 = main;                           //將form1參數引進來，而不是創立一個新的from1物件
            this.name = name;

            client = new MqttClient(name);
            string clientid = Guid.NewGuid().ToString();
            client.Connect(clientid);
            client.Subscribe(new string[] { topic }, new byte[] { 0 });
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;     //註冊M2MQTT library中的事件方法
            CTag = client;                          
           

            void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)        //當收到publish的時候發生
            {
                string ReceiveMsg = Encoding.UTF8.GetString(e.Message);
                string ReceiveTpc = e.Topic;
                
                string Tagcmd = form1.GetTagDT();               //將textbox4中想要查看的Tag名稱傳進來

                Console.WriteLine(name + "\\" +
                (ReceiveTpc + $": {DateTime.Now}\t{(ReceiveMsg.Split(',').Length)/4}") +
                 "\n" + ("==============================================="));       //顯示有收到幾個Tag
                if (Tagcmd != null)                 //只要收到的textbox4不為空
                {
                    JObject msgPayload = JsonConvert.DeserializeObject<JObject>(ReceiveMsg);

                    foreach (JObject jo in (from data in msgPayload["values"]           //將資料流中的values欄位傳進data
                                            where data["id"].ToString() == Tagcmd       //篩選data中的欄位id中名稱=TextBox4的資料
                                            group data by data["id"] into g             //將資料傳到g
                                            select g.First()                        
                                            ))
                    {
                        form1.setTextBox4($"ID:\n{jo["id"]}"  + Environment.NewLine +$"Value:\n{jo["v"]}"       //呼叫委派將他們印出來
                                        + Environment.NewLine + $"Quality:\n{jo["q"]}" + Environment.NewLine + $"Timestamp:\n{jo["t"]}");
                    }
                }
            }
            
        }
       

       /*  public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            
            string ReceiveMsg = Encoding.UTF8.GetString(e.Message);
            string ReceiveTpc = e.Topic;

            
           
            Console.WriteLine( name + "\\" +
            (ReceiveTpc + $": {DateTime.Now}\t{ReceiveMsg.Split(',').Length}") +
             "\n" + ("==============================================="));
            
            
        }*/

       /* public void Subscriber(MqttClient bobo,string topic,string brokername,object obj)
        {
            MqttClient mqttClient = (MqttClient)bobo;
            
            mqttClient.Subscribe(new string[] { topic }, new byte[] { 0 });
            list.Add(obj);
          
        }
        public void connecter(string node)
        {
            MqttClient client = new MqttClient(node);
            string clientid = Guid.NewGuid().ToString();
            client.Connect(clientid);
            //client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            CTag = client;
        }*/
        
    }
}
