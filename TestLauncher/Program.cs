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
        private static FormMatchmaking<int> client0;
        private static FormMatchmaking<int> client1;

        private static readonly Guid MatchmakingFileId = DhtUtils.GenerateFileId();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            LocalNetworkConnectionHub hub = new LocalNetworkConnectionHub();

            LocalNetworkConnectionFactory factory0 = hub.CreateNodeFactory();
            LocalNetworkConnectionFactory factory1 = hub.CreateNodeFactory();

            INetworkRelay<int> relay0 = new NetworkRelay<LocalNetworkConnection, int>(factory0, MatchmakingFileId);
            INetworkRelay<int> relay1 = new NetworkRelay<LocalNetworkConnection, int>(factory1, MatchmakingFileId);

            relay0.ConnectToNodeAsync(1).Wait();

            Task.Delay(100).Wait();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            client0 = new FormMatchmaking<int>(relay0, MatchmakingFileId);
            client1 = new FormMatchmaking<int>(relay1, MatchmakingFileId);

            Task.Delay(100).Wait();

            var thread = new Thread(ThreadStart);
            thread.TrySetApartmentState(ApartmentState.STA);
            thread.Start();

            Application.Run(client0);
        }

        private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("MyHandler caught : " + e.Message);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
        }

        private static void ThreadStart()
        {
            Application.Run(client1); // <-- other form started on its own UI thread
        }
    }
}
