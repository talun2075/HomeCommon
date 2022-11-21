using InnerCore.Api.DeConz;
using InnerCore.Api.DeConz.Models.Lights;
using System;

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

        private static async void test()
        {
            var deconzClient = new DeConzClient("deconz.tami", 888, "463608DCF3");
            var light = await deconzClient.GetLightAsync("31");
            var lighthex = light.HexColor;


            
        }

        private static void DeconzClient_SensorChanged(object sender, InnerCore.Api.DeConz.Models.WebSocket.SensorChangedEvent e)
        {
            //var k = e;
        }
    }
}
