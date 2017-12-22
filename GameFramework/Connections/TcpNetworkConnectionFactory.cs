﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameFramework
{
    public class TcpNetworkConnectionFactory :
        INetworkConnectionFactory<TcpNetworkConnection, IPEndPoint>
    {
        public EventHandler<TcpNetworkConnection> OnClientConnected { get; set; }

        private CancellationTokenSource listeningCancellation;
        private bool disposed = false;

        public TcpNetworkConnectionFactory(int listeningPort)
        {
            this.StartListening(listeningPort);
        }

        private void StartListening(int listeningPort)
        {
            var listener = new TcpListener(IPAddress.Any, listeningPort);
            listener.AllowNatTraversal(true);
            listener.Start();

            listeningCancellation = new CancellationTokenSource();
            listeningCancellation.Token.Register(listener.Stop);

            Task.Run(async () =>
            {
                while (!listeningCancellation.IsCancellationRequested)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    var tcpNetworkConnection = new TcpNetworkConnection(client);
                    OnClientConnected.Invoke(this, tcpNetworkConnection);
                    tcpNetworkConnection.StartRecievingTask();
                }
            }, listeningCancellation.Token);
        }

        public async Task<TcpNetworkConnection> ConnectToAsync(IPEndPoint endPoint, EventHandler<INetworkMessage> onMessageRecievedHandler = null)
        {
            var client = new TcpClient();
            await client.ConnectAsync(endPoint.Address, endPoint.Port);

            var tcpNetworkConnection = new TcpNetworkConnection(client);
            if (onMessageRecievedHandler != null)
                tcpNetworkConnection.OnRecieve = onMessageRecievedHandler;
            tcpNetworkConnection.StartRecievingTask();
            return tcpNetworkConnection;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                listeningCancellation?.Cancel();
            }

            disposed = true;
        }
    }
}
