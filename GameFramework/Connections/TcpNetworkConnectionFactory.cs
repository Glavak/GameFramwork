using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameFramework
{
    public class TcpNetworkConnectionFactory : INetworkConnectionFactory<TcpNetworkConnection, IPEndPoint>
    {
        public EventHandler<TcpNetworkConnection> OnClientConnected { get; set; }

        private CancellationTokenSource listeningCancellation;
        private int listeningPort;

        public TcpNetworkConnectionFactory(int listeningPort)
        {
            this.listeningPort = listeningPort;
        }

        public void StartListening()
        {
            var listener = new TcpListener(IPAddress.Any, listeningPort);
            listener.AllowNatTraversal(true);
            listener.Start();

            listeningCancellation = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!listeningCancellation.IsCancellationRequested)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    var tcpNetworkConnection = new TcpNetworkConnection(client);
                    tcpNetworkConnection.StartRecievingTask();
                    OnClientConnected.Invoke(this, tcpNetworkConnection);
                }
            }, listeningCancellation.Token);
        }

        public void StopListening()
        {
            if (listeningCancellation != null)
            {
                listeningCancellation.Cancel();
            }
        }

        public async Task<TcpNetworkConnection> ConnectToAsync(IPEndPoint endPoint)
        {
            var client = new TcpClient();
            await client.ConnectAsync(endPoint.Address, endPoint.Port);

            var tcpNetworkConnection = new TcpNetworkConnection(client);
            tcpNetworkConnection.StartRecievingTask();
            return tcpNetworkConnection;
        }
    }
}
