namespace TicTacToe
{
    partial class FormGame<TNetworkAddress>
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
            this.tableLayoutPanelGame = new System.Windows.Forms.TableLayoutPanel();
            this.labelGameWith = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelYourId = new System.Windows.Forms.Label();
            this.textBoxYourId = new System.Windows.Forms.TextBox();
            this.textBoxGameWith = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tableLayoutPanelGame
            // 
            this.tableLayoutPanelGame.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanelGame.ColumnCount = 3;
            this.tableLayoutPanelGame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelGame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelGame.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelGame.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanelGame.Name = "tableLayoutPanelGame";
            this.tableLayoutPanelGame.RowCount = 3;
            this.tableLayoutPanelGame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelGame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelGame.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelGame.Size = new System.Drawing.Size(200, 200);
            this.tableLayoutPanelGame.TabIndex = 0;
            // 
            // labelGameWith
            // 
            this.labelGameWith.AutoSize = true;
            this.labelGameWith.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGameWith.ForeColor = System.Drawing.SystemColors.Highlight;
            this.labelGameWith.Location = new System.Drawing.Point(218, 37);
            this.labelGameWith.Name = "labelGameWith";
            this.labelGameWith.Size = new System.Drawing.Size(83, 18);
            this.labelGameWith.TabIndex = 1;
            this.labelGameWith.Text = "Game with:";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(221, 61);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(90, 13);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "Flipping the coin..";
            // 
            // labelYourId
            // 
            this.labelYourId.AutoSize = true;
            this.labelYourId.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYourId.ForeColor = System.Drawing.Color.ForestGreen;
            this.labelYourId.Location = new System.Drawing.Point(218, 13);
            this.labelYourId.Name = "labelYourId";
            this.labelYourId.Size = new System.Drawing.Size(57, 17);
            this.labelYourId.TabIndex = 3;
            this.labelYourId.Text = "Your id:";
            // 
            // textBoxYourId
            // 
            this.textBoxYourId.Location = new System.Drawing.Point(307, 12);
            this.textBoxYourId.Name = "textBoxYourId";
            this.textBoxYourId.ReadOnly = true;
            this.textBoxYourId.Size = new System.Drawing.Size(357, 20);
            this.textBoxYourId.TabIndex = 4;
            // 
            // textBoxGameWith
            // 
            this.textBoxGameWith.Location = new System.Drawing.Point(307, 38);
            this.textBoxGameWith.Name = "textBoxGameWith";
            this.textBoxGameWith.ReadOnly = true;
            this.textBoxGameWith.Size = new System.Drawing.Size(357, 20);
            this.textBoxGameWith.TabIndex = 4;
            // 
            // FormGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 325);
            this.Controls.Add(this.textBoxGameWith);
            this.Controls.Add(this.textBoxYourId);
            this.Controls.Add(this.labelYourId);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelGameWith);
            this.Controls.Add(this.tableLayoutPanelGame);
            this.Name = "FormGame";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormGame_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelGame;
        private System.Windows.Forms.Label labelGameWith;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelYourId;
        private System.Windows.Forms.TextBox textBoxYourId;
        private System.Windows.Forms.TextBox textBoxGameWith;
    }
}

