using ESTCore.Tcp;

using Masuit.Tools.Hardware;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var ipendPoint = new IPEndPoint(IPAddress.Parse("192.168.1.254"), 30003);
            ESTModbusTcpClient client = new ESTModbusTcpClient(ipendPoint);
            client.ConnectAsync();

            client.Read((byte)1, "00", 2);
            //var aaa = "01 03 00 00 00 02 C4 0B";
            //var ccc = StringToHexByte(aaa);
            //client.SendAsync(ccc);

            Console.Read();
        }

        public static byte[] StringToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
                hexString += " ";
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }
}
