namespace TicTacToe
{
    partial class FormMatchmaking<TNetworkAddress>
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Loading..."}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Honeydew, null);
            this.listViewGames = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOpponent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBoxYourId = new System.Windows.Forms.TextBox();
            this.labelYourId = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.timerUpdateList = new System.Windows.Forms.Timer(this.components);
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewGames
            // 
            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderOpponent,
            this.columnHeaderLevel});
            this.listViewGames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewGames.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.listViewGames.Location = new System.Drawing.Point(12, 38);
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.Size = new System.Drawing.Size(344, 381);
            this.listViewGames.TabIndex = 0;
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.DoubleClick += new System.EventHandler(this.listViewGames_DoubleClick);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Message";
            this.columnHeaderName.Width = 180;
            // 
            // columnHeaderOpponent
            // 
            this.columnHeaderOpponent.Text = "Opponent id";
            this.columnHeaderOpponent.Width = 120;
            // 
            // columnHeaderLevel
            // 
            this.columnHeaderLevel.Text = "Level";
            this.columnHeaderLevel.Width = 40;
            // 
            // textBoxYourId
            // 
            this.textBoxYourId.Location = new System.Drawing.Point(85, 12);
            this.textBoxYourId.Name = "textBoxYourId";
            this.textBoxYourId.ReadOnly = true;
            this.textBoxYourId.Size = new System.Drawing.Size(357, 20);
            this.textBoxYourId.TabIndex = 6;
            // 
            // labelYourId
            // 
            this.labelYourId.AutoSize = true;
            this.labelYourId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYourId.ForeColor = System.Drawing.Color.ForestGreen;
            this.labelYourId.Location = new System.Drawing.Point(12, 13);
            this.labelYourId.Name = "labelYourId";
            this.labelYourId.Size = new System.Drawing.Size(57, 17);
            this.labelYourId.TabIndex = 5;
            this.labelYourId.Text = "Your id:";
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(362, 84);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(80, 23);
            this.buttonConnect.TabIndex = 7;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(362, 113);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(80, 23);
            this.buttonCreate.TabIndex = 8;
            this.buttonCreate.Text = "Create new";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // timerUpdateList
            // 
            this.timerUpdateList.Enabled = true;
            this.timerUpdateList.Interval = 700;
            this.timerUpdateList.Tick += new System.EventHandler(this.timerUpdateList_Tick);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(362, 38);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(80, 23);
            this.buttonRefresh.TabIndex = 9;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(375, 406);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(67, 13);
            this.labelStatus.TabIndex = 10;
            this.labelStatus.Text = "Connecitng..";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // FormMatchmaking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 431);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.textBoxYourId);
            this.Controls.Add(this.labelYourId);
            this.Controls.Add(this.listViewGames);
            this.Name = "FormMatchmaking";
            this.Text = "P2P TicTacToe - Games list";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewGames;
        private System.Windows.Forms.TextBox textBoxYourId;
        private System.Windows.Forms.Label labelYourId;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderOpponent;
        private System.Windows.Forms.ColumnHeader columnHeaderLevel;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Timer timerUpdateList;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Label labelStatus;
    }
}