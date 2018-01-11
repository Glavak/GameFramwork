using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameFramework
{
    public sealed class LocalNetworkConnectionHub
    {
        /// <summary>
        /// HACK: List of tasks, which should be finished for everything in simulated network to settle down
        /// </summary>
        //TODO: make in non-static
        public static readonly List<Task> TasksToWait = new List<Task>();

        public static void WaitForSettle()
        {
            while (TasksToWait.Any())
            {
                Task.WaitAll(TasksToWait.ToArray());
                TasksToWait.RemoveAll(t => t.IsCompleted);
            }
        }

        public List<LocalNetworkConnectionFactory> NodeFactories { get; }

        public LocalNetworkConnectionHub()
        {
            NodeFactories = new List<LocalNetworkConnectionFactory>();
        }

        public LocalNetworkConnectionFactory CreateNodeFactory()
        {
            var factory = new LocalNetworkConnectionFactory(NodeFactories.Count, this);

            NodeFactories.Add(factory);

            return factory;
        }
    }
}
