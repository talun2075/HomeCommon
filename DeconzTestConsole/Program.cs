using InnerCore.Api.DeConz;
using InnerCore.Api.DeConz.Models.Lights;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Xml.Serialization;

namespace DeconzTestConsole
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello World!");
            test();
            Console.ReadLine();
        }
        public const string link = "deconz.tami";
        private static async void test()
        {
            Console.WriteLine("Start!");
            //await DeConzClient.RegisterAsync("deconz.tami", 888, "test");
            var deconzClient = new DeConzClient("deconz.tami", 888, "463608DCF3");
            //deconzClient.SensorChanged += DeconzClient_SensorChanged1;
            //deconzClient.ListenToEvents(link,999, new System.Threading.CancellationToken());
            //var light = await deconzClient.GetLightAsync("37");
            //var lighthex = light.HexColor;
            //var comm = new LightCommand();
            //comm.TurnOn();
            //comm.Hue = 0;
            //comm.Saturation = 0;
            //comm.Brightness = 0;
            //comm.TransitionTime = new TimeSpan(0);
            //comm.Effect = Effect.ColorLoop;
            //await deconzClient.SendCommandAsync(comm, new List<string>() { "37" });
            //Console.WriteLine("ok");
            //comm.TurnOff();
            //await deconzClient.SendCommandAsync(comm, new List<string>() { "37" });
            var group = await deconzClient.GetGroupAsync("4");
        }

        private static void DeconzClient_SensorChanged1(object sender, InnerCore.Api.DeConz.Models.WebSocket.SensorChangedEvent e)
        {
            if(e.State != null)
            {
                var k = 1 + 1;
            }
        }

        private static void DeconzClient_SensorChanged(object sender, InnerCore.Api.DeConz.Models.WebSocket.SensorChangedEvent e)
        {
            //var k = e;
        }
    }
}
