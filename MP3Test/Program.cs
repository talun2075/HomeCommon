using System;
using System.IO;

namespace MP3Test
{
    class Program
    {
        static void Main(string[] args)
        {

            test();

            //Console.WriteLine("Hello World!");
            //MP3.TagLibDelivery.GetPictureHash(@"\\NAS\Musik\Musik\AAA YVA\Julie Covington_Don't Cry For Me Argentina.mp3");
            //var k = TagLib.File.Create(@"\\NAS\Musik\Musik\AAA YVA\Julie Covington_Don't Cry For Me Argentina.mp3");
            //var img = k.Tag.Pictures[0].Data.Data;
            //var imgname = k.Tag.Pictures[0].Data.Checksum;
            //File.WriteAllBytes(@"C:\talun\"+ imgname+".png", img);
            Console.ReadLine();
            
        }

        public static async void test()
        {
            await ExternalDevices.Marantz.Initialisieren("http://marantz.tami");
            ExternalDevices.Marantz.SelectedInput = ExternalDevices.MarantzInputs.PS4;
        }
    }
}
