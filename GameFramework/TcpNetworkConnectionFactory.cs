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
    public class TcpNetworkConnectionFactory
    {
        public EventHandler<TcpNetworkConnection> OnClientConnected { get; set; }

        private CancellationTokenSource listeningCancellation;
        private int port;

        public TcpNetworkConnectionFactory(int port)
        {
            this.port = port;
        }

        public void StartListening()
        {
            var listener = new TcpListener(IPAddress.Any, port);
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

        public async Task<TcpNetworkConnection> ConnectToAsync(IPAddress address, int port)
        {
            var client = new TcpClient();
            await client.ConnectAsync(address, port);

            var tcpNetworkConnection = new TcpNetworkConnection(client);
            tcpNetworkConnection.StartRecievingTask();
            return tcpNetworkConnection;
        }
    }
}
