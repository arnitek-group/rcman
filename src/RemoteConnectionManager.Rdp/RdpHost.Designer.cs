using System.Windows.Forms;

namespace RemoteConnectionManager.Rdp
{
    partial class RdpHost
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RdpHost));
            this.AxMsRdpClient = Rdp.Client;
            ((System.ComponentModel.ISupportInitialize)(this.AxMsRdpClient)).BeginInit();
            this.SuspendLayout();
            // 
            // AxMsRdpClient
            // 
            this.AxMsRdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AxMsRdpClient.Enabled = true;
            this.AxMsRdpClient.Location = new System.Drawing.Point(0, 0);
            this.AxMsRdpClient.Name = "AxMsRdpClient";
            this.AxMsRdpClient.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("AxMsRdpClient.OcxState")));
            this.AxMsRdpClient.Size = new System.Drawing.Size(640, 480);
            this.AxMsRdpClient.TabIndex = 0;
            // 
            // RdpHost
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.AxMsRdpClient);
            this.Name = "RdpHost";
            this.Size = new System.Drawing.Size(640, 480);
            ((System.ComponentModel.ISupportInitialize)(this.AxMsRdpClient)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxHost AxMsRdpClient;
    }
}
