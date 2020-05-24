using EpicorRestAPI;
using Ionic.BZip2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dechunk
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Epicor Host URL <no https>:");
            EpicorRest.AppPoolHost = Console.ReadLine();
            Console.WriteLine("Enter Epicor App Server Instance <Epicor10Production>");
            EpicorRest.AppPoolInstance = Console.ReadLine();
            Console.WriteLine("Enter Epicor User");
            EpicorRest.UserName = Console.ReadLine();
            Console.WriteLine("Enter Epicor User Password");
            EpicorRest.Password = Console.ReadLine();
            EpicorRest.IgnoreCertErrors = true;
            EpicorRest.License = EpicorLicenseType.WebService;

            dynamic result = EpicorRest.DynamicGet("BaqSvc", "XXChunk", new Dictionary<string, string>());
            Console.WriteLine("Enter String to Search For in Customization:");
            string stringToFind = Console.ReadLine();
            foreach(dynamic va in result.value)
            {
                string customizationData = decompressString(va.XXXChunk_Chunk.ToString());
                if(customizationData.Contains(stringToFind))
                {
                    Console.WriteLine($"Found in {Environment.NewLine}Key1:{va.XXXDef_Key1.ToString()}{Environment.NewLine}Key2:{va.XXXDef_Key2.ToString()}{Environment.NewLine}Key1:{va.XXXDef_Key3.ToString()}{Environment.NewLine}----------------------------------------------");
                }

            }
            Console.WriteLine("Done, if yound it will be listed above");
            Console.ReadLine();

        }

        private static string decompressString(string data)
        {
            try
            {
                int num;
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(data));
                BZip2InputStream stream2 = new BZip2InputStream(stream);
                byte[] buffer = new byte[0x3e8];
                MemoryStream stream3 = new MemoryStream();
                while ((num = stream2.Read(buffer, 0, 0x3e8)) > 0)
                {
                    stream3.Write(buffer, 0, num);
                }
                stream2.Close();
                stream.Close();
                byte[] bytes = stream3.ToArray();
                stream3.Close();
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return data;
            }
        }
    }
}
