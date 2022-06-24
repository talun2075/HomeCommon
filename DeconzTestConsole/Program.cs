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
            //deconzClient.SensorChanged += DeconzClient_SensorChanged;
            //await deconzClient.ListenToEvents("deconz.tami", 999);
            var workroom = await deconzClient.GetGroupAsync("33");
            var lc = new LightCommand();
            await deconzClient.SendCommandAsync(lc.TurnOn(), workroom.Lights);
        }

        private static void DeconzClient_SensorChanged(object sender, InnerCore.Api.DeConz.Models.WebSocket.SensorChangedEvent e)
        {
            //var k = e;
        }
    }
}
