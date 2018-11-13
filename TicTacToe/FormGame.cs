using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GameFramework;

namespace TicTacToe
{
    public partial class FormGame<TNetworkAddress> : Form
    {
        private readonly INetworkRelay<TNetworkAddress> relay;
        private readonly Guid opponentGuid;

        private readonly int priority;
        private bool isPlayingX;
        private Button[,] buttons;
        private GameState state;
        private readonly Random random = new Random();

        public FormGame(INetworkRelay<TNetworkAddress> relay, Guid opponentGuid)
        {
            this.relay = relay;
            this.opponentGuid = opponentGuid;
            InitializeComponent();

            textBoxYourId.Text = relay.OwnId.ToString();
            textBoxGameWith.Text = opponentGuid.ToString();

            priority = random.Next();
            relay.OnDirectMessage += OnDirectMessage;
        }

        private void OnDirectMessage(object sender, byte[] bytes)
        {
            if ((Guid) sender != opponentGuid) return;

            switch (state)
            {
                case GameState.FlippingCoin:
                    int opponentPriority = BitsUtils.BytesToInt(bytes);
                    if (priority > opponentPriority)
                    {
                        state = GameState.OurTurn;
                        isPlayingX = true;
                        Invoke(new MethodInvoker(() => { labelStatus.Text = "Your turn, it's your day!"; }));
                    }
                    else if (priority < opponentPriority)
                    {
                        state = GameState.OpponentTurn;
                        isPlayingX = false;

                        Invoke(new MethodInvoker(() =>
                        {
                            labelStatus.Text = "Opponent's turn, better luck next time";
                            SetFieldInteractable(false);
                        }));
                    }
                    else
                        throw new Exception("Equal priorities :c");

                    break;
                case GameState.OpponentTurn:
                    byte x = bytes[0];
                    byte y = bytes[1];
                    state = GameState.OurTurn;
                    Invoke(new MethodInvoker(() =>
                    {
                        buttons[x, y].Text = !isPlayingX ? "X" : "O";
                        labelStatus.Text = "Your turn";
                        SetFieldInteractable(true);
                    }));
                    CheckGameOver();
                    break;
            }
        }

        private void FormGame_Load(object sender, EventArgs e)
        {
            relay.SendDirectMessage(opponentGuid, BitsUtils.IntToBytes(priority));

            buttons = new Button[tableLayoutPanelGame.ColumnCount, tableLayoutPanelGame.RowCount];

            for (byte y = 0; y < tableLayoutPanelGame.RowCount; y++)
            {
                for (byte x = 0; x < tableLayoutPanelGame.ColumnCount; x++)
                {
                    buttons[x, y] = new Button
                    {
                        Dock = DockStyle.Fill
                    };
                    buttons[x, y].Click += OnClick;

                    tableLayoutPanelGame.Controls.Add(buttons[x, y], x, y);
                }
            }
        }

        private void OnClick(object o, EventArgs eventArgs)
        {
            for (byte y = 0; y < buttons.GetLength(0); y++)
            {
                for (byte x = 0; x < buttons.GetLength(1); x++)
                {
                    if (o.Equals(buttons[x, y]))
                    {
                        MakeTurn(x, y);
                        CheckGameOver();
                        return;
                    }
                }
            }
        }

        private void MakeTurn(byte x, byte y)
        {
            relay.SendDirectMessage(opponentGuid, new[] {x, y});
            buttons[x, y].Text = isPlayingX ? "X" : "O";
            state = GameState.OpponentTurn;
            labelStatus.Text = "Opponent's turn";
            SetFieldInteractable(false);
        }

        private void SetFieldInteractable(bool interactable)
        {
            for (byte y = 0; y < buttons.GetLength(0); y++)
            {
                for (byte x = 0; x < buttons.GetLength(1); x++)
                {
                    buttons[x, y].Enabled = interactable && buttons[x, y].Text == "";
                }
            }
        }

        private void CheckGameOver()
        {
            // Verticals
            for (int x = 0; x < 3; x++)
            {
                if (buttons[x, 0].Text == buttons[x, 1].Text && buttons[x, 2].Text == buttons[x, 1].Text)
                {
                    SetWin(buttons[x, 0].Text);
                }
            }

            // Horizontals
            for (int y = 0; y < 3; y++)
            {
                if (buttons[0, y].Text == buttons[1, y].Text && buttons[2, y].Text == buttons[1, y].Text)
                {
                    SetWin(buttons[0, y].Text);
                }
            }

            // Diag
            if (buttons[0, 0].Text == buttons[1, 1].Text && buttons[2, 2].Text == buttons[1, 1].Text)
            {
                SetWin(buttons[0, 0].Text);
            }

            // Anti-diag
            if (buttons[0, 2].Text == buttons[1, 1].Text && buttons[2, 0].Text == buttons[1, 1].Text)
            {
                SetWin(buttons[1, 1].Text);
            }
        }

        private void SetWin(string winner)
        {
            if (winner == "") return;

            state = GameState.GameOver;

            Invoke(new MethodInvoker(() =>
            {
                if (winner == "X" ^ isPlayingX)
                {
                    labelStatus.Text = "You lost :c";

                    ModifyPlayerLevel(relay.OwnId, -1);
                    //ModifyPlayerLevel(opponentGuid, 1);
                }
                else
                {
                    labelStatus.Text = "You win, congrats!";

                    ModifyPlayerLevel(relay.OwnId, 1);
                    //ModifyPlayerLevel(opponentGuid, -1);
                }

                SetFieldInteractable(false);
            }));
        }

        private void ModifyPlayerLevel(Guid playerId, int levelOffset)
        {
            relay.GetFile(playerId, (s, file) =>
            {
                int level = Convert.ToInt32(file.Entries["Level"]);

                if (level + levelOffset > 0) level += levelOffset;
                else level = 1;

                var newEntries = file.Entries.SetItem("Level", level.ToString());
                relay.UpdateFile(playerId, newEntries);
            });
        }
    }

    enum GameState
    {
        FlippingCoin,
        OurTurn,
        OpponentTurn,
        GameOver
    }
}
