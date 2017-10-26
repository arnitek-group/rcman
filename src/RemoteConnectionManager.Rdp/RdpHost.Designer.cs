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
            this.axMsRdpClient = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
            ((System.ComponentModel.ISupportInitialize)(this.axMsRdpClient)).BeginInit();
            this.SuspendLayout();
            // 
            // axMsRdpClient
            // 
            this.axMsRdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axMsRdpClient.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            this.axMsRdpClient.Enabled = true;
            this.axMsRdpClient.Location = new System.Drawing.Point(0, 0);
            this.axMsRdpClient.Name = "axMsRdpClient";
            this.axMsRdpClient.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMsRdpClient.OcxState")));
            this.axMsRdpClient.Size = new System.Drawing.Size(640, 480);
            this.axMsRdpClient.TabIndex = 0;
            // 
            // RdpHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(this.axMsRdpClient);
            this.Name = "RdpHost";
            this.Size = new System.Drawing.Size(640, 480);
            ((System.ComponentModel.ISupportInitialize)(this.axMsRdpClient)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        
        private AxMSTSCLib.AxMsRdpClient9NotSafeForScripting axMsRdpClient;
    }
}
