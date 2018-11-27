namespace TicTacToe
{
    partial class FormLeaderboards<TNetworkAddress>
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
            this.listViewLeaders = new System.Windows.Forms.ListView();
            this.labelStatus = new System.Windows.Forms.Label();
            this.columnHeaderPlace = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPlayer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listViewLeaders
            // 
            this.listViewLeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewLeaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderPlace,
            this.columnHeaderPlayer,
            this.columnHeaderLevel});
            this.listViewLeaders.FullRowSelect = true;
            this.listViewLeaders.Location = new System.Drawing.Point(12, 12);
            this.listViewLeaders.Name = "listViewLeaders";
            this.listViewLeaders.Size = new System.Drawing.Size(396, 453);
            this.listViewLeaders.TabIndex = 0;
            this.listViewLeaders.UseCompatibleStateImageBehavior = false;
            this.listViewLeaders.View = System.Windows.Forms.View.Details;
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(344, 468);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(64, 13);
            this.labelStatus.TabIndex = 1;
            this.labelStatus.Text = "Refreshing..";
            // 
            // columnHeaderPlace
            // 
            this.columnHeaderPlace.Text = "#";
            this.columnHeaderPlace.Width = 19;
            // 
            // columnHeaderPlayer
            // 
            this.columnHeaderPlayer.Text = "Player";
            this.columnHeaderPlayer.Width = 313;
            // 
            // columnHeaderLevel
            // 
            this.columnHeaderLevel.Text = "Level";
            // 
            // FormLeaderboards
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 490);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.listViewLeaders);
            this.Name = "FormLeaderboards";
            this.ShowIcon = false;
            this.Text = "Leaderboards";
            this.Load += new System.EventHandler(this.FormLeaderboards_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewLeaders;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ColumnHeader columnHeaderPlace;
        private System.Windows.Forms.ColumnHeader columnHeaderPlayer;
        private System.Windows.Forms.ColumnHeader columnHeaderLevel;
    }
}