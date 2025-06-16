namespace Client
{
    partial class ClientForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button buttonHit;
        private System.Windows.Forms.Button buttonStand;
        private System.Windows.Forms.TextBox textBoxMessages;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.buttonHit = new System.Windows.Forms.Button();
            this.buttonStand = new System.Windows.Forms.Button();
            this.textBoxMessages = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonHit
            // 
            this.buttonHit.Location = new System.Drawing.Point(12, 12);
            this.buttonHit.Name = "buttonHit";
            this.buttonHit.Size = new System.Drawing.Size(75, 30);
            this.buttonHit.TabIndex = 0;
            this.buttonHit.Text = "HIT";
            this.buttonHit.UseVisualStyleBackColor = true;
            this.buttonHit.Click += new System.EventHandler(this.buttonHit_Click);
            // 
            // buttonStand
            // 
            this.buttonStand.Location = new System.Drawing.Point(93, 12);
            this.buttonStand.Name = "buttonStand";
            this.buttonStand.Size = new System.Drawing.Size(75, 30);
            this.buttonStand.TabIndex = 1;
            this.buttonStand.Text = "STAND";
            this.buttonStand.UseVisualStyleBackColor = true;
            this.buttonStand.Click += new System.EventHandler(this.buttonStand_Click);
            // 
            // textBoxMessages
            // 
            this.textBoxMessages.Location = new System.Drawing.Point(12, 50);
            this.textBoxMessages.Multiline = true;
            this.textBoxMessages.Name = "textBoxMessages";
            this.textBoxMessages.ReadOnly = true;
            this.textBoxMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMessages.Size = new System.Drawing.Size(360, 200);
            this.textBoxMessages.TabIndex = 2;
            // 
            // ClientForm
            // 
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.textBoxMessages);
            this.Controls.Add(this.buttonStand);
            this.Controls.Add(this.buttonHit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ClientForm";
            this.Text = "Blackjack Multiplayer Client";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
