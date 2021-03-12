﻿using System;
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
    public class SubManager
    {    
        
        public MqttClient CTag;

        private Form1 form1 = null;
        private string name;
        private MqttClient client;



        public SubManager() { }
        public SubManager(string name,string topic,Form1 main)
        {
            form1 = main;
            this.name = name;

            client = new MqttClient(name);
            string clientid = Guid.NewGuid().ToString();
            client.Connect(clientid);
            client.Subscribe(new string[] { topic }, new byte[] { 0 });
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            CTag = client;
           

            void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
            {
                string ReceiveMsg = Encoding.UTF8.GetString(e.Message);
                string ReceiveTpc = e.Topic;
                
                string Tagcmd = form1.GetTagDT();

                Console.WriteLine(name + "\\" +
                (ReceiveTpc + $": {DateTime.Now}\t{(ReceiveMsg.Split(',').Length)/4}") +
                 "\n" + ("==============================================="));
                if (Tagcmd != null)
                {
                    JObject msgPayload = JsonConvert.DeserializeObject<JObject>(ReceiveMsg);

                    foreach (JObject jo in (from data in msgPayload["values"]
                                            where data["id"].ToString() == Tagcmd
                                            group data by data["id"] into g
                                            select g.First()
                                            ))
                    {
                        form1.setTextBox4($"ID:\n{jo["id"]}"  + Environment.NewLine +$"Value:\n{jo["v"]}" 
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
