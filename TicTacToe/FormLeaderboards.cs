using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameFramework;

namespace TicTacToe
{
    public partial class FormLeaderboards<TNetworkAddress> : Form
    {
        private readonly INetworkRelay<TNetworkAddress> relay;
        private readonly Guid leaderboardsFileId;

        public FormLeaderboards(INetworkRelay<TNetworkAddress> relay, Guid leaderboardsFileId)
        {
            this.relay = relay;
            this.leaderboardsFileId = leaderboardsFileId;

            InitializeComponent();
        }

        private void FormLeaderboards_Load(object sender, EventArgs e)
        {
            relay.GetFile(leaderboardsFileId, FileLoaded);
        }

        private void FileLoaded(object sender, NetworkFile file)
        {
            Invoke(new MethodInvoker(() =>
            {
                listViewLeaders.BeginUpdate();
                listViewLeaders.Items.Clear();
            }));

            int place = 1;
            foreach (var entry in file.Entries.OrderByDescending(pair => Convert.ToDouble(pair.Value, CultureInfo.InvariantCulture)))
            {
                string authorGuid = entry.Key;
                string score = entry.Value;

                var listViewItem = new ListViewItem(new[] { place++.ToString(), authorGuid, score });
                if (authorGuid == relay.OwnId.ToString()) listViewItem.BackColor = Color.Aquamarine;
                Invoke(new MethodInvoker(() => { listViewLeaders.Items.Add(listViewItem); }));
            }

            Invoke(new MethodInvoker(() =>
            {
                listViewLeaders.EndUpdate();
                labelStatus.Text = "Ready";
            }));
        }
    }
}
