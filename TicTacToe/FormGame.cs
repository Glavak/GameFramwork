using System;
using System.Threading;
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
        Random random = new Random();

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
            switch (state)
            {
                case GameState.FlippingCoin:
                    int opponentPriority = BitsUtils.BytesToInt(bytes);
                    Console.WriteLine(relay.OwnId + " ondir" + priority + " " + opponentPriority + " " + Thread.CurrentThread.ManagedThreadId);
                    if (priority > opponentPriority)
                    {
                        state = GameState.OurTurn;
                        isPlayingX = true;
                        Invoke(new MethodInvoker(() =>
                        {
                            labelStatus.Text = "Your turn, it's your day!";
                        }));
                    }
                    else if (priority < opponentPriority)
                    {
                        state = GameState.OpponentTurn;
                        isPlayingX = false;

                        Invoke(new MethodInvoker(() =>
                        {
                            labelStatus.Text = "Opponents turn, better luck next time";
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
                    }
                }
            }
        }

        private void MakeTurn(byte x, byte y)
        {
            relay.SendDirectMessage(opponentGuid, new[] {x, y});
            buttons[x, y].Text = isPlayingX ? "X" : "O";
            state = GameState.OpponentTurn;
            labelStatus.Text = "Opponents turn";
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
    }

    enum GameState
    {
        FlippingCoin,
        OurTurn,
        OpponentTurn
    }
}
