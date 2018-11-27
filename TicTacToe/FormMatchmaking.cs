using System;
using System.Drawing;
using System.Windows.Forms;
using GameFramework;

namespace TicTacToe
{
    public partial class FormMatchmaking<TNetworkAddress> : Form
    {
        private readonly INetworkRelay<TNetworkAddress> relay;
        private readonly Guid matchmakingFileId;
        private readonly Guid leaderboardsFileId;

        private const byte MessageConnect = 0;
        private const byte MessageReject = 1;

        public FormMatchmaking(INetworkRelay<TNetworkAddress> relay, Guid matchmakingFileId, Guid leaderboardsFileId)
        {
            this.relay = relay;
            this.matchmakingFileId = matchmakingFileId;
            this.leaderboardsFileId = leaderboardsFileId;

            relay.OnDirectMessage += OnInviteDirectMessage;

            InitializeComponent();

            textBoxYourId.Text = relay.OwnId.ToString();

            relay.GetFile(relay.OwnId, (s, file) =>
            {
                var newEntries = file.Entries.Add("Level", "1");
                relay.UpdateFile(relay.OwnId, newEntries);
            });
        }

        private void OnInviteDirectMessage(object sender, byte[] bytes)
        {
            if (bytes[0] == MessageConnect)
            {
                relay.OnDirectMessage -= OnInviteDirectMessage;
                relay.SendDirectMessage((Guid) sender, new[] {MessageConnect});

                StartGame((Guid) sender);

                relay.GetFile(matchmakingFileId, (s, file) =>
                {
                    var newEntries = file.Entries.SetItem(relay.OwnId.ToString(), "");
                    relay.UpdateFile(matchmakingFileId, newEntries);

                    OnFileRecieved(sender, file);
                });
            }
        }

        private void StartGame(Guid opponent)
        {
            Invoke(new MethodInvoker(() =>
            {
                var formGame = new FormGame<TNetworkAddress>(relay, opponent, leaderboardsFileId);

                formGame.Show();
                Hide();

                formGame.Location = this.Location;
                formGame.FormClosed += OnFormGameClosed;
            }));
        }

        private void OnFormGameClosed(object sender, FormClosedEventArgs formClosedEventArgs)
        {
            Show();
            buttonRefresh.PerformClick();

            buttonConnect.Enabled = true;
            buttonCreate.Enabled = true;

            relay.OnDirectMessage += OnInviteDirectMessage;

            relay.GetFile(relay.OwnId, (s, file) =>
            {
                var level = file.Entries["Level"];
                Invoke(new MethodInvoker(() => textBoxLevel.Text = level));
            });
        }

        private void OnReplyDirectMessage(object sender, byte[] bytes)
        {
            relay.OnDirectMessage -= OnReplyDirectMessage;

            if (bytes[0] == MessageConnect)
            {
                StartGame((Guid) sender);
            }
            else
            {
                labelStatus.Text = "Rejected :c";
                relay.OnDirectMessage += OnInviteDirectMessage;
            }
        }

        private void listViewGames_DoubleClick(object sender, EventArgs e)
        {
            buttonConnect.PerformClick();
        }

        private void OnFileRecieved(object sender, NetworkFile networkFile)
        {
            Invoke(new MethodInvoker(() =>
            {
                listViewGames.BeginUpdate();
                listViewGames.Items.Clear();
            }));

            foreach (var entry in networkFile.Entries)
            {
                string message = entry.Value;
                string authorGuid = entry.Key;

                if (message == "") continue;

                var listViewItem = new ListViewItem(new[] {message, authorGuid, GetPlayerLevel(new Guid(authorGuid))});
                if (authorGuid == relay.OwnId.ToString()) listViewItem.BackColor = Color.Aquamarine;
                Invoke(new MethodInvoker(() => { listViewGames.Items.Add(listViewItem); }));
            }

            Invoke(new MethodInvoker(() =>
            {
                listViewGames.EndUpdate();
                labelStatus.Text = "Ready";
            }));
        }

        private string GetPlayerLevel(Guid playerId)
        {
            NetworkFile file = relay.GetFileLocalOrNull(playerId, false, true);

            if (file == null) return "N/A";

            if (!file.Entries.TryGetValue("Level", out string level)) return "N/A";

            return level;
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (listViewGames.SelectedItems.Count != 1) return;

            Guid opponentGuid = Guid.Parse(listViewGames.SelectedItems[0].SubItems[1].Text);

            if (opponentGuid == relay.OwnId) return;

            labelStatus.Text = "Connecting..";

            relay.OnDirectMessage -= OnInviteDirectMessage;
            relay.OnDirectMessage += OnReplyDirectMessage;

            relay.SendDirectMessage(opponentGuid, new[] {MessageConnect});
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            buttonConnect.Enabled = false;
            buttonCreate.Enabled = false;

            labelStatus.Text = "Creating..";

            relay.GetFile(matchmakingFileId, (s, file) =>
            {
                var id = relay.OwnId.ToString();
                var newEntries = file.Entries.ContainsKey(id)
                    ? file.Entries.SetItem(id, "No message")
                    : file.Entries.Add(id, "No message");
                file = relay.UpdateFile(matchmakingFileId, newEntries);

                OnFileRecieved(sender, file);
            });
        }

        private void timerUpdateList_Tick(object sender, EventArgs e)
        {
            //relay.GetFile(matchmakingFileId, OnFileRecieved);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            labelStatus.Text = "Refreshing..";
            relay.GetFile(matchmakingFileId, OnFileRecieved);
        }

        private void buttonLeaders_Click(object sender, EventArgs e)
        {
            FormLeaderboards<TNetworkAddress> l = new FormLeaderboards<TNetworkAddress>(relay, leaderboardsFileId);
            l.Show();
        }
    }
}
