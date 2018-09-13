using System;
using System.Threading;
using System.Threading.Tasks;
using GameFramework;
using System.Windows.Forms;
using TicTacToe;

namespace TestLauncher
{
    internal static class Program
    {
        private static FormGame<int> client0;
        private static FormGame<int> client1;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            LocalNetworkConnectionHub hub = new LocalNetworkConnectionHub();

            LocalNetworkConnectionFactory factory0 = hub.CreateNodeFactory();
            LocalNetworkConnectionFactory factory1 = hub.CreateNodeFactory();

            INetworkRelay<int> relay0 = new NetworkRelay<LocalNetworkConnection, int>(factory0);
            INetworkRelay<int> relay1 = new NetworkRelay<LocalNetworkConnection, int>(factory1);

            relay0.ConnectToNodeAsync(1).Wait();

            Task.Delay(100).Wait();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            client0 = new FormGame<int>(relay0, relay1.OwnId);
            client1 = new FormGame<int>(relay1, relay0.OwnId);

            Task.Delay(100).Wait();

            var thread = new Thread(ThreadStart);
            // allow UI with ApartmentState.STA though [STAThread] above should give that to you
            thread.TrySetApartmentState(ApartmentState.STA);
            thread.Start();

            Application.Run(client0);
        }

        private static void ThreadStart()
        {
            Application.Run(client1); // <-- other form started on its own UI thread
        }
    }
}
