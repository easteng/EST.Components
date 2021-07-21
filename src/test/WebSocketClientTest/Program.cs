using ESTCore.Common.WebSocket;
using ESTCore.Message;
using ESTCore.WebSocket;
using ESTCore.WebSocket.Message;

using System;
using System.Text;
using System.Threading;

namespace WebSocketClientTest
{
    class Program
    {
        static WebSocketClient wsClient;
        static void Main(string[] args)
        {

            wsClient = new WebSocketClient("127.0.0.1", 8013);
            wsClient.OnClientApplicationMessageReceive += WsClient_OnClientApplicationMessageReceive;
            wsClient.OnClientConnected += WsClient_OnClientConnected;
            wsClient.OnNetworkError += WsClient_OnNetworkError;
            var connect = wsClient.ConnectServer();

            Console.WriteLine("Hello World!");

            Console.Read();
        }

        private static void WsClient_OnNetworkError(object sender, EventArgs e)
        {
           // throw new NotImplementedException();
        }

        private static void WsClient_OnClientConnected()
        {
            Console.WriteLine("已成功连接到服务");
            wsClient.SendServer("666");
            //wsClient.SendServer(true, "777");
            //wsClient.SendServer(1, true, Encoding.Default.GetBytes("888"));
            // throw new NotImplementedException();

            while (true)
            {
                var msg = new HealthCheckMessage("helth");
                msg.Message = DateTime.Now.ToString("mm:ss");
                wsClient.SendServer(false,msg.ToString());

                Thread.Sleep(1000);
            }
        }

        private static void WsClient_OnClientApplicationMessageReceive(WebSocketMessage message)
        {
            //throw new NotImplementedException();

            // 收到服务端消息
        }
    }
}
